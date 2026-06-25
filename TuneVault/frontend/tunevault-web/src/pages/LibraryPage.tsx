import { FormEvent, useEffect, useState } from 'react';
import { favoritesApi, mediaApi } from '../api/services';
import type { MediaItem } from '../api/types';
import { MediaCard } from '../components/MediaCard';
import { useAuth } from '../context/AuthContext';

export function LibraryPage() {
  const { isAuthenticated } = useAuth();
  const [favorites, setFavorites] = useState<MediaItem[]>([]);
  const [uploading, setUploading] = useState(false);
  const [message, setMessage] = useState('');

  const loadFavorites = async () => {
    if (!isAuthenticated) return;
    setFavorites(await favoritesApi.list());
  };

  useEffect(() => {
    void loadFavorites();
  }, [isAuthenticated]);

  const onUpload = async (e: FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    if (!isAuthenticated) return;
    const form = e.currentTarget;
    const formData = new FormData(form);
    setUploading(true);
    setMessage('');
    try {
      await mediaApi.upload(formData);
      setMessage('Upload successful');
      form.reset();
    } catch (err) {
      setMessage(err instanceof Error ? err.message : 'Upload failed');
    } finally {
      setUploading(false);
    }
  };

  return (
    <div className="space-y-8">
      <section>
        <h2 className="text-2xl font-bold mb-4">Your favorites</h2>
        {!isAuthenticated ? (
          <p className="text-spotify-light">Login to see favorites.</p>
        ) : (
          <div className="grid grid-cols-2 md:grid-cols-4 gap-4">
            {favorites.map((item) => <MediaCard key={item.id} item={item} />)}
          </div>
        )}
      </section>
      <section>
        <h2 className="text-2xl font-bold mb-4">Upload media</h2>
        {!isAuthenticated ? (
          <p className="text-spotify-light">Login to upload.</p>
        ) : (
          <form onSubmit={onUpload} className="max-w-xl space-y-3 bg-spotify-gray p-4 rounded-lg">
            <input name="title" placeholder="Title" required className="w-full p-2 rounded bg-spotify-dark" />
            <input name="artist" placeholder="Artist" required className="w-full p-2 rounded bg-spotify-dark" />
            <input name="genre" placeholder="Genre" required className="w-full p-2 rounded bg-spotify-dark" />
            <select name="type" className="w-full p-2 rounded bg-spotify-dark">
              <option value="Audio">Audio</option>
              <option value="Video">Video</option>
            </select>
            <input name="durationSeconds" type="number" defaultValue={180} className="w-full p-2 rounded bg-spotify-dark" />
            <input name="description" placeholder="Description" className="w-full p-2 rounded bg-spotify-dark" />
            <input name="file" type="file" accept=".mp3,.wav,.mp4,.webm" required className="w-full" />
            <button disabled={uploading} className="bg-spotify-green text-black px-4 py-2 rounded-full font-semibold">
              {uploading ? 'Uploading...' : 'Upload'}
            </button>
            {message && <p className="text-sm text-spotify-light">{message}</p>}
          </form>
        )}
      </section>
    </div>
  );
}
