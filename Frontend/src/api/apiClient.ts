import axios from 'axios';

const apiClient = axios.create({
  baseURL: 'http://localhost:5269/api',
  headers: { 'Content-Type': 'application/json' },
});

apiClient.interceptors.request.use((config) => {
  const token = localStorage.getItem('token');
  if (token) config.headers.Authorization = `Bearer ${token}`;
  return config;
});

apiClient.interceptors.response.use(
  (res) => res,
  (err) => {
    const status = err.response?.status;
    const reqUrl = err.config?.url || '';
    const isAuthEndpoint = reqUrl.includes('/auth/login') || reqUrl.includes('/auth/register');

    // For auth endpoints (login/register) we want the calling code to handle 401s
    // so the UI can show inline messages. Only perform a global redirect for
    // 401 responses from other endpoints.
    if (status === 401 && !isAuthEndpoint) {
        localStorage.removeItem('token');
        window.location.href = '/login';
    }
    return Promise.reject(err);
  }
);

export default apiClient;
