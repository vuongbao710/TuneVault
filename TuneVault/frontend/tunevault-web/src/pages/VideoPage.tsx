import { useEffect, useState } from 'react';
import { useParams } from 'react-router-dom';
import { getStreamUrl } from '../api/client';
import { mediaApi } from '../api/services';
import type { MediaItem } from '../api/types';

export function VideoPage() {
  const { id } = useParams();
  const [item, setItem] = useState<MediaItem | null>(null);

  useEffect(() => {
    if (!id) return;
    void mediaApi.get(id).then(setItem);
    void mediaApi.recordHistory(id).catch(() => undefined);
  }, [id]);

  if (!item) return <p className="text-spotify-light">Loading video...</p>;

  return (
    <div>
      <h2 className="text-2xl font-bold mb-2">{item.title}</h2>
      <p className="text-spotify-light mb-4">{item.artist}</p>
      <video
        controls
        className="w-full max-w-4xl rounded-lg bg-black"
        src={getStreamUrl(item.id)}
      />
    </div>
  );
}
