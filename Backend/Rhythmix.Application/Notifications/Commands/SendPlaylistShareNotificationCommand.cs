using Dapper;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using Rhythmix.Application.Common.Interfaces;

namespace Rhythmix.Application.Notifications.Commands
{
    /// <summary>
    /// Gọi command này ngay sau khi share playlist thành công để tạo thông báo cho người nhận
    /// </summary>
    public record SendPlaylistShareNotificationCommand(
        string ReceiverUserId,  // người nhận thông báo
        string SenderName,      // tên người gửi
        string PlaylistName     // tên playlist được share
    ) : IRequest;

    public class SendPlaylistShareNotificationCommandHandler : IRequestHandler<SendPlaylistShareNotificationCommand>
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly INotificationHub _notificationHub;

        public SendPlaylistShareNotificationCommandHandler(
            IDbConnectionFactory connectionFactory,
            INotificationHub notificationHub)
        {
            _connectionFactory = connectionFactory;
            _notificationHub   = notificationHub;
        }

        public async Task Handle(SendPlaylistShareNotificationCommand request, CancellationToken cancellationToken)
        {
            var payload = $"{{\"SenderName\":\"{request.SenderName}\",\"PlaylistName\":\"{request.PlaylistName}\"}}";

            // 1. Lưu vào DB
            using var connection = _connectionFactory.CreateConnection();
            const string sql = @"
                INSERT INTO Notifications (NotificationId, UserId, Type, Payload, IsRead, CreatedAt)
                VALUES (@NotificationId, @UserId, @Type, @Payload, 0, @CreatedAt)";

            await connection.ExecuteAsync(sql, new
            {
                NotificationId = Guid.NewGuid(),
                UserId         = request.ReceiverUserId,
                Type           = "PlaylistShare",
                Payload        = payload,
                CreatedAt      = DateTime.UtcNow
            });

            // 2. Push real-time qua SignalR
            await _notificationHub.SendNotification(request.ReceiverUserId, new
            {
                Type      = "PlaylistShare",
                Message   = $"{request.SenderName} đã chia sẻ playlist \"{request.PlaylistName}\" với bạn",
                CreatedAt = DateTime.UtcNow
            });
        }
    }
}
