import apiClient from './apiClient';
import type { UserProfileDto, ApiResponse } from '../types/api';

/**
 * Follows/Follow service for user relationships
 * Manage following/followers
 */
export const followService = {
  /**
   * Follow a user
   * Requires: Authorization
   */
  follow: async (userId: string) => {
    const res = await apiClient.post<ApiResponse<void>>(`/follows/${userId}`, {});
    return res.data.success;
  },

  /**
   * Unfollow a user
   * Requires: Authorization
   */
  unfollow: async (userId: string) => {
    const res = await apiClient.delete<ApiResponse<void>>(`/follows/${userId}`);
    return res.data.success;
  },

  /**
   * Check if current user is following a specific user
   */
  isFollowing: async (userId: string) => {
    const res = await apiClient.get<ApiResponse<boolean>>(`/follows/${userId}`);
    return res.data.data;
  },

  /**
   * Get list of users that the current user is following
   * Requires: Authorization
   */
  getFollowing: async () => {
    const res = await apiClient.get<ApiResponse<UserProfileDto[]>>('/follows/following');
    return res.data.data;
  },

  /**
   * Get list of followers for a user
   */
  getFollowers: async (userId: string) => {
    const res = await apiClient.get<ApiResponse<UserProfileDto[]>>(`/follows/${userId}/followers`);
    return res.data.data;
  },

  /**
   * Get follower count for a user
   */
  getFollowerCount: async (userId: string) => {
    const res = await apiClient.get<ApiResponse<number>>(`/follows/${userId}/count`);
    return res.data.data;
  },
};
