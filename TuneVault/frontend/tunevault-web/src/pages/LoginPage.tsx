import { FormEvent, useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';

export function LoginPage() {
  const { login, register } = useAuth();
  const navigate = useNavigate();
  const [isRegister, setIsRegister] = useState(false);
  const [email, setEmail] = useState('alice@demo.com');
  const [password, setPassword] = useState('Demo@123');
  const [displayName, setDisplayName] = useState('');
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);

  const onSubmit = async (e: FormEvent) => {
    e.preventDefault();
    setError('');
    setLoading(true);
    try {
      if (isRegister) await register(email, password, displayName);
      else await login(email, password);
      navigate('/');
    } catch (err: unknown) {
      setError(err instanceof Error ? err.message : 'Authentication failed');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="min-h-screen flex items-center justify-center bg-gradient-to-b from-purple-900 to-spotify-black p-4">
      <form onSubmit={onSubmit} className="w-full max-w-md bg-spotify-dark p-8 rounded-xl shadow-xl">
        <h1 className="text-3xl font-bold mb-2">TuneVault</h1>
        <p className="text-spotify-light mb-6">{isRegister ? 'Create account' : 'Login to continue'}</p>
        {isRegister && (
          <input
            className="w-full mb-3 p-3 rounded-md bg-spotify-gray outline-none"
            placeholder="Display name"
            value={displayName}
            onChange={(e) => setDisplayName(e.target.value)}
            required
          />
        )}
        <input
          className="w-full mb-3 p-3 rounded-md bg-spotify-gray outline-none"
          type="email"
          placeholder="Email"
          value={email}
          onChange={(e) => setEmail(e.target.value)}
          required
        />
        <input
          className="w-full mb-3 p-3 rounded-md bg-spotify-gray outline-none"
          type="password"
          placeholder="Password"
          value={password}
          onChange={(e) => setPassword(e.target.value)}
          required
        />
        {error && <p className="text-red-400 text-sm mb-3">{error}</p>}
        <button disabled={loading} className="w-full bg-spotify-green text-black font-semibold py-3 rounded-full">
          {loading ? 'Please wait...' : isRegister ? 'Register' : 'Login'}
        </button>
        <button type="button" onClick={() => setIsRegister(!isRegister)} className="w-full mt-3 text-sm text-spotify-light">
          {isRegister ? 'Already have an account? Login' : 'Need an account? Register'}
        </button>
        <p className="text-xs text-spotify-light mt-4">
          Demo: alice@demo.com / bob@demo.com — password <code>Demo@123</code>
        </p>
        <Link to="/" className="block text-center text-xs text-spotify-light mt-2">Browse as guest (limited)</Link>
      </form>
    </div>
  );
}
