import axios from "axios";

const API_BASE_URL = "http://localhost:5269/api";

export interface PlayHistoryItemDto {
  mediaId: string;
  playedAt: string;
}

const getAuthHeaders = () => {
  const token = localStorage.getItem("token");

  return {
    Authorization: `Bearer ${token}`,
  };
};

export const playHistoryService = {
  async getMyHistories(take = 10): Promise<PlayHistoryItemDto[]> {
    const response = await axios.get(`${API_BASE_URL}/play-histories?take=${take}`, {
      headers: getAuthHeaders(),
    });

    return response.data;
  },

  async add(mediaId: string): Promise<void> {
    await axios.post(
      `${API_BASE_URL}/play-histories`,
      { mediaId },
      {
        headers: getAuthHeaders(),
      }
    );
  },

  async clear(): Promise<void> {
    await axios.delete(`${API_BASE_URL}/play-histories`, {
      headers: getAuthHeaders(),
    });
  },
};