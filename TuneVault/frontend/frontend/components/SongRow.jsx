
function SongRow({ song, index, queue, currentSong, isPlaying }) {
  const isCurrent = currentSong && currentSong.id === song.id;
  function handleClick() {
    if (isCurrent) mediaPlayer.togglePlayPause();
    else mediaPlayer.playQueue(queue, index);
  }
  return (
    <button
      onClick={handleClick}
      className={`group grid w-full grid-cols-[2rem_1fr_4rem] items-center gap-3 rounded px-3 py-2 text-left hover:bg-spotify-dark ${isCurrent ? 'text-spotify-green' : 'text-white'}`}
    >
      <span className="relative h-4 w-4 text-sm text-spotify-light">
        <span className="absolute inset-0 group-hover:hidden">{index + 1}</span>
        <span className="absolute inset-0 hidden group-hover:block">{isCurrent && isPlaying ? '⏸' : '▶'}</span>
      </span>
      <span className="min-w-0">
        <span className="block truncate text-sm font-medium">{song.title}</span>
        <span className="block truncate text-xs text-spotify-light">{song.artist}</span>
      </span>
      <span className="text-right text-xs text-spotify-light">{song.getFormattedDuration()}</span>
    </button>
  );
}
