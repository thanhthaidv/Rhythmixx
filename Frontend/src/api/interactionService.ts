import apiClient from './apiClient';
import type { InteractionDto, ApiResponse } from '../types/api';

/**
 * Interactions service for user interactions with media
 * Track likes, views, shares
 */
export const interactionService = {
  /**
   * Record a view/play event for a media item
   * Requires: Authorization (can be anonymous for public items)
   */
  recordView: async (mediaId: string) => {
    const res = await apiClient.post<ApiResponse<InteractionDto>>('/interactions/view', { mediaId });
    return res.data.data;
  },

  /**
   * Like a media item
   * Requires: Authorization
   */
  like: async (mediaId: string) => {
    const res = await apiClient.post<ApiResponse<InteractionDto>>('/interactions/like', { mediaId });
    return res.data.data;
  },

  /**
   * Unlike a media item
   * Requires: Authorization
   */
  unlike: async (mediaId: string) => {
    const res = await apiClient.delete<ApiResponse<void>>(`/interactions/like/${mediaId}`);
    return res.data.success;
  },

  /**
   * Share a media item
   * Requires: Authorization
   */
  share: async (mediaId: string, targetUserId: string) => {
    const res = await apiClient.post<ApiResponse<InteractionDto>>('/interactions/share', {
      mediaId,
      targetUserId,
    });
    return res.data.data;
  },

  /**
   * Get interaction statistics for a media item
   */
  getStats: async (mediaId: string) => {
    const res = await apiClient.get<ApiResponse<any>>(`/interactions/${mediaId}/stats`);
    return res.data.data;
  },

  /**
   * Get user's interactions
   * Requires: Authorization
   */
  getUserInteractions: async (type?: 'like' | 'view' | 'share') => {
    const query = type ? `?type=${type}` : '';
    const res = await apiClient.get<ApiResponse<InteractionDto[]>>(`/interactions/user${query}`);
    return res.data.data;
  },
};
