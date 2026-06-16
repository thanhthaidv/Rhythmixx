import { useState, useEffect } from 'react';
import { playlistService } from '../api';

export const usePlaylists = () => {
  const [data, setData] = useState<any[]>([]);
  const [error, setError] = useState<string | null>(null);
  // THÊM DÒNG NÀY VÀO ĐỂ KHAI BÁO LOADING
  const [loading, setLoading] = useState<boolean>(false); 

  const fetchPlaylists = async () => {
    try {
      setLoading(true); // Bây giờ lỗi này sẽ biến mất
      const response = await playlistService.getAll(); // Giả định tên hàm
      setData(response);
      setLoading(false);
    } catch (err: any) {
      setError(err.message || 'Có lỗi xảy ra');
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchPlaylists();
  }, []);

  return { data, loading, error, refetch: fetchPlaylists };
};