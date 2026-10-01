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

export interface UserPermissionsDto {
  permissions: string[];
  defaultPermissions: string[];
}

export interface PatchPermissionsDto {
  grant: string[];
  revoke: string[];
}

export const ALL_PERMISSIONS: { key: string; label: string; group: string }[] = [
  { key: 'catalog.view', label: 'View Catalog', group: 'Catalog' },
  { key: 'catalog.create', label: 'Create Items', group: 'Catalog' },
  { key: 'catalog.edit', label: 'Edit Items', group: 'Catalog' },
  { key: 'catalog.archive', label: 'Archive Items', group: 'Catalog' },
  { key: 'supplier.view', label: 'View Suppliers', group: 'Suppliers' },
  { key: 'supplier.create', label: 'Create Suppliers', group: 'Suppliers' },
  { key: 'supplier.edit', label: 'Edit Suppliers', group: 'Suppliers' },
  { key: 'supplier.delete', label: 'Delete Suppliers', group: 'Suppliers' },
  { key: 'broker.view', label: 'View Brokers', group: 'Brokers' },
  { key: 'broker.create', label: 'Create Brokers', group: 'Brokers' },
  { key: 'broker.edit', label: 'Edit Brokers', group: 'Brokers' },
  { key: 'broker.delete', label: 'Delete Brokers', group: 'Brokers' },
  { key: 'purchase.view', label: 'View Purchases', group: 'Purchases' },
  { key: 'purchase.create', label: 'Create Purchases', group: 'Purchases' },
  { key: 'purchase.edit', label: 'Edit Purchases', group: 'Purchases' },
  { key: 'purchase.delete', label: 'Delete Purchases', group: 'Purchases' },
  { key: 'purchase.payment', label: 'Manage Payments', group: 'Purchases' },
  { key: 'purchase.delivery', label: 'Manage Delivery', group: 'Purchases' },
  { key: 'purchase.verify', label: 'Verify Purchases', group: 'Purchases' },
  { key: 'purchase.commit', label: 'Commit Purchases', group: 'Purchases' },
  { key: 'purchase.damage_report', label: 'Report Damage', group: 'Purchases' },
  { key: 'purchase.damage_approve', label: 'Approve Damage', group: 'Purchases' },
  { key: 'stock.view', label: 'View Stock', group: 'Stock' },
  { key: 'stock.adjust', label: 'Adjust Stock', group: 'Stock' },
  { key: 'stock.physical', label: 'Physical Count', group: 'Stock' },
  { key: 'stock.system', label: 'System Adjustments', group: 'Stock' },
  { key: 'reports.view', label: 'View Reports', group: 'Reports' },
  { key: 'users.view', label: 'View Users', group: 'Users' },
  { key: 'users.manage', label: 'Manage Users', group: 'Users' },
  { key: 'roles.manage', label: 'Manage Roles', group: 'Users' },
  { key: 'settings.manage', label: 'Manage Settings', group: 'Settings' },
  { key: 'providers.manage', label: 'Manage Providers', group: 'Settings' },
];

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

  getPermissions: async (id: string): Promise<UserPermissionsDto> => {
    const res = await apiClient.get(`/users/${id}/permissions`);
    return res.data.data || res.data;
  },

  patchPermissions: async (id: string, data: PatchPermissionsDto): Promise<void> => {
    await apiClient.patch(`/users/${id}/permissions`, data);
  },
};

// Keep numeric API values explicit; do not infer roles from option order.
export const USER_ROLES = [{ value: 0, label: 'SuperAdmin' }, { value: 1, label: 'Owner' }, { value: 2, label: 'Admin' }, { value: 3, label: 'Manager' }, { value: 4, label: 'Staff' }];
export const USER_STATUSES = [{ value: 0, label: 'Active' }, { value: 1, label: 'Inactive' }, { value: 2, label: 'Blocked' }, { value: 3, label: 'Deleted' }, { value: 4, label: 'Pending verification' }];
