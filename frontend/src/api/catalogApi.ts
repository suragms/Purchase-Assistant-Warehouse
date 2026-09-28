import apiClient from './apiClient';

export interface CatalogItem {
  id: string;
  itemCode: string;
  barcode?: string;
  name: string;
  categoryId: string;
  categoryName: string;
  typeId?: string;
  typeName?: string;
  defaultUnit: string;
  kgPerUnit?: number;
  reorderLevel: number;
  currentStock: number;
  isActive: boolean;
  rowVersion: string;
}

export interface CatalogItemDetail extends CatalogItem {
  lastSupplierId?: string;
  lastSupplierName?: string;
  lastBrokerId?: string;
  lastBrokerName?: string;
}

export interface Category {
  id: string;
  name: string;
  itemCount: number;
}

export interface CategoryType {
  id: string;
  categoryId: string;
  categoryName: string;
  name: string;
  itemCount: number;
}

export interface Supplier {
  id: string;
  name: string;
  phone?: string;
  address?: string;
  notes?: string;
  isActive: boolean;
  linkedItemsCount: number;
}

export interface Broker {
  id: string;
  name: string;
  isActive: boolean;
  linkedSuppliersCount: number;
}

export interface GlobalSearchResponse {
  items: CatalogItem[];
  suppliers: Supplier[];
  brokers: Broker[];
  categories: Category[];
}

export const catalogApi = {
  // Catalog Items
  getItems: async (page = 1, pageSize = 50, search?: string, categoryId?: string) => {
    const params = new URLSearchParams();
    params.append('page', page.toString());
    params.append('pageSize', pageSize.toString());
    if (search) params.append('search', search);
    if (categoryId) params.append('categoryId', categoryId);

    const res = await apiClient.get(`/catalog/items?${params.toString()}`);
    return res.data;
  },

  getItemById: async (id: string): Promise<CatalogItemDetail> => {
    const res = await apiClient.get(`/catalog/items/${id}`);
    return res.data;
  },

  createItem: async (item: Partial<CatalogItem>): Promise<CatalogItem> => {
    const res = await apiClient.post('/catalog/items', item);
    return res.data;
  },

  updateItem: async (id: string, item: Partial<CatalogItem>): Promise<CatalogItem> => {
    const res = await apiClient.put(`/catalog/items/${id}`, item);
    return res.data;
  },

  deleteItem: async (id: string): Promise<void> => {
    await apiClient.delete(`/catalog/items/${id}`);
  },

  // Categories
  getCategories: async (): Promise<Category[]> => {
    const res = await apiClient.get('/catalog/categories');
    return res.data;
  },

  createCategory: async (name: string): Promise<Category> => {
    const res = await apiClient.post(`/catalog/categories?name=${encodeURIComponent(name)}`);
    return res.data;
  },

  updateCategory: async (id: string, name: string): Promise<Category> => {
    const res = await apiClient.put(`/catalog/categories/${id}?name=${encodeURIComponent(name)}`);
    return res.data;
  },

  deleteCategory: async (id: string): Promise<void> => {
    await apiClient.delete(`/catalog/categories/${id}`);
  },

  // Suppliers
  getSuppliers: async (): Promise<Supplier[]> => {
    const res = await apiClient.get('/catalog/suppliers');
    return res.data;
  },

  createSupplier: async (supplier: Partial<Supplier>): Promise<Supplier> => {
    const res = await apiClient.post('/catalog/suppliers', supplier);
    return res.data;
  },

  updateSupplier: async (id: string, supplier: Partial<Supplier>): Promise<Supplier> => {
    const res = await apiClient.put(`/catalog/suppliers/${id}`, supplier);
    return res.data;
  },

  deleteSupplier: async (id: string): Promise<void> => {
    await apiClient.delete(`/catalog/suppliers/${id}`);
  },

  // Brokers
  getBrokers: async (): Promise<Broker[]> => {
    const res = await apiClient.get('/catalog/brokers');
    return res.data;
  },

  createBroker: async (broker: Partial<Broker>): Promise<Broker> => {
    const res = await apiClient.post('/catalog/brokers', broker);
    return res.data;
  },

  updateBroker: async (id: string, broker: Partial<Broker>): Promise<Broker> => {
    const res = await apiClient.put(`/catalog/brokers/${id}`, broker);
    return res.data;
  },

  deleteBroker: async (id: string): Promise<void> => {
    await apiClient.delete(`/catalog/brokers/${id}`);
  },

  // Search
  search: async (query: string): Promise<GlobalSearchResponse> => {
    const res = await apiClient.get(`/catalog/search?q=${encodeURIComponent(query)}`);
    return res.data;
  }
};
