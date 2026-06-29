
function SearchPage({ keyword, playerState }) {
  const results = React.useMemo(() => {
    if (!keyword.trim()) return [];
    const k = keyword.toLowerCase();
    return mockSongsPool
      .filter((s) => s.title.toLowerCase().includes(k) || s.artist.toLowerCase().includes(k))
      .map((s) => new Song(s));
  }, [keyword]);

  return (
    <div className="flex flex-col gap-4">
      <h1 className="text-2xl font-bold text-white">{keyword ? `Kết quả cho "${keyword}"` : 'Nhập từ khoá để tìm kiếm'}</h1>
      {keyword && results.length === 0 && <p className="text-spotify-light">Không tìm thấy bài hát phù hợp.</p>}
      <div className="flex flex-col">
        {results.map((song, i) => (
          <SongRow key={song.id} song={song} index={i} queue={results} currentSong={playerState.currentSong} isPlaying={playerState.isPlaying} />
        ))}
      </div>
    </div>
  );
}
