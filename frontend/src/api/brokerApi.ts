import apiClient from './apiClient';
import type { PaginatedResult } from './catalogApi';

export interface BrokerDto {
  id: string;
  name: string;
  code?: string;
  contactEmail?: string;
  phone?: string;
  isActive: boolean;
}

export const brokerApi = {
  getBrokers: async (page = 1, pageSize = 50, search?: string): Promise<PaginatedResult<BrokerDto>> => {
    const params = new URLSearchParams();
    params.append('page', page.toString());
    params.append('pageSize', pageSize.toString());
    if (search) params.append('search', search);

    const res = await apiClient.get(`/catalog/brokers?${params.toString()}`);
    return res.data;
  },
};
