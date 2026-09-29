import apiClient from './apiClient';
import type { PaginatedResult } from './catalogApi'; // reusing PaginatedResult shape

export interface NotificationDto {
  id: string;
  userId: string;
  type: string;
  title: string;
  message: string;
  isRead: boolean;
  createdAt: string;
  readAt?: string;
  referenceType?: string;
  referenceId?: string;
}

export const notificationApi = {
  getNotifications: async (page = 1, pageSize = 10, onlyUnread = false): Promise<PaginatedResult<NotificationDto>> => {
    const res = await apiClient.get('/notifications', {
      params: { page, pageSize, onlyUnread },
    });
    return res.data;
  },

  getUnreadCount: async (): Promise<number> => {
    const res = await apiClient.get('/notifications/unread-count');
    return res.data.count;
  },

  markAsRead: async (id: string): Promise<void> => {
    await apiClient.post(`/notifications/${id}/read`);
  },

  markAllAsRead: async (): Promise<void> => {
    await apiClient.post('/notifications/read-all');
  },
};
