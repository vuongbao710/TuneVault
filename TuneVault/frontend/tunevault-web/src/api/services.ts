import { api } from './client';
import type {
  ApiResponse,
  AuthResponse,
  MediaItem,
  NotificationItem,
  Playlist,
  PlaylistDetail,
  SearchResult,
  ShareItem,
  UserProfile,
} from './types';

export const authApi = {
  login: async (email: string, password: string) => {
    const { data } = await api.post<ApiResponse<AuthResponse>>('/api/auth/login', { email, password });
    return data.data;
  },
  register: async (email: string, password: string, displayName: string) => {
    const { data } = await api.post<ApiResponse<AuthResponse>>('/api/auth/register', { email, password, displayName });
    return data.data;
  },
};

export const mediaApi = {
  list: async () => (await api.get<ApiResponse<MediaItem[]>>('/api/media')).data.data,
  get: async (id: string) => (await api.get<ApiResponse<MediaItem>>(`/api/media/${id}`)).data.data,
  upload: async (form: FormData) => (await api.post<ApiResponse<MediaItem>>('/api/media/upload', form)).data.data,
  toggleFavorite: async (id: string) => (await api.post<ApiResponse<{ isFavorite: boolean }>>(`/api/favorites/${id}/toggle`)).data.data,
  recordHistory: async (id: string) => api.post(`/api/history/${id}`),
};

export const playlistApi = {
  list: async () => (await api.get<ApiResponse<Playlist[]>>('/api/playlists')).data.data,
  get: async (id: string) => (await api.get<ApiResponse<PlaylistDetail>>(`/api/playlists/${id}`)).data.data,
  create: async (payload: { name: string; description?: string; isPublic: boolean }) =>
    (await api.post<ApiResponse<Playlist>>('/api/playlists', payload)).data.data,
};

export const searchApi = {
  search: async (q: string, page = 1) =>
    (await api.get<ApiResponse<SearchResult>>('/api/search', { params: { q, page } })).data.data,
};

export const shareApi = {
  share: async (payload: { receiverUserId: string; mediaItemId?: string; playlistId?: string }) =>
    (await api.post<ApiResponse<ShareItem>>('/api/shares', payload)).data.data,
  withMe: async () => (await api.get<ApiResponse<ShareItem[]>>('/api/shares/with-me')).data.data,
  byMe: async () => (await api.get<ApiResponse<ShareItem[]>>('/api/shares/by-me')).data.data,
};

export const notificationApi = {
  list: async () => (await api.get<ApiResponse<NotificationItem[]>>('/api/notifications')).data.data,
  markRead: async (id: string) => api.put(`/api/notifications/${id}/read`),
  markAllRead: async () => api.put('/api/notifications/read-all'),
};

export const profileApi = {
  get: async () => (await api.get<ApiResponse<UserProfile>>('/api/profile')).data.data,
  update: async (payload: { displayName: string; bio?: string; avatarUrl?: string }) =>
    (await api.put<ApiResponse<UserProfile>>('/api/profile', payload)).data.data,
};

export const favoritesApi = {
  list: async () => (await api.get<ApiResponse<MediaItem[]>>('/api/favorites')).data.data,
};

export const historyApi = {
  list: async () => (await api.get<ApiResponse<MediaItem[]>>('/api/history')).data.data,
};
