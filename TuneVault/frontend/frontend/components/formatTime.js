/* ============================================================
   UTIL: formatTime
   Định dạng số giây -> chuỗi "m:ss". Dùng chung cho PlayerBar.
   ============================================================ */
function formatTime(seconds) {
  const m = Math.floor(seconds / 60);
  const s = Math.floor(seconds % 60);
  return `${m}:${s.toString().padStart(2, '0')}`;
}
