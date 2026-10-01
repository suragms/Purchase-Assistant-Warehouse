import apiClient from './apiClient';

export interface UserDto {
  id: string;
  name: string;
  email: string;
  status: number; // UserStatus values from the backend.
  role: number; // Role values from the backend.
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

// Keep numeric API values explicit; do not infer roles from option order.
export const USER_ROLES = [{ value: 0, label: 'SuperAdmin' }, { value: 1, label: 'Owner' }, { value: 2, label: 'Admin' }, { value: 3, label: 'Manager' }, { value: 4, label: 'Staff' }];
export const USER_STATUSES = [{ value: 0, label: 'Active' }, { value: 1, label: 'Inactive' }, { value: 2, label: 'Blocked' }, { value: 3, label: 'Deleted' }, { value: 4, label: 'Pending verification' }];
