/* ============================================================
   PAGE: LoginPage
   Trang đăng nhập. Bản demo không có Backend thật nên chấp nhận
   mọi email/mật khẩu.
   ============================================================ */
function LoginPage({ onLogin }) {
  const [email, setEmail] = React.useState('');
  const [password, setPassword] = React.useState('');
  function handleSubmit(e) {
    e.preventDefault();
    onLogin(new User({ id: 'u1', displayName: email.split('@')[0] || 'Bạn', email: email || 'demo@tunevault.app' }));
  }
  return (
    <div className="flex h-screen items-center justify-center bg-spotify-black">
      <form onSubmit={handleSubmit} className="w-full max-w-sm rounded-md bg-spotify-dark p-8">
        <h1 className="mb-2 text-center text-2xl font-bold text-spotify-green">TuneVault</h1>
        <p className="mb-6 text-center text-xs text-spotify-light">Bản demo 1 file — đăng nhập với email/mật khẩu bất kỳ</p>
        <label className="mb-1 block text-sm text-spotify-light">Email</label>
        <input type="email" required value={email} onChange={(e) => setEmail(e.target.value)} className="mb-4 w-full rounded bg-spotify-gray px-3 py-2 text-sm text-white focus:outline-none" />
        <label className="mb-1 block text-sm text-spotify-light">Mật khẩu</label>
        <input type="password" required value={password} onChange={(e) => setPassword(e.target.value)} className="mb-4 w-full rounded bg-spotify-gray px-3 py-2 text-sm text-white focus:outline-none" />
        <button type="submit" className="w-full rounded-full bg-spotify-green px-5 py-2 text-sm font-semibold text-black hover:brightness-110">Đăng nhập</button>
      </form>
    </div>
  );
}
