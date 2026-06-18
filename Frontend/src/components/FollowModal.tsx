import { useNavigate } from "react-router-dom";
import { X } from "lucide-react";

type FollowModalProps = {
  isOpen: boolean;
  onClose: () => void;
  title: string;
  list: any[];
};

const FollowModal = ({ isOpen, onClose, title, list }: FollowModalProps) => {
  const navigate = useNavigate();

  if (!isOpen) return null;

  const handleUserClick = (userId: string) => {
    navigate(`/profile/${userId}`);
    onClose();
  };

  return (
    <div className="fixed inset-0 bg-black/70 backdrop-blur-sm flex items-center justify-center z-50 p-4">
      <div className="bg-zinc-900 p-6 rounded-2xl w-full max-w-sm border border-zinc-800 shadow-2xl">
        <div className="flex justify-between items-center mb-5">
          <h2 className="text-xl font-black text-white">{title}</h2>
          <button onClick={onClose} className="text-zinc-400 hover:text-white transition-colors">
            <X className="size-5" />
          </button>
        </div>

        <div className="space-y-3 max-h-80 overflow-y-auto pr-2 custom-scrollbar">
          {list.length > 0 ? (
            list.map((u) => {
              const id = u.id || u.Id;
              const name = u.displayName || u.DisplayName || u.userName || u.UserName || u.name || u.email || "Unknown user";
              const avatarUrl = u.avatarUrl || u.AvatarUrl || "";

              return (
                <div
                  key={id}
                  onClick={() => handleUserClick(id)}
                  className="flex items-center gap-3 p-2 rounded-lg hover:bg-zinc-800 cursor-pointer transition-colors"
                >
                  {avatarUrl ? (
                    <img src={avatarUrl} className="size-10 rounded-full object-cover" alt={name} />
                  ) : (
                    <div className="flex size-10 items-center justify-center rounded-full bg-zinc-800 text-sm font-bold text-white">
                      {name.slice(0, 1).toUpperCase()}
                    </div>
                  )}
                  <span className="text-white font-medium truncate">{name}</span>
                </div>
              );
            })
          ) : (
            <p className="text-zinc-500 text-sm text-center py-4">Chua co ai o day.</p>
          )}
        </div>
      </div>
    </div>
  );
};

export default FollowModal;
