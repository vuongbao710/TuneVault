import { Outlet } from 'react-router-dom';
import { useSignalR } from '../hooks/useSignalR';
import { PlayerBar } from './PlayerBar';
import { Sidebar } from './Sidebar';

export function AppLayout() {
  const { unreadCount } = useSignalR();

  return (
    <div className="min-h-screen bg-spotify-black flex pb-24">
      <Sidebar unreadCount={unreadCount} />
      <main className="flex-1 p-6 overflow-y-auto">
        <Outlet />
      </main>
      <PlayerBar />
    </div>
  );
}
