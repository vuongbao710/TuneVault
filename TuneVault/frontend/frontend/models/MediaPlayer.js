
class MediaPlayer {
  constructor() {
    this.element = document.createElement('video'); // 1 thẻ <video> dùng cho cả audio & video
    this.element.playsInline = true;
    this.listeners = [];
    this.state = {
      currentSong: null,
      queue: [],
      isPlaying: false,
      currentTime: 0,
      volume: 0.8,
    };
    this.element.volume = this.state.volume;

    this.element.addEventListener('timeupdate', () => {
      this._setState({ currentTime: this.element.currentTime });
    });
    this.element.addEventListener('ended', () => this.next());
  }

  subscribe(listener) {
    this.listeners.push(listener);
    listener(this.state);
    return () => {
      this.listeners = this.listeners.filter((l) => l !== listener);
    };
  }

  _setState(partial) {
    this.state = { ...this.state, ...partial };
    this.listeners.forEach((l) => l(this.state));
  }

  playQueue(queue, startIndex = 0) {
    const song = queue[startIndex];
    if (!song) return;
    this._setState({ queue });
    this._loadAndPlay(song);
  }

  _loadAndPlay(song) {
    this.element.pause();
    this.element.src = song.mediaUrl;
    this.element.currentTime = 0;
    this.element.play().catch(() => {});
    this._setState({ currentSong: song, isPlaying: true, currentTime: 0 });
  }

  togglePlayPause() {
    if (this.state.isPlaying) {
      this.element.pause();
      this._setState({ isPlaying: false });
    } else {
      this.element.play().catch(() => {});
      this._setState({ isPlaying: true });
    }
  }

  next() {
    const { queue, currentSong } = this.state;
    if (!currentSong) return;
    const i = queue.findIndex((s) => s.id === currentSong.id);
    const nextSong = queue[i + 1];
    if (nextSong) this._loadAndPlay(nextSong);
    else this._setState({ isPlaying: false });
  }

  previous() {
    const { queue, currentSong } = this.state;
    if (!currentSong) return;
    const i = queue.findIndex((s) => s.id === currentSong.id);
    const prevSong = queue[i - 1];
    if (prevSong) this._loadAndPlay(prevSong);
  }

  seekTo(seconds) {
    this.element.currentTime = seconds;
    this._setState({ currentTime: seconds });
  }

  setVolume(volume) {
    const clamped = Math.max(0, Math.min(1, volume));
    this.element.volume = clamped;
    this._setState({ volume: clamped });
  }

  getElement() {
    return this.element;
  }
}

const mediaPlayer = new MediaPlayer(); // singleton dùng chung toàn app
