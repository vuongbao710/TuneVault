/* ============================================================
   COMPONENT: SongCard
   Thẻ playlist hình vuông (dùng trong HomePage, PlaylistsPage).
   Tên file giữ theo bản gốc, dù thực chất hiển thị 1 Playlist.
   ============================================================ */
function SongCard({ playlist, onOpen }) {
  return (
    <button onClick={() => onOpen(playlist.id)} className="w-40 flex-shrink-0 rounded-md bg-spotify-dark p-3 text-left transition-colors hover:bg-spotify-gray">
      <div className="mb-3 flex aspect-square w-full items-center justify-center rounded-md bg-spotify-gray text-3xl">🎵</div>
      <p className="truncate text-sm font-semibold text-white">{playlist.name}</p>
      <p className="truncate text-xs text-spotify-light">{playlist.description || `${playlist.songCount} bài hát`}</p>
    </button>
  );
}
