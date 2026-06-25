import { useEffect, useState } from 'react';
import { searchApi } from '../api/services';
import type { MediaItem } from '../api/types';
import { MediaCard } from '../components/MediaCard';

export function SearchPage() {
  const [query, setQuery] = useState('');
  const [results, setResults] = useState<MediaItem[]>([]);
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    const timer = setTimeout(async () => {
      setLoading(true);
      try {
        const data = await searchApi.search(query);
        setResults(data.items);
      } finally {
        setLoading(false);
      }
    }, 300);
    return () => clearTimeout(timer);
  }, [query]);

  return (
    <div>
      <h2 className="text-2xl font-bold mb-4">Search</h2>
      <input
        value={query}
        onChange={(e) => setQuery(e.target.value)}
        placeholder="Search songs, artists, genres..."
        className="w-full max-w-xl p-3 rounded-full bg-spotify-gray outline-none mb-6"
      />
      {loading ? <p className="text-spotify-light">Searching...</p> : (
        <div className="grid grid-cols-2 md:grid-cols-3 xl:grid-cols-4 gap-4">
          {results.map((item) => <MediaCard key={item.id} item={item} />)}
        </div>
      )}
    </div>
  );
}
