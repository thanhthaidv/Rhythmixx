import apiClient from './apiClient';
import type { ApiResponse, ArtistDto, MediaItemDto } from '../types/api';

export const artistService = {
  search: async (query: string) => {
    const res = await apiClient.get<ApiResponse<ArtistDto[]>>(
      `/artists/search?q=${encodeURIComponent(query)}`
    );
    return res.data.data;
  },

  getMedia: async (artistId: string) => {
    const res = await apiClient.get<ApiResponse<MediaItemDto[]>>(`/artists/${artistId}/media`);
    return res.data.data;
  },

  uploadCover: async (artistId: string, coverImage: File) => {
    const formData = new FormData();
    formData.append('coverImage', coverImage);

    const res = await apiClient.post<ApiResponse<{ coverImageUrl?: string; CoverImageUrl?: string }>>(
      `/artists/${artistId}/cover`,
      formData,
      { headers: { 'Content-Type': 'multipart/form-data' } }
    );

    return res.data.data?.coverImageUrl ?? res.data.data?.CoverImageUrl ?? '';
  },
};
