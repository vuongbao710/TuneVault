/* ============================================================
   COMPONENT: RightPanel
   Panel bên phải hiển thị chi tiết bài hát đang phát.
   ============================================================ */
function RightPanel({ currentSong }) {
  if (!currentSong) {
    return (
      <aside className="hidden w-72 flex-col items-center justify-center bg-spotify-black p-6 text-center text-sm text-spotify-light lg:flex">
        Đang phát gì đó sẽ hiện chi tiết tại đây
      </aside>
    );
  }
  return (
    <aside className="hidden w-72 flex-col gap-4 bg-spotify-black p-6 lg:flex">
      <div className="flex aspect-square w-full items-center justify-center rounded-md bg-spotify-gray text-5xl">
        {currentSong.isVideo() ? '🎬' : '🎵'}
      </div>
      <div>
        <h2 className="text-lg font-bold text-white">{currentSong.title}</h2>
        <p className="text-sm text-spotify-light">{currentSong.artist}</p>
        {currentSong.album && <p className="mt-1 text-xs text-spotify-light">Album: {currentSong.album}</p>}
      </div>
      <div className="rounded-md bg-spotify-dark p-3 text-xs text-spotify-light">
        Thể loại: {currentSong.isVideo() ? 'Video' : 'Audio'} · Thời lượng: {currentSong.getFormattedDuration()}
      </div>
    </aside>
  );
}
