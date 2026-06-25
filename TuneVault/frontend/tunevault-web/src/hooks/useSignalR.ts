import { HubConnectionBuilder, LogLevel } from '@microsoft/signalr';
import { useCallback, useEffect, useState } from 'react';
import { HUB_URL } from '../api/client';
import { notificationApi } from '../api/services';
import type { NotificationItem } from '../api/types';
import { useAuth } from '../context/AuthContext';

export function useSignalR(onNotification?: (item: NotificationItem) => void) {
  const { isAuthenticated } = useAuth();
  const [unreadCount, setUnreadCount] = useState(0);

  const refreshUnread = useCallback(async () => {
    if (!isAuthenticated) {
      setUnreadCount(0);
      return;
    }
    try {
      const items = await notificationApi.list();
      setUnreadCount(items.filter((n) => !n.isRead).length);
    } catch {
      setUnreadCount(0);
    }
  }, [isAuthenticated]);

  useEffect(() => {
    void refreshUnread();
  }, [refreshUnread]);

  useEffect(() => {
    if (!isAuthenticated) return;

    const token = localStorage.getItem('token');
    const connection = new HubConnectionBuilder()
      .withUrl(HUB_URL, { accessTokenFactory: () => token || '' })
      .withAutomaticReconnect()
      .configureLogging(LogLevel.Information)
      .build();

    connection.on('ReceiveNotification', (notification: NotificationItem) => {
      setUnreadCount((c) => c + 1);
      onNotification?.(notification);
    });

    void connection.start().catch(console.error);

    return () => {
      void connection.stop();
    };
  }, [isAuthenticated, onNotification]);

  return { unreadCount, refreshUnread };
}
