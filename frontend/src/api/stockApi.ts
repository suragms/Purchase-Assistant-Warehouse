import apiClient from './apiClient';
import type { PaginatedResult } from './catalogApi';

export interface StockItem {
  id: string;
  itemCode: string;
  barcode?: string;
  name: string;
  categoryName: string;
  defaultUnit: string;
  systemStock: number;
  physicalStock: number;
  reservedStock: number;
  availableStock: number;
  reorderLevel: number;
  isActive: boolean;
  rowVersion: string;
}

export interface StockMovement {
  id: string;
  catalogItemId: string;
  movementType: string;
  quantityDelta: number;
  quantityBefore: number;
  quantityAfter: number;
  referenceType?: string;
  referenceId?: string;
  reason?: string;
  notes?: string;
  createdById: string;
  createdByName: string;
  createdAt: string;
}

export interface AdjustStockRequest {
  quantityDelta: number;
  reason?: string;
  notes?: string;
  expectedVersion: string;
}

export interface UpdatePhysicalStockRequest {
  physicalStock: number;
  reason?: string;
  notes?: string;
  expectedVersion: string;
}

export interface ReconcileStockRequest {
  reason?: string;
  notes?: string;
  expectedVersion: string;
}

export const stockApi = {
  getItems: async (
    page = 1,
    pageSize = 50,
    search?: string,
    filters?: { categoryId?: string; supplierId?: string; severity?: string }
  ): Promise<PaginatedResult<StockItem>> => {
    const params = new URLSearchParams();
    params.append('page', page.toString());
    params.append('pageSize', pageSize.toString());
    if (search) params.append('search', search);
    for (const [key, value] of Object.entries(filters ?? {})) if (value) params.append(key, value);
    const res = await apiClient.get(`/stock?${params.toString()}`);
    return res.data;
  },

  getLowStock: async (
    page = 1,
    pageSize = 50,
    search?: string,
    filters?: { categoryId?: string; supplierId?: string; severity?: string }
  ): Promise<PaginatedResult<StockItem>> => {
    const params = new URLSearchParams();
    params.append('page', page.toString());
    params.append('pageSize', pageSize.toString());
    if (search) params.append('search', search);
    for (const [key, value] of Object.entries(filters ?? {})) if (value) params.append(key, value);
    const res = await apiClient.get(`/stock/low-stock?${params.toString()}`);
    return res.data;
  },

  getOutOfStock: async (
    page = 1,
    pageSize = 50,
    search?: string,
    filters?: { categoryId?: string; supplierId?: string; severity?: string }
  ): Promise<PaginatedResult<StockItem>> => {
    const params = new URLSearchParams();
    params.append('page', page.toString());
    params.append('pageSize', pageSize.toString());
    if (search) params.append('search', search);
    for (const [key, value] of Object.entries(filters ?? {})) if (value) params.append(key, value);
    const res = await apiClient.get(`/stock/out-of-stock?${params.toString()}`);
    return res.data;
  },

  getDetail: async (id: string): Promise<StockItem> => {
    const res = await apiClient.get(`/stock/${id}`);
    return res.data;
  },

  getActivity: async (
    id: string,
    page = 1,
    pageSize = 50
  ): Promise<PaginatedResult<StockMovement>> => {
    const params = new URLSearchParams();
    params.append('page', page.toString());
    params.append('pageSize', pageSize.toString());
    const res = await apiClient.get(`/stock/${id}/activity?${params.toString()}`);
    return res.data;
  },

  adjustStock: async (id: string, request: AdjustStockRequest): Promise<StockItem> => {
    const res = await apiClient.post(`/stock/${id}/adjust`, request);
    return res.data;
  },

  updatePhysicalStock: async (
    id: string,
    request: UpdatePhysicalStockRequest
  ): Promise<StockItem> => {
    const res = await apiClient.post(`/stock/${id}/physical`, request);
    return res.data;
  },

  reconcileStock: async (id: string, request: ReconcileStockRequest): Promise<StockItem> => {
    const res = await apiClient.post(`/stock/${id}/reconcile`, request);
    return res.data;
  },
};
