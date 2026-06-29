
function PlayerBar({ player }) {
  const { currentSong, isPlaying, currentTime, volume } = player;

  if (!currentSong) {
    return (
      <footer className="flex h-20 items-center justify-center border-t border-spotify-gray bg-spotify-dark text-sm text-spotify-light">
        Chọn một bài hát để bắt đầu nghe
      </footer>
    );
  }

  return (
    <footer className="flex h-20 items-center gap-4 border-t border-spotify-gray bg-spotify-dark px-4">
      <MediaSurface isVideo={currentSong.isVideo()} />

      <div className="hidden min-w-0 sm:block sm:w-44">
        <p className="truncate text-sm font-medium text-white">{currentSong.title}</p>
        <p className="truncate text-xs text-spotify-light">{currentSong.artist}</p>
      </div>

      <div className="flex flex-1 flex-col items-center gap-1">
        <div className="flex items-center gap-4">
          <button onClick={() => mediaPlayer.previous()} className="text-lg text-spotify-light hover:text-white">⏮</button>
          <button
            onClick={() => mediaPlayer.togglePlayPause()}
            className="flex h-9 w-9 items-center justify-center rounded-full bg-white text-black"
          >
            {isPlaying ? '⏸' : '▶'}
          </button>
          <button onClick={() => mediaPlayer.next()} className="text-lg text-spotify-light hover:text-white">⏭</button>
        </div>
        <div className="flex w-full max-w-md items-center gap-2 text-xs text-spotify-light">
          <span>{formatTime(currentTime)}</span>
          <input
            type="range" min={0} max={currentSong.durationInSeconds || 0} value={currentTime}
            onChange={(e) => mediaPlayer.seekTo(Number(e.target.value))}
            className="h-1 flex-1 accent-spotify-green"
          />
          <span>{currentSong.getFormattedDuration()}</span>
        </div>
      </div>

      <div className="hidden items-center gap-2 sm:flex">
        <span aria-hidden>🔊</span>
        <input
          type="range" min={0} max={1} step={0.01} value={volume}
          onChange={(e) => mediaPlayer.setVolume(Number(e.target.value))}
          className="h-1 w-24 accent-spotify-green"
        />
      </div>
    </footer>
  );
}
