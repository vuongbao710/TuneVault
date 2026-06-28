/* ============================================================
   MOCK DATA: Playlist mẫu + Thông báo mẫu
   Yêu cầu: models/Playlist.js và data/mockSongs.js phải được
   load TRƯỚC file này.
   ============================================================ */
function buildPlaylists() {
  return [
    { id: 'p1', name: 'Top Hits Việt', description: 'Những bài nghe nhiều nhất', songs: [mockSongsPool[0], mockSongsPool[1], mockSongsPool[2]] },
    { id: 'p2', name: 'Lofi Thư Giãn', description: 'Nhạc nhẹ cho buổi tối', songs: [mockSongsPool[3], mockSongsPool[4]] },
    { id: 'p3', name: 'MV Nổi Bật', description: 'Video ca nhạc', songs: [mockSongsPool[5]] },
    { id: 'p4', name: 'Mới Phát Hành', description: 'Cập nhật mỗi tuần', songs: [mockSongsPool[0], mockSongsPool[3], mockSongsPool[5]] },
  ].map((p) => new Playlist(p));
}

const mockNotificationsSeed = [
  { id: 'n1', message: 'Hà Anh Tuấn đã chia sẻ playlist "Lofi Thư Giãn" với bạn', isRead: false },
  { id: 'n2', message: 'Bài hát "Mưa Sài Gòn" vừa được thêm vào Top Hits Việt', isRead: true },
];
