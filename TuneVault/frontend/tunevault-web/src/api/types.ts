export type MediaType = 'Audio' | 'Video';

export interface ApiResponse<T> {
  success: boolean;
  data: T;
  errors: string[];
}

export interface AuthResponse {
  token: string;
  userId: string;
  email: string;
  displayName: string;
}

export interface UserProfile {
  id: string;
  email: string;
  displayName: string;
  bio?: string;
  avatarUrl?: string;
}

export interface MediaItem {
  id: string;
  title: string;
  artist: string;
  genre: string;
  type: MediaType;
  durationSeconds: number;
  description?: string;
  ownerId: string;
  ownerName: string;
  createdAt: string;
}

export interface Playlist {
  id: string;
  name: string;
  description?: string;
  isPublic: boolean;
  ownerId: string;
  ownerName: string;
  trackCount: number;
  createdAt: string;
}

export interface PlaylistDetail extends Omit<Playlist, 'trackCount'> {
  tracks: MediaItem[];
}

export interface ShareItem {
  id: string;
  senderId: string;
  senderName: string;
  receiverId: string;
  receiverName: string;
  mediaItemId?: string;
  playlistId?: string;
  media?: MediaItem;
  playlistName?: string;
  sharedAt: string;
}

export interface NotificationItem {
  id: string;
  type: 'MediaShared' | 'PlaylistShared';
  payloadJson: string;
  isRead: boolean;
  createdAt: string;
}

export interface SearchResult {
  items: MediaItem[];
  totalCount: number;
  page: number;
  pageSize: number;
}
