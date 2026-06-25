import { NavLink } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';

const links = [
  { to: '/', label: 'Home' },
  { to: '/search', label: 'Search' },
  { to: '/library', label: 'Library' },
  { to: '/shares', label: 'Shares' },
  { to: '/notifications', label: 'Notifications' },
  { to: '/profile', label: 'Profile' },
];

export function Sidebar({ unreadCount }: { unreadCount: number }) {
  const { user, logout } = useAuth();

  return (
    <aside className="w-64 bg-black p-6 flex flex-col gap-6 min-h-full">
      <div>
        <h1 className="text-2xl font-bold text-spotify-green">TuneVault</h1>
        <p className="text-sm text-spotify-light mt-1">{user?.displayName}</p>
      </div>
      <nav className="flex flex-col gap-2">
        {links.map((link) => (
          <NavLink
            key={link.to}
            to={link.to}
            className={({ isActive }) =>
              `rounded-md px-3 py-2 text-sm font-medium hover:bg-spotify-gray ${isActive ? 'bg-spotify-gray text-white' : 'text-spotify-light'}`
            }
          >
            <span className="flex items-center justify-between">
              {link.label}
              {link.to === '/notifications' && unreadCount > 0 && (
                <span className="bg-spotify-green text-black text-xs rounded-full px-2 py-0.5">{unreadCount}</span>
              )}
            </span>
          </NavLink>
        ))}
      </nav>
      <button onClick={logout} className="mt-auto text-left text-sm text-spotify-light hover:text-white">
        Logout
      </button>
    </aside>
  );
}
