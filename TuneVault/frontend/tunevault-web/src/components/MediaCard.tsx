import { Link } from 'react-router-dom';
import { usePlayer } from '../context/PlayerContext';
import type { MediaItem } from '../api/types';

export function MediaCard({ item, onShare }: { item: MediaItem; onShare?: (item: MediaItem) => void }) {
  const { play } = usePlayer();

  return (
    <div className="bg-spotify-gray rounded-lg p-4 hover:bg-[#3e3e3e] transition-colors">
      <div className="aspect-square rounded-md bg-gradient-to-br from-purple-700 to-spotify-green mb-3 flex items-center justify-center text-3xl">
        {item.type === 'Video' ? '🎬' : '🎵'}
      </div>
      <h3 className="font-semibold truncate">{item.title}</h3>
      <p className="text-sm text-spotify-light truncate">{item.artist}</p>
      <p className="text-xs text-spotify-light mt-1">{item.genre}</p>
      <div className="flex gap-2 mt-3">
        {item.type === 'Video' ? (
          <Link to={`/video/${item.id}`} className="text-xs bg-spotify-green text-black px-3 py-1 rounded-full font-semibold">
            Watch
          </Link>
        ) : (
          <button onClick={() => void play(item)} className="text-xs bg-spotify-green text-black px-3 py-1 rounded-full font-semibold">
            Play
          </button>
        )}
        {onShare && (
          <button onClick={() => onShare(item)} className="text-xs border border-spotify-light px-3 py-1 rounded-full">
            Share
          </button>
        )}
      </div>
    </div>
  );
}
