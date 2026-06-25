import { Link } from 'react-router-dom';
import { useEffect, useState } from 'react';
import { historyApi, mediaApi, playlistApi } from '../api/services';
import type { MediaItem, Playlist } from '../api/types';
import { MediaCard } from '../components/MediaCard';
import { useAuth } from '../context/AuthContext';

export function HomePage() {
  const { isAuthenticated } = useAuth();
  const [media, setMedia] = useState<MediaItem[]>([]);
  const [playlists, setPlaylists] = useState<Playlist[]>([]);
  const [history, setHistory] = useState<MediaItem[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  useEffect(() => {
    void (async () => {
      try {
        setLoading(true);
        const [mediaItems, playlistItems] = await Promise.all([mediaApi.list(), playlistApi.list()]);
        setMedia(mediaItems.slice(0, 8));
        setPlaylists(playlistItems.slice(0, 4));
        if (isAuthenticated) {
          setHistory(await historyApi.list());
        }
      } catch (err) {
        setError(err instanceof Error ? err.message : 'Failed to load home feed');
      } finally {
        setLoading(false);
      }
    })();
  }, [isAuthenticated]);

  if (loading) return <p className="text-spotify-light">Loading...</p>;
  if (error) return <p className="text-red-400">{error}</p>;

  return (
    <div className="space-y-8">
      <section>
        <h2 className="text-2xl font-bold mb-4">Popular on TuneVault</h2>
        <div className="grid grid-cols-2 md:grid-cols-3 xl:grid-cols-4 gap-4">
          {media.map((item) => <MediaCard key={item.id} item={item} />)}
        </div>
      </section>
      <section>
        <h2 className="text-2xl font-bold mb-4">Playlists</h2>
        <div className="grid grid-cols-1 md:grid-cols-2 gap-3">
          {playlists.map((p) => (
            <Link key={p.id} to={`/playlists/${p.id}`} className="bg-spotify-gray rounded-lg p-4 hover:bg-[#3e3e3e] block">
              <h3 className="font-semibold">{p.name}</h3>
              <p className="text-sm text-spotify-light">{p.trackCount} tracks • {p.ownerName}</p>
            </Link>
          ))}
        </div>
      </section>
      {history.length > 0 && (
        <section>
          <h2 className="text-2xl font-bold mb-4">Recently played</h2>
          <div className="grid grid-cols-2 md:grid-cols-4 gap-4">
            {history.map((item) => <MediaCard key={item.id} item={item} />)}
          </div>
        </section>
      )}
    </div>
  );
}
