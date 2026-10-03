import apiClient from './apiClient';

export const IntentStatus = {
  Success: 'Success',
  AmbiguousMatch: 'AmbiguousMatch',
  MissingInformation: 'MissingInformation',
  Error: 'Error',
} as const;
export type IntentStatus = typeof IntentStatus[keyof typeof IntentStatus];

export interface CatalogItemOptionDto {
  catalogItemId: string;
  name: string;
  description?: string;
}

export interface PurchaseIntentItemCandidateDto {
  catalogItemId?: string;
  itemCode?: string;
  catalogItemName?: string;
  requestedQuantity: number;
  unitOfMeasure?: string;
  isAmbiguous: boolean;
  options?: CatalogItemOptionDto[];
}

export interface PurchaseIntentCandidateDto {
  status: IntentStatus;
  message?: string;
  supplierId?: string;
  supplierName?: string;
  items: PurchaseIntentItemCandidateDto[];
  notes?: string;
  warnings?: string[];
  generatedAt?: string;
}

export const purchaseIntentApi = {
  parseIntent: async (prompt: string): Promise<PurchaseIntentCandidateDto> => {
    const res = await apiClient.post('/ai/purchase-intent/parse', { prompt }, { timeout: 60000 });
    return res.data;
  },
};
