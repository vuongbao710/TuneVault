import { useEffect, useState } from 'react';
import { mediaApi, shareApi } from '../api/services';
import type { MediaItem, ShareItem } from '../api/types';
import { MediaCard } from '../components/MediaCard';
import { useAuth } from '../context/AuthContext';

export function SharesPage() {
  const { user } = useAuth();
  const [withMe, setWithMe] = useState<ShareItem[]>([]);
  const [byMe, setByMe] = useState<ShareItem[]>([]);
  const [media, setMedia] = useState<MediaItem[]>([]);
  const [receiverId, setReceiverId] = useState('');
  const [selectedMedia, setSelectedMedia] = useState<MediaItem | null>(null);
  const [message, setMessage] = useState('');

  const load = async () => {
    setWithMe(await shareApi.withMe());
    setByMe(await shareApi.byMe());
    setMedia(await mediaApi.list());
  };

  useEffect(() => {
    void load();
  }, []);

  const share = async () => {
    if (!selectedMedia || !receiverId) return;
    setMessage('');
    try {
      await shareApi.share({ receiverUserId: receiverId, mediaItemId: selectedMedia.id });
      setMessage(`Shared "${selectedMedia.title}" successfully`);
      setSelectedMedia(null);
      setReceiverId('');
      await load();
    } catch (err) {
      setMessage(err instanceof Error ? err.message : 'Share failed');
    }
  };

  return (
    <div className="space-y-8">
      <section className="bg-spotify-gray p-4 rounded-lg">
        <h2 className="text-xl font-bold mb-3">Share a track</h2>
        <p className="text-sm text-spotify-light mb-2">Your user id: <code>{user?.userId}</code></p>
        {selectedMedia ? (
          <p className="mb-2">Selected: {selectedMedia.title}</p>
        ) : (
          <p className="text-sm text-spotify-light mb-2">Pick a track below and enter receiver user id (e.g. Bob).</p>
        )}
        <input
          value={receiverId}
          onChange={(e) => setReceiverId(e.target.value)}
          placeholder="Receiver user id"
          className="w-full max-w-md p-2 rounded bg-spotify-dark mb-2"
        />
        <button onClick={() => void share()} className="bg-spotify-green text-black px-4 py-2 rounded-full font-semibold">
          Send share
        </button>
        {message && <p className="text-sm mt-2 text-spotify-light">{message}</p>}
        <div className="grid grid-cols-2 md:grid-cols-4 gap-3 mt-4">
          {media.slice(0, 8).map((item) => (
            <button
              key={item.id}
              type="button"
              onClick={() => setSelectedMedia(item)}
              className={`text-left p-2 rounded ${selectedMedia?.id === item.id ? 'ring-2 ring-spotify-green' : 'bg-spotify-dark'}`}
            >
              <p className="text-sm font-medium truncate">{item.title}</p>
              <p className="text-xs text-spotify-light truncate">{item.artist}</p>
            </button>
          ))}
        </div>
      </section>

      <section>
        <h2 className="text-2xl font-bold mb-4">Shared with me</h2>
        <div className="space-y-3">
          {withMe.map((share) => (
            <div key={share.id} className="bg-spotify-gray p-3 rounded-lg">
              <p>{share.senderName} shared {share.media?.title || share.playlistName}</p>
              {share.media && <MediaCard item={share.media} onShare={setSelectedMedia} />}
            </div>
          ))}
        </div>
      </section>

      <section>
        <h2 className="text-2xl font-bold mb-4">Shared by me</h2>
        <ul className="space-y-2 text-spotify-light">
          {byMe.map((share) => (
            <li key={share.id}>To {share.receiverName}: {share.media?.title || share.playlistName}</li>
          ))}
        </ul>
      </section>
    </div>
  );
}
