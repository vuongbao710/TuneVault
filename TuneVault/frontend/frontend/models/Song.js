
class Song {
  constructor(data) {
    this.id = data.id;
    this.title = data.title;
    this.artist = data.artist;
    this.album = data.album || '';
    this.durationInSeconds = data.durationInSeconds;
    this.mediaUrl = data.mediaUrl;
    this.mediaType = data.mediaType; // 'audio' | 'video'
  }

  getFormattedDuration() {
    const m = Math.floor(this.durationInSeconds / 60);
    const s = Math.floor(this.durationInSeconds % 60);
    return `${m}:${s.toString().padStart(2, '0')}`;
  }

  isVideo() {
    return this.mediaType === 'video';
  }
}
