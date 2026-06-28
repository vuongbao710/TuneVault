/* ============================================================
   MOCK DATA: Danh sách bài hát mẫu
   Đứng thay cho việc gọi Backend thật (ASP.NET Core).
   Audio mẫu lấy từ SoundHelix (file test công khai, ai cũng dùng được).
   Video mẫu lấy từ thư viện video CC0 chính thức của MDN (Mozilla).
   ============================================================ */
const mockSongsPool = [
  { id: 's1', title: 'Hoàng Hôn Phố Cổ', artist: 'Mai Lan', album: 'Lofi Việt', durationInSeconds: 222, mediaType: 'audio', mediaUrl: 'https://www.soundhelix.com/examples/mp3/SoundHelix-Song-1.mp3' },
  { id: 's2', title: 'Sóng Vỗ Bờ Xa', artist: 'Tuấn Ngọc', album: 'Biển Nhớ', durationInSeconds: 257, mediaType: 'audio', mediaUrl: 'https://www.soundhelix.com/examples/mp3/SoundHelix-Song-2.mp3' },
  { id: 's3', title: 'Mưa Sài Gòn', artist: 'Như Quỳnh', album: 'Mưa', durationInSeconds: 199, mediaType: 'audio', mediaUrl: 'https://www.soundhelix.com/examples/mp3/SoundHelix-Song-3.mp3' },
  { id: 's4', title: 'Gió Heo May', artist: 'Hà Anh Tuấn', album: 'Thu Hà Nội', durationInSeconds: 241, mediaType: 'audio', mediaUrl: 'https://www.soundhelix.com/examples/mp3/SoundHelix-Song-4.mp3' },
  { id: 's5', title: 'Đêm Phố Cổ', artist: 'Vũ.', album: 'Đêm', durationInSeconds: 215, mediaType: 'audio', mediaUrl: 'https://www.soundhelix.com/examples/mp3/SoundHelix-Song-5.mp3' },
  { id: 'v1', title: 'MV: Cánh Hoa Rơi (demo video)', artist: 'TuneVault Team', album: '', durationInSeconds: 10, mediaType: 'video', mediaUrl: 'https://interactive-examples.mdn.mozilla.net/media/cc0-videos/flower.mp4' },
];
