/* ============================================================
   CẤU HÌNH TAILWIND
   Định nghĩa bảng màu theo theme Spotify (đen/xanh) dùng xuyên suốt app.
   ============================================================ */
tailwind.config = {
  theme: {
    extend: {
      colors: {
        spotify: {
          black: '#121212',
          dark: '#181818',
          gray: '#282828',
          green: '#1DB954',
          light: '#b3b3b3',
        },
      },
    },
  },
};
