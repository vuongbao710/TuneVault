import { createContext, useContext, useMemo, useRef, useState, type ReactNode } from 'react';
import { getStreamUrl } from '../api/client';
import { mediaApi } from '../api/services';
import type { MediaItem } from '../api/types';

interface PlayerContextValue {
  current: MediaItem | null;
  isPlaying: boolean;
  play: (item: MediaItem) => Promise<void>;
  toggle: () => void;
  pause: () => void;
  audioRef: React.RefObject<HTMLAudioElement>;
}

const PlayerContext = createContext<PlayerContextValue | undefined>(undefined);

export function PlayerProvider({ children }: { children: ReactNode }) {
  const [current, setCurrent] = useState<MediaItem | null>(null);
  const [isPlaying, setIsPlaying] = useState(false);
  const audioRef = useRef<HTMLAudioElement>(null);

  const value = useMemo<PlayerContextValue>(() => ({
    current,
    isPlaying,
    audioRef,
    play: async (item) => {
      setCurrent(item);
      if (item.type === 'Video') return;
      setTimeout(async () => {
        if (!audioRef.current) return;
        audioRef.current.src = getStreamUrl(item.id);
        try {
          await mediaApi.recordHistory(item.id);
        } catch {
          /* ignore when not logged in */
        }
        await audioRef.current.play();
        setIsPlaying(true);
      }, 0);
    },
    toggle: () => {
      if (!audioRef.current) return;
      if (audioRef.current.paused) {
        void audioRef.current.play();
        setIsPlaying(true);
      } else {
        audioRef.current.pause();
        setIsPlaying(false);
      }
    },
    pause: () => {
      audioRef.current?.pause();
      setIsPlaying(false);
    },
  }), [current, isPlaying]);

  return <PlayerContext.Provider value={value}>{children}</PlayerContext.Provider>;
}

export function usePlayer() {
  const ctx = useContext(PlayerContext);
  if (!ctx) throw new Error('usePlayer must be used within PlayerProvider');
  return ctx;
}
