using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Rhythmix.Application.Common.Models;
using Rhythmix.Application.Follows.Commands;
using Rhythmix.Application.Follows.Queries;

namespace Rhythmix.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class FollowsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public FollowsController(IMediator mediator)
            => _mediator = mediator;

        private string CurrentUserId => User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;

        /// <summary>
        /// Follow hoặc Unfollow một user/nghệ sĩ (toggle)
        /// </summary>
        [HttpPost("{followingId}")]
        public async Task<IActionResult> ToggleFollow(Guid followingId)
        {
            if (CurrentUserId == followingId.ToString())
                return BadRequest(ApiResponse<object>.ToFailure(new() { "Không thể tự follow bản thân." }));

            var message = await _mediator.Send(new ToggleFollowCommand(CurrentUserId, followingId));
            return Ok(ApiResponse<object>.ToSuccess(new { Message = message }));
        }

        /// <summary>
        /// Kiểm tra mình có đang follow một user không
        /// </summary>
        [HttpGet("{followingId}/status")]
        public async Task<IActionResult> GetFollowStatus(Guid followingId)
        {
            var isFollowing = await _mediator.Send(new GetFollowStatusQuery(CurrentUserId, followingId));
            return Ok(ApiResponse<object>.ToSuccess(new { IsFollowing = isFollowing }));
        }
    }
}
