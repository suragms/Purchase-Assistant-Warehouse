import apiClient from './apiClient';

export interface SpendAnalyticsDto {
  periodLabel: string;
  totalSpend: number;
  purchaseCount: number;
}

export interface SummaryItemDto {
  key: string;
  totalSpend: number;
  count: number;
}

export interface PurchaseSummaryReportDto {
  bySupplier: SummaryItemDto[];
  byCategory: SummaryItemDto[];
  byStatus: SummaryItemDto[];
}

export interface StockAnalyticsDto {
  totalCatalogItems: number;
  lowStockCount: number;
  outOfStockCount: number;
  estimatedInventoryValue: number;
  unpricedStockItemCount: number;
  totalMovementsCount: number;
}

export interface PeriodComparisonDto {
  currentStartDate: string;
  currentEndDate: string;
  previousStartDate: string;
  previousEndDate: string;
  currentPeriodSpend: number;
  previousPeriodSpend: number;
  spendChangePercentage: number;
  currentPeriodOrders: number;
  previousPeriodOrders: number;
  ordersChangePercentage: number;
  currentPeriodAvgOrderValue: number;
  previousPeriodAvgOrderValue: number;
  avgOrderValueChangePercentage: number;
}

export const reportApi = {
  getSpendAnalytics: async (startDate?: string, endDate?: string, groupBy = 'day'): Promise<SpendAnalyticsDto[]> => {
    const params = new URLSearchParams();
    if (startDate) params.append('startDate', startDate);
    if (endDate) params.append('endDate', endDate);
    if (groupBy) params.append('groupBy', groupBy);
    const res = await apiClient.get(`/reports/spend?${params.toString()}`);
    return res.data;
  },

  getPurchaseSummary: async (startDate?: string, endDate?: string): Promise<PurchaseSummaryReportDto> => {
    const params = new URLSearchParams();
    if (startDate) params.append('startDate', startDate);
    if (endDate) params.append('endDate', endDate);
    const res = await apiClient.get(`/reports/purchases-summary?${params.toString()}`);
    return res.data;
  },

  getStockAnalytics: async (): Promise<StockAnalyticsDto> => {
    const res = await apiClient.get('/reports/stock-analytics');
    return res.data;
  },

  getPeriodComparison: async (startDate?: string, endDate?: string): Promise<PeriodComparisonDto> => {
    const params = new URLSearchParams();
    if (startDate) params.append('startDate', startDate);
    if (endDate) params.append('endDate', endDate);
    const res = await apiClient.get(`/reports/comparison?${params.toString()}`);
    return res.data;
  },
};
