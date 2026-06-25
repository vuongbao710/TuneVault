import axios from 'axios';

const API_URL = import.meta.env.VITE_API_URL || 'http://localhost:5080';

export const api = axios.create({
  baseURL: API_URL,
});

api.interceptors.request.use((config) => {
  const token = localStorage.getItem('token');
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

api.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      localStorage.removeItem('token');
      localStorage.removeItem('user');
      if (window.location.pathname !== '/login') {
        window.location.href = '/login';
      }
    }
    return Promise.reject(error);
  }
);

export const getStreamUrl = (mediaId: string) => {
  const token = localStorage.getItem('token');
  const base = `${API_URL}/api/media/${mediaId}/stream`;
  return token ? `${base}?access_token=${token}` : base;
};

export const HUB_URL = `${API_URL}/hubs/notifications`;
