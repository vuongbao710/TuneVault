/* ============================================================
   PAGE: ProfilePage
   Trang hồ sơ người dùng + danh sách thông báo gần đây.
   ============================================================ */
function ProfilePage({ currentUser, notifications, onMarkAllRead }) {
  return (
    <div className="flex flex-col gap-8">
      <div className="flex items-center gap-4">
        <div className="flex h-20 w-20 items-center justify-center rounded-full bg-spotify-green text-2xl font-bold text-black">
          {currentUser.getInitials()}
        </div>
        <div>
          <h1 className="text-2xl font-bold text-white">{currentUser.displayName}</h1>
          <p className="text-sm text-spotify-light">{currentUser.email}</p>
        </div>
      </div>
      <section>
        <div className="mb-3 flex items-center justify-between">
          <h2 className="text-lg font-semibold text-white">Thông báo gần đây</h2>
          <button onClick={onMarkAllRead} className="text-xs text-spotify-light hover:text-white">Đánh dấu đã đọc tất cả</button>
        </div>
        <ul className="flex flex-col gap-2">
          {notifications.map((n) => (
            <li key={n.id} className={`rounded-md p-3 text-sm ${n.isRead ? 'bg-spotify-dark text-spotify-light' : 'bg-spotify-gray text-white'}`}>
              {n.message}
            </li>
          ))}
        </ul>
      </section>
    </div>
  );
}
