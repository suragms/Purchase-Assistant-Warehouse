import apiClient from './apiClient';
import type { PaginatedResult } from './catalogApi';

export const PurchaseStatus = {
  Draft: 0,
  Confirmed: 1,
  Dispatched: 2,
  Arrived: 3,
  Verified: 4,
  Completed: 5,
  Cancelled: 6,
} as const;
export type PurchaseStatus = typeof PurchaseStatus[keyof typeof PurchaseStatus];

export const PaymentState = {
  Pending: 0,
  Partial: 1,
  Paid: 2,
  Overdue: 3,
  DueSoon: 4,
} as const;
export type PaymentState = typeof PaymentState[keyof typeof PaymentState];

export const DeliveryState = {
  Pending: 0,
  Partial: 1,
  Delivered: 2,
} as const;
export type DeliveryState = typeof DeliveryState[keyof typeof DeliveryState];

export interface PurchaseItemDto {
  id: string;
  purchaseOrderId: string;
  catalogItemId: string;
  itemCode: string;
  catalogItemName: string;
  orderedQuantity: number;
  receivedQuantity: number;
  unitPrice: number;
  discountPercent?: number;
  taxPercent?: number;
  kgPerUnit?: number;
  landingCostPerKg?: number;
  lineTotal: number;
  notes?: string;
}

export interface PurchaseOrderDto {
  version: number;
  id: string;
  orderNumber: string;
  supplierId: string;
  supplierName: string;
  brokerId?: string;
  brokerName?: string;
  status: PurchaseStatus;
  paymentState: PaymentState;
  deliveryState: DeliveryState;
  notes?: string;
  subtotal: number;
  taxTotal: number;
  grandTotal: number;
  paidAmount?: number;
  remainingAmount?: number;
  paidAt?: string;
  paymentDays?: number;
  dueDate?: string;
  confirmedAt?: string;
  dispatchedAt?: string;
  arrivedAt?: string;
  verifiedAt?: string;
  verifiedById?: string;
  completedAt?: string;
  createdAt: string;
  items: PurchaseItemDto[];
}

export interface UpsertPurchaseItemDto {
  catalogItemId: string;
  orderedQuantity: number;
  unitPrice: number;
  discountPercent?: number;
  taxPercent?: number;
  kgPerUnit?: number;
  landingCostPerKg?: number;
  notes?: string;
}

export interface PurchasePreviewDto {
  subtotal: number;
  taxTotal: number;
  grandTotal: number;
  items: PurchaseItemDto[];
  previewToken: string;
}

export interface PurchaseActivityDto {
  id: string;
  eventType: string;
  userId?: string;
  createdAt: string;
  detailsJson?: string;
}

export interface UpsertPurchaseOrderDto {
  paymentDays?: number;
  previewToken?: string;
  expectedVersion?: number;
  orderNumber?: string;
  supplierId: string;
  brokerId?: string;
  notes?: string;
  items: UpsertPurchaseItemDto[];
}

export interface ReceivePurchaseItemDto {
  purchaseItemId: string;
  receivedQuantityDelta: number;
  notes?: string;
}

export interface ReceivePurchaseDto {
  expectedVersion?: number;
  items: ReceivePurchaseItemDto[];
}

export const purchaseApi = {
  getActivity: async (id: string): Promise<PurchaseActivityDto[]> => {
    const res = await apiClient.get(`/purchases/${id}/activity`);
    return res.data;
  },
  previewPurchase: async (dto: UpsertPurchaseOrderDto): Promise<PurchasePreviewDto> => {
    const res = await apiClient.post('/purchases/preview', dto);
    return res.data;
  },
  getPurchases: async (
    page = 1,
    pageSize = 50,
    search?: string,
    status?: PurchaseStatus,
    supplierId?: string
  ): Promise<PaginatedResult<PurchaseOrderDto>> => {
    const params = new URLSearchParams();
    params.append('page', page.toString());
    params.append('pageSize', pageSize.toString());
    if (search) params.append('search', search);
    if (status !== undefined) params.append('status', status.toString());
    if (supplierId) params.append('supplierId', supplierId);

    const res = await apiClient.get(`/purchases?${params.toString()}`);
    return res.data;
  },

  getPurchaseById: async (id: string): Promise<PurchaseOrderDto> => {
    const res = await apiClient.get(`/purchases/${id}`);
    return res.data;
  },

  createPurchase: async (dto: UpsertPurchaseOrderDto): Promise<PurchaseOrderDto> => {
    const res = await apiClient.post('/purchases', dto);
    return res.data;
  },

  updatePurchase: async (id: string, dto: UpsertPurchaseOrderDto): Promise<PurchaseOrderDto> => {
    const res = await apiClient.put(`/purchases/${id}`, dto);
    return res.data;
  },

  deletePurchase: async (id: string): Promise<void> => {
    await apiClient.delete(`/purchases/${id}`);
  },

  updateStatus: async (id: string, status: PurchaseStatus, expectedVersion: number): Promise<PurchaseOrderDto> => {
    const res = await apiClient.post(`/purchases/${id}/status`, { status, expectedVersion });
    return res.data;
  },

  receiveItems: async (id: string, dto: ReceivePurchaseDto): Promise<PurchaseOrderDto> => {
    const res = await apiClient.post(`/purchases/${id}/receive`, dto);
    return res.data;
  },
  updatePayment: async (id: string, paidAmount: number, expectedVersion: number): Promise<PurchaseOrderDto> => {
    const res = await apiClient.patch(`/purchases/${id}/payment`, { paidAmount, expectedVersion });
    return res.data;
  },
};
