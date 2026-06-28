/* ============================================================
   PAGE: PlaylistsPage
   Trang danh sách tất cả playlist của người dùng.
   ============================================================ */
function PlaylistsPage({ playlists, onOpenPlaylist }) {
  return (
    <div className="flex flex-col gap-6">
      <h1 className="text-2xl font-bold text-white">Playlist của bạn</h1>
      <div className="flex flex-wrap gap-4">
        {playlists.map((p) => <SongCard key={p.id} playlist={p} onOpen={onOpenPlaylist} />)}
      </div>
    </div>
  );
}
