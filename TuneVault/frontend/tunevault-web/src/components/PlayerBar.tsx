import { usePlayer } from '../context/PlayerContext';

export function PlayerBar() {
  const { current, isPlaying, toggle, audioRef } = usePlayer();

  if (!current || current.type === 'Video') return null;

  return (
    <div className="fixed bottom-0 left-0 right-0 h-20 bg-spotify-gray border-t border-black px-4 flex items-center gap-4 z-50">
      <audio ref={audioRef} onEnded={() => toggle()} />
      <div className="min-w-[200px]">
        <p className="font-medium">{current.title}</p>
        <p className="text-sm text-spotify-light">{current.artist}</p>
      </div>
      <button
        onClick={toggle}
        className="w-10 h-10 rounded-full bg-white text-black font-bold flex items-center justify-center"
      >
        {isPlaying ? '❚❚' : '▶'}
      </button>
      <div className="text-sm text-spotify-light">{current.durationSeconds}s</div>
    </div>
  );
}
