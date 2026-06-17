import apiClient from './apiClient';
import type { AlbumDetailDto, AlbumDto, ApiResponse, CreateAlbumDto } from '../types/api';

export const albumService = {
  getMyAlbums: async () => {
    const res = await apiClient.get<ApiResponse<AlbumDto[]>>('/albums/my-albums');
    return res.data.data;
  },

  getById: async (albumId: string) => {
    const res = await apiClient.get<ApiResponse<AlbumDetailDto>>(`/albums/${albumId}`);
    return res.data.data;
  },

  create: async (data: CreateAlbumDto) => {
    const res = await apiClient.post<ApiResponse<AlbumDto>>('/albums', data);
    return res.data.data;
  },

  update: async (albumId: string, data: CreateAlbumDto) => {
    const res = await apiClient.put<ApiResponse<AlbumDto>>(`/albums/${albumId}`, data);
    return res.data.data;
  },

  delete: async (albumId: string) => {
    const res = await apiClient.delete<ApiResponse<void>>(`/albums/${albumId}`);
    return res.data.success;
  },
};
