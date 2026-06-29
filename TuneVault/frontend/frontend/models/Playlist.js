
class Playlist {
  constructor(data) {
    this.id = data.id;
    this.name = data.name;
    this.description = data.description || '';
    this._songs = (data.songs || []).map((s) => new Song(s));
  }

  get songs() {
    return this._songs;
  }

  get songCount() {
    return this._songs.length;
  }

  getTotalDurationFormatted() {
    const total = this._songs.reduce((sum, s) => sum + s.durationInSeconds, 0);
    const m = Math.floor(total / 60);
    const s = Math.floor(total % 60);
    return `${m}:${s.toString().padStart(2, '0')}`;
  }
}
