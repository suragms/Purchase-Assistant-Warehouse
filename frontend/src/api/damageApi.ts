// Damage Report API types and functions
export interface CreateDamageReportDto {
  itemName?: string;
  qtyDamaged: number;
  damageType?: 'Damaged' | 'Short' | 'Missing' | 'Returned';
  catalogItemId?: string;
  unit?: string;
  reason?: 'TornBag' | 'WetDamage' | 'WrongItem' | 'ShortWeight' | 'Other';
  photoUrl?: string;
  notes?: string;
  emitNotification?: boolean;
  damagedItemsInBatch?: number;
}

export interface UpdateDamageReportStatusDto {
  status: 'Approved' | 'Returned' | 'Rejected';
  notes?: string;
}

export interface DamageReportDto {
  id: string;
  createdAt: string;
  reportedBy?: string;
  purchaseOrderId: string;
  catalogItemId?: string;
  itemName: string;
  qtyDamaged: number;
  unit?: string;
  damageType: string;
  reason?: string;
  status: string;
  photoUrl?: string;
  notes?: string;
}

export interface PendingDamageReportsCountDto {
  count: number;
}

export const damageApi = {
  getDamageReports: async (purchaseOrderId: string): Promise<DamageReportDto[]> => {
    const response = await fetch(`/api/v1/purchases/${purchaseOrderId}/damage-reports`, {
      headers: { Authorization: `Bearer ${localStorage.getItem('token')}` }
    });
    if (!response.ok) throw new Error(`HTTP ${response.status}`);
    return response.json();
  },

  createDamageReport: async (purchaseOrderId: string, dto: CreateDamageReportDto): Promise<DamageReportDto> => {
    const response = await fetch(`/api/v1/purchases/${purchaseOrderId}/damage-reports`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        Authorization: `Bearer ${localStorage.getItem('token')}`
      },
      body: JSON.stringify(dto)
    });
    if (!response.ok) throw new Error(`HTTP ${response.status}`);
    return response.json();
  },

  updateDamageReportStatus: async (
    purchaseOrderId: string,
    reportId: string,
    dto: UpdateDamageReportStatusDto
  ): Promise<DamageReportDto> => {
    const response = await fetch(`/api/v1/purchases/${purchaseOrderId}/damage-reports/${reportId}`, {
      method: 'PATCH',
      headers: {
        'Content-Type': 'application/json',
        Authorization: `Bearer ${localStorage.getItem('token')}`
      },
      body: JSON.stringify(dto)
    });
    if (!response.ok) throw new Error(`HTTP ${response.status}`);
    return response.json();
  },

  getPendingCount: async (): Promise<PendingDamageReportsCountDto> => {
    const response = await fetch('/api/v1/damage-reports/pending-count', {
      headers: { Authorization: `Bearer ${localStorage.getItem('token')}` }
    });
    if (!response.ok) throw new Error(`HTTP ${response.status}`);
    return response.json();
  }
};