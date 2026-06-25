import { useEffect, useState } from 'react';
import { notificationApi } from '../api/services';
import type { NotificationItem } from '../api/types';
import { useSignalR } from '../hooks/useSignalR';

export function NotificationsPage() {
  const [items, setItems] = useState<NotificationItem[]>([]);
  const { refreshUnread } = useSignalR();

  const load = async () => {
    setItems(await notificationApi.list());
    await refreshUnread();
  };

  useEffect(() => {
    void load();
  }, []);

  const markRead = async (id: string) => {
    await notificationApi.markRead(id);
    await load();
  };

  const markAll = async () => {
    await notificationApi.markAllRead();
    await load();
  };

  return (
    <div>
      <div className="flex items-center justify-between mb-4">
        <h2 className="text-2xl font-bold">Notifications</h2>
        <button onClick={() => void markAll()} className="text-sm text-spotify-green">Mark all read</button>
      </div>
      <div className="space-y-3">
        {items.map((item) => {
          const payload = JSON.parse(item.payloadJson) as { mediaTitle?: string; playlistName?: string; senderId?: string };
          return (
            <div key={item.id} className={`p-4 rounded-lg ${item.isRead ? 'bg-spotify-dark' : 'bg-spotify-gray border border-spotify-green'}`}>
              <p className="font-medium">{item.type === 'MediaShared' ? 'Media shared' : 'Playlist shared'}</p>
              <p className="text-sm text-spotify-light">{payload.mediaTitle || payload.playlistName}</p>
              {!item.isRead && (
                <button onClick={() => void markRead(item.id)} className="text-xs mt-2 text-spotify-green">
                  Mark read
                </button>
              )}
            </div>
          );
        })}
      </div>
    </div>
  );
}
