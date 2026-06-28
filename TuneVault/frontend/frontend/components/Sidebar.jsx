/* ============================================================
   COMPONENT: Sidebar
   Menu điều hướng bên trái (Trang chủ, Tìm kiếm, Playlist, Hồ sơ).
   ============================================================ */
function Sidebar({ currentView, onNavigate }) {
  const items = [
    { key: 'home', label: 'Trang chủ', icon: '🏠' },
    { key: 'search', label: 'Tìm kiếm', icon: '🔍' },
    { key: 'playlists', label: 'Playlist của bạn', icon: '📁' },
    { key: 'profile', label: 'Hồ sơ', icon: '👤' },
  ];
  return (
    <aside className="hidden w-60 flex-col gap-2 bg-spotify-black p-4 md:flex">
      <div className="mb-6 px-2 text-2xl font-bold text-spotify-green">TuneVault</div>
      <nav className="flex flex-col gap-1">
        {items.map((item) => (
          <button
            key={item.key}
            onClick={() => onNavigate(item.key)}
            className={`flex items-center gap-3 rounded px-3 py-2 text-left text-sm font-medium transition-colors ${
              currentView === item.key ? 'bg-spotify-gray text-white' : 'text-spotify-light hover:bg-spotify-dark hover:text-white'
            }`}
          >
            <span aria-hidden>{item.icon}</span>
            {item.label}
          </button>
        ))}
      </nav>
    </aside>
  );
}
