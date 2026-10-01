import apiClient from './apiClient';
import type { PaginatedResult } from './catalogApi';

export interface SupplierDto {
  id: string;
  name: string;
  code?: string;
  contactEmail?: string;
  phone?: string;
  address?: string;
  isActive: boolean;
}

export const supplierApi = {
  getSuppliers: async (page = 1, pageSize = 50, search?: string): Promise<PaginatedResult<SupplierDto>> => {
    const params = new URLSearchParams();
    params.append('page', page.toString());
    params.append('pageSize', pageSize.toString());
    if (search) params.append('search', search);

    const res = await apiClient.get(`/catalog/suppliers?${params.toString()}`);
    // These existing endpoints return an unpaginated array.
    if (Array.isArray(res.data)) {
      const data = search ? res.data.filter(item => item.name.toLowerCase().includes(search.toLowerCase())) : res.data;
      return { data, meta: { page: 1, pageSize: data.length, totalCount: data.length, totalPages: 1 } };
    }
    return res.data;
  },
};
