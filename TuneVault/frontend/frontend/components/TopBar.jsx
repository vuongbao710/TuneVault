/* ============================================================
   COMPONENT: TopBar
   Thanh trên cùng: ô tìm kiếm + chuông thông báo + nút đăng xuất.
   ============================================================ */
function TopBar({ currentUser, onLogout, onSearch, unreadCount }) {
  const [keyword, setKeyword] = React.useState('');
  return (
    <header className="flex items-center justify-between gap-4 bg-spotify-black px-6 py-3">
      <form
        onSubmit={(e) => { e.preventDefault(); onSearch(keyword); }}
        className="w-full max-w-md"
      >
        <input
          type="text"
          value={keyword}
          onChange={(e) => setKeyword(e.target.value)}
          placeholder="Bạn muốn nghe gì hôm nay?"
          className="w-full rounded-full bg-spotify-gray px-4 py-2 text-sm text-white placeholder-spotify-light focus:outline-none focus:ring-2 focus:ring-spotify-green"
        />
      </form>
      <div className="flex items-center gap-4">
        <div className="relative rounded-full p-2">
          🔔
          {unreadCount > 0 && (
            <span className="absolute -right-1 -top-1 flex h-4 w-4 items-center justify-center rounded-full bg-spotify-green text-[10px] font-bold text-black">
              {unreadCount}
            </span>
          )}
        </div>
        {currentUser && (
          <button onClick={onLogout} className="rounded-full bg-spotify-gray px-3 py-1 text-sm font-semibold hover:bg-spotify-dark">
            {currentUser.displayName} · Đăng xuất
          </button>
        )}
      </div>
    </header>
  );
}
