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
  all: ['types'] as const,
  lists: () => [...typeKeys.all, 'list'] as const,
  list: (filters: Record<string, unknown>) => [...typeKeys.lists(), filters] as const,
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
