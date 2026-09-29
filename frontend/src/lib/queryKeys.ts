// Centralized TanStack Query key factory
export const catalogKeys = {
  all: ['catalog'] as const,
  lists: () => [...catalogKeys.all, 'list'] as const,
  list: (filters: Record<string, unknown>) => [...catalogKeys.lists(), filters] as const,
  detail: (id: string) => [...catalogKeys.all, 'detail', id] as const,
};

export const categoryKeys = {
  all: ['categories'] as const,
  lists: () => [...categoryKeys.all, 'list'] as const,
  list: (filters?: Record<string, unknown>) => [...categoryKeys.lists(), filters ?? {}] as const,
  detail: (id: string) => [...categoryKeys.all, 'detail', id] as const,
};

export const typeKeys = {
  all: () => ['types'] as const,
  byCategory: (categoryId: string) => ['types', 'category', categoryId] as const,
  detail: (id: string) => ['types', id] as const,
};

export const supplierKeys = {
  all: ['suppliers'] as const,
  lists: () => [...supplierKeys.all, 'list'] as const,
  list: (filters?: Record<string, unknown>) => [...supplierKeys.lists(), filters ?? {}] as const,
  detail: (id: string) => [...supplierKeys.all, 'detail', id] as const,
};

export const brokerKeys = {
  all: ['brokers'] as const,
  lists: () => [...brokerKeys.all, 'list'] as const,
  list: (filters?: Record<string, unknown>) => [...brokerKeys.lists(), filters ?? {}] as const,
  detail: (id: string) => [...brokerKeys.all, 'detail', id] as const,
};

export const searchKeys = {
  all: ['search'] as const,
  global: (query: string) => [...searchKeys.all, 'global', query] as const,
};

export const duplicateKeys = {
  all: () => ['duplicates'] as const,
  list: (minSimilarity?: number) => ['duplicates', 'list', minSimilarity ?? 70] as const,
};

export const barcodeKeys = {
  lookup: (barcode: string) => ['barcode', barcode] as const,
};

export const stockKeys = {
  all: ['stock'] as const,
  lists: () => [...stockKeys.all, 'list'] as const,
  list: (filters: Record<string, unknown>) => [...stockKeys.lists(), filters] as const,
  lowStock: (filters?: Record<string, unknown>) => [...stockKeys.all, 'low-stock', filters ?? {}] as const,
  outOfStock: (filters?: Record<string, unknown>) => [...stockKeys.all, 'out-of-stock', filters ?? {}] as const,
  detail: (id: string) => [...stockKeys.all, 'detail', id] as const,
  activity: (id: string, filters?: Record<string, unknown>) => [...stockKeys.all, 'activity', id, filters ?? {}] as const,
};

export const purchaseKeys = {
  all: ['purchases'] as const,
  lists: () => [...purchaseKeys.all, 'list'] as const,
  list: (filters: Record<string, unknown>) => [...purchaseKeys.lists(), filters] as const,
  detail: (id: string) => [...purchaseKeys.all, 'detail', id] as const,
};

export const dashboardKeys = {
  all: ['dashboard'] as const,
  summary: () => [...dashboardKeys.all, 'summary'] as const,
};

export const notificationKeys = {
  all: ['notifications'] as const,
  lists: () => [...notificationKeys.all, 'list'] as const,
  list: (filters?: Record<string, unknown>) => [...notificationKeys.lists(), filters ?? {}] as const,
  unreadCount: () => [...notificationKeys.all, 'unread-count'] as const,
};

export const reportKeys = {
  all: ['reports'] as const,
  spend: (filters?: Record<string, unknown>) => [...reportKeys.all, 'spend', filters ?? {}] as const,
  summary: (filters?: Record<string, unknown>) => [...reportKeys.all, 'summary', filters ?? {}] as const,
  stock: () => [...reportKeys.all, 'stock'] as const,
  comparison: (filters?: Record<string, unknown>) => [...reportKeys.all, 'comparison', filters ?? {}] as const,
};
