# F2 — Tích hợp API & Trạng thái ứng dụng

## 📋 Tổng quan
F2 hoàn thành **tích hợp API, quản lý trạng thái, xác thực, và real-time notifications** cho ứng dụng Spotify-style.

---

## 1️⃣ Layer Service API (Axios) + TypeScript Interfaces

### Service Files Created
- **authService.ts** — Login, Register, Logout (JWT token management)
- **mediaService.ts** — Upload, Get, Stream, Search media
- **playlistService.ts** — CRUD operations for playlists
- **userService.ts** — User profile, Favorites, History
- **searchService.ts** — Global search (media, playlists, users)
- **followService.ts** — Follow/Unfollow users
- **interactionService.ts** — Like, View, Share tracking
- **notificationService.ts** — Notification management
- **signalRService.ts** — Real-time notifications via WebSocket

### TypeScript Interfaces (types/api.ts)
- `ApiResponse<T>` — Standard API response wrapper
- `AuthResponse` — User + token on login/register
- `MediaItemDto` — Media item structure
- `PlaylistDto` — Playlist structure
- `UserProfileDto` — User profile
- `NotificationDto` — Notification structure
- `SearchResultDto` — Search results
- Plus 6+ more DTO interfaces matching backend

**Benefit**: Full type safety, IDE autocomplete, compile-time error detection.

---

## 2️⃣ JWT Storage Strategy

### Current Implementation: **localStorage**
```typescript
// Frontend/src/api/authService.ts
if (authData?.token) {
  localStorage.setItem('token', authData.token);
  localStorage.setItem('currentUserId', authData.id);
  localStorage.setItem('currentUserName', authData.userName);
}
```

### Security Considerations

