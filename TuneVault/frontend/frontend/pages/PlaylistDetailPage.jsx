/* ============================================================
   PAGE: PlaylistDetailPage
   Trang chi tiết 1 playlist: nút "Phát tất cả" + danh sách bài hát.
   ============================================================ */
function PlaylistDetailPage({ playlist, playerState }) {
  if (!playlist) return <p className="text-spotify-light">Không tìm thấy playlist.</p>;
  return (
    <div className="flex flex-col gap-6">
      <div className="flex items-end gap-6">
        <div className="flex h-40 w-40 flex-shrink-0 items-center justify-center rounded-md bg-spotify-gray text-5xl">🎵</div>
        <div>
          <p className="text-xs uppercase text-spotify-light">Playlist</p>
          <h1 className="text-3xl font-bold text-white">{playlist.name}</h1>
          <p className="mt-2 text-sm text-spotify-light">{playlist.songCount} bài hát · {playlist.getTotalDurationFormatted()}</p>
        </div>
      </div>
      <button
        onClick={() => mediaPlayer.playQueue(playlist.songs, 0)}
        disabled={playlist.songCount === 0}
        className="w-fit rounded-full bg-spotify-green px-5 py-2 text-sm font-semibold text-black hover:brightness-110 disabled:opacity-50"
      >
        ▶ Phát tất cả
      </button>
      <div className="flex flex-col">
        {playlist.songs.map((song, i) => (
          <SongRow key={song.id} song={song} index={i} queue={playlist.songs} currentSong={playerState.currentSong} isPlaying={playerState.isPlaying} />
        ))}
      </div>
    </div>
  );
}
