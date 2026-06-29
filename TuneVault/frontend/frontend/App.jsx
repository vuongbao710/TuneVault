
function App() {
  const [currentUser, setCurrentUser] = React.useState(null);
  const [view, setView] = React.useState({ page: 'home' });
  const [searchKeyword, setSearchKeyword] = React.useState('');
  const [playerState, setPlayerState] = React.useState(mediaPlayer.state);
  const [notifications, setNotifications] = React.useState(mockNotificationsSeed);
  const playlists = React.useMemo(() => buildPlaylists(), []);

  // Đăng ký lắng nghe MediaPlayer (Observer pattern) -> re-render khi trạng thái đổi.
  React.useEffect(() => {
    const unsubscribe = mediaPlayer.subscribe(setPlayerState);
    return unsubscribe;
  }, []);

  // Giả lập 1 thông báo real-time đến sau 8 giây (thay cho SignalR thật).
  React.useEffect(() => {
    if (!currentUser) return;
    const timer = setTimeout(() => {
      setNotifications((prev) => [
        { id: 'n' + Date.now(), message: 'Có người vừa thích playlist "Top Hits Việt" của bạn', isRead: false },
        ...prev,
      ]);
    }, 8000);
    return () => clearTimeout(timer);
  }, [currentUser]);

  if (!currentUser) {
    return <LoginPage onLogin={setCurrentUser} />;
  }

  const unreadCount = notifications.filter((n) => !n.isRead).length;

  function renderPage() {
    if (view.page === 'home') return <HomePage playlists={playlists} onOpenPlaylist={(id) => setView({ page: 'playlistDetail', id })} />;
    if (view.page === 'playlists') return <PlaylistsPage playlists={playlists} onOpenPlaylist={(id) => setView({ page: 'playlistDetail', id })} />;
    if (view.page === 'playlistDetail') {
      const playlist = playlists.find((p) => p.id === view.id);
      return <PlaylistDetailPage playlist={playlist} playerState={playerState} />;
    }
    if (view.page === 'search') return <SearchPage keyword={searchKeyword} playerState={playerState} />;
    if (view.page === 'profile') {
      return (
        <ProfilePage
          currentUser={currentUser}
          notifications={notifications}
          onMarkAllRead={() => setNotifications((prev) => prev.map((n) => ({ ...n, isRead: true })))}
        />
      );
    }
    return null;
  }

  return (
    <div className="flex h-screen flex-col bg-spotify-black">
      <div className="flex flex-1 overflow-hidden">
        <Sidebar currentView={view.page} onNavigate={(page) => setView({ page })} />
        <div className="flex flex-1 flex-col overflow-hidden">
          <TopBar
            currentUser={currentUser}
            onLogout={() => setCurrentUser(null)}
            unreadCount={unreadCount}
            onSearch={(kw) => { setSearchKeyword(kw); setView({ page: 'search' }); }}
          />
          <main className="flex-1 overflow-y-auto px-6 py-4">{renderPage()}</main>
        </div>
        <RightPanel currentSong={playerState.currentSong} />
      </div>
      <PlayerBar player={playerState} />
    </div>
  );
}

ReactDOM.createRoot(document.getElementById('root')).render(<App />);
