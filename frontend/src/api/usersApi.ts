import apiClient from './apiClient';

export interface UserDto {
  id: string;
  name: string;
  email: string;
  status: number; // 0: Active, 1: Blocked, etc.
  role: number; // 0: Admin, 1: Manager, 2: Staff, etc.
  createdAt: string;
}

export interface CreateUserDto {
  name: string;
  email: string;
  password: string;
  role: number;
}

export interface UpdateUserDto {
  name: string;
  email: string;
  role: number;
  status: number;
}

export const usersApi = {
  getUsers: async (): Promise<UserDto[]> => {
    const res = await apiClient.get('/users');
    return res.data.data || res.data;
  },

  getUserById: async (id: string): Promise<UserDto> => {
    const res = await apiClient.get(`/users/${id}`);
    return res.data.data || res.data;
  },

  createUser: async (data: CreateUserDto): Promise<UserDto> => {
    const res = await apiClient.post('/users', data);
    return res.data.data || res.data;
  },

  updateUser: async (id: string, data: UpdateUserDto): Promise<void> => {
    await apiClient.put(`/users/${id}`, data);
  },

  blockUser: async (id: string): Promise<void> => {
    await apiClient.post(`/users/${id}/block`);
  },

  deleteUser: async (id: string): Promise<void> => {
    await apiClient.delete(`/users/${id}`);
  },
};
