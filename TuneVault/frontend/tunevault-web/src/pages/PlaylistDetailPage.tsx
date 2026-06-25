import { useEffect, useState } from 'react';
import { useParams } from 'react-router-dom';
import { playlistApi } from '../api/services';
import type { PlaylistDetail } from '../api/types';
import { MediaCard } from '../components/MediaCard';

export function PlaylistDetailPage() {
  const { id } = useParams();
  const [playlist, setPlaylist] = useState<PlaylistDetail | null>(null);
  const [error, setError] = useState('');

  useEffect(() => {
    if (!id) return;
    void playlistApi.get(id).then(setPlaylist).catch((err) => setError(err.message));
  }, [id]);

  if (error) return <p className="text-red-400">{error}</p>;
  if (!playlist) return <p className="text-spotify-light">Loading playlist...</p>;

  return (
    <div>
      <h2 className="text-3xl font-bold">{playlist.name}</h2>
      <p className="text-spotify-light mb-1">{playlist.description}</p>
      <p className="text-sm text-spotify-light mb-6">By {playlist.ownerName}</p>
      <div className="grid grid-cols-2 md:grid-cols-3 xl:grid-cols-4 gap-4">
        {playlist.tracks.map((track) => <MediaCard key={track.id} item={track} />)}
      </div>
    </div>
  );
}