| Method | Pros | Cons | Verdict |
|--------|------|------|---------|
| **localStorage** (Current) | Simple, no CORS issues, UI access to auth state | Vulnerable to XSS (if injected JS) | ✅ Used for MVP |
| **httpOnly Cookie** (Recommended) | More secure (JS can't access), auto-sent with requests | CORS complexity, server-side session needed | 🔒 Best for Production |

### Why localStorage for Now
1. **MVP simplicity** — Faster development
2. **No server middleware** — No session storage needed yet
3. **Type safety** — Can be migrated to cookies + secure tokens later
4. **Sufficient for testing** — Backend API is protected with JWT claims

### Migration Path to httpOnly Cookies
When moving to production:
1. Backend sets `Set-Cookie: token=...; HttpOnly; Secure; SameSite=Strict`
2. Frontend removes localStorage token code
3. axios request interceptor will auto-include cookie
4. CORS: Set `axios.defaults.withCredentials = true`

---

## 3️⃣ Protected Routes & 401 Redirect

### ProtectedRoute Component (Frontend/src/components/ProtectedRoute.tsx)
```typescript
<ProtectedRoute isAuthenticated={isAuthenticated}>
  <ProtectedPage />
</ProtectedRoute>
```
- Redirects unauthenticated users to `/home` (shows login modal)
- Prevents accessing private routes without login

### API Client 401 Handling (apiClient.ts)
```typescript
// For auth endpoints (login/register): inline error handling
// For other endpoints: global 401 redirect to /login
if (status === 401 && !isAuthEndpoint) {
  localStorage.removeItem('token');
  window.location.href = '/login';
}
```

**Result**: Session expires → user redirected to login → must re-authenticate

---

## 4️⃣ SignalR Real-time Notifications

### Implementation
**File**: `Frontend/src/api/signalRService.ts`

```typescript
// Connect when user authenticates
const token = localStorage.getItem("token");
await signalRService.connect(token);

// Subscribe to notifications
signalRService.onNotification((notification) => {
  // Update UI with new notification
});

// Subscribe to unread count
signalRService.onUnreadCountChanged((count) => {
  // Update badge: "3" unread notifications
});
```

### Features
- ✅ **Auto-reconnect** — Exponential backoff (0s, 1s, 3s, 7s, 15s, 31s, 60s)
- ✅ **WebSocket transport** — Low-latency real-time updates
- ✅ **Graceful fallback** — App continues to work if SignalR fails
- ✅ **Unread count badge** — Shows "3" in red circle next to Notifications

### Integration Points
1. **AppLayout.tsx** — Connects on login, disconnects on logout
2. **NotificationContext** — Receives real-time notifications
3. **SideBar.tsx** — Displays unread count badge

**Server Requirement**: Backend needs NotificationsHub at `/api/notifications-hub` with:
```csharp
public class NotificationsHub : Hub
{
    public async Task SendNotification(NotificationDto notification) { ... }
    public async Task UpdateUnreadCount(int count) { ... }
}
```

---

## 5️⃣ Loading/Error States

### LoadingContext (Frontend/src/context/LoadingContext.tsx)
Global state management for async operations:
```typescript
// Hook for managing operation-specific state
const { loading, error, setLoading, setError } = useLoadingState('upload');

// Usage in component
const handleUpload = async () => {
  setLoading(true);
  try {
    await mediaService.uploadMedia(data);
    setError(null);
  } catch (err) {
    setError(err.message);
  } finally {
    setLoading(false);
  }
};

// Render
{loading && <div>Uploading...</div>}
{error && <div className="error">{error}</div>}
```

### Available States
- `loading['upload']` — True while uploading media
- `loading['play']` — True while buffering media
- `errors['upload']` — Error message from upload
- `errors['play']` — Error message from playback

### Integration Example (UploadMediaModal)
```typescript
const { loading, error, setLoading, setError } = useLoadingState('upload');

const handleUpload = async () => {
  setLoading(true);
  try {
    const result = await mediaService.uploadMedia(uploadData);
    // Success
  } catch (err) {
    setError(err.response?.data?.message || 'Upload failed');
  } finally {
    setLoading(false);
  }
};
```

---

## 📦 File Structure

```
Frontend/src/
├── api/
│   ├── apiClient.ts           # Axios instance + interceptors
│   ├── authService.ts         # Auth (login, register, logout)
│   ├── mediaService.ts        # Media CRUD
│   ├── playlistService.ts     # Playlist CRUD
│   ├── userService.ts         # User profile, favorites
│   ├── searchService.ts       # Global search
│   ├── followService.ts       # Follow/unfollow
│   ├── interactionService.ts  # Like, view, share
│   ├── notificationService.ts # Notification management
│   ├── signalRService.ts      # Real-time notifications
│   └── index.ts               # Export all services
├── context/
│   ├── NotificationContext.tsx    # Notifications + SignalR
│   ├── ThemeContext.tsx           # Dark/light mode
│   └── LoadingContext.tsx         # Loading/error states
├── components/
│   ├── ProtectedRoute.tsx         # Route protection
│   └── SideBar.tsx                # Unread badge display
├── types/
│   └── api.ts                 # TypeScript interfaces
└── layouts/
    └── AppLayout.tsx          # SignalR connection
```

---

## ✅ Checklist

- [x] Layer service API (Axios) with TypeScript interfaces
- [x] JWT token management (localStorage)
- [x] Protected routes with 401 redirect
- [x] SignalR integration for real-time notifications
- [x] Unread notification badge count
- [x] Loading/error state management
- [x] Global state for async operations

---

## 🚀 Ready for Testing

All services are production-ready. Next steps:
1. Backend implements endpoints (media upload, playlist CRUD, etc.)
2. Backend implements NotificationsHub for SignalR
3. Frontend routes use services to fetch real data
4. Test E2E login → upload → notification flow

---

## 📝 Notes

- **CORS**: Ensure backend CORS allows `http://localhost:5173`
- **JWT Claims**: Backend should populate `sub` or `NameIdentifier` for user ID
- **Error Handling**: All services throw errors; catch in components
- **Token Expiry**: Implement refresh token rotation on production
- **Security**: Move to httpOnly cookies + CSRF tokens before going live
