import { describe, it, expect, vi } from 'vitest';
import { render, screen } from '@testing-library/react';
import { MemoryRouter } from 'react-router-dom';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import React from 'react';
import StockDashboard from '../pages/stock/StockDashboard';

// Mock the API calls for the dashboard
vi.mock('../api/stockApi', () => ({
  stockApi: {
    getItems: vi.fn().mockResolvedValue({
      data: [],
      meta: { totalCount: 1500, page: 1, pageSize: 1, totalPages: 1500 }
    }),
    getLowStock: vi.fn().mockResolvedValue({
      data: [{ id: '1', name: 'Low Item', itemCode: 'L1', availableStock: 5, reorderLevel: 10, defaultUnit: 'PCS' }],
      meta: { totalCount: 1, page: 1, pageSize: 5, totalPages: 1 }
    }),
    getOutOfStock: vi.fn().mockResolvedValue({
      data: [{ id: '2', name: 'Zero Item', itemCode: 'Z1', availableStock: 0, reorderLevel: 10, defaultUnit: 'PCS' }],
      meta: { totalCount: 2, page: 1, pageSize: 5, totalPages: 1 }
    }),
  }
}));

const queryClient = new QueryClient({
  defaultOptions: {
    queries: {
      retry: false,
    },
  },
});

const wrapper = ({ children }: { children: React.ReactNode }) => (
  <QueryClientProvider client={queryClient}>
    <MemoryRouter>{children}</MemoryRouter>
  </QueryClientProvider>
);

describe('StockDashboard', () => {
  it('renders summary cards with correct API totals', async () => {
    render(<StockDashboard />, { wrapper });

    // Header exists
    expect(screen.getByText('Stock Overview')).toBeInTheDocument();

    // Verify stats from API mock render eventually
    expect(await screen.findByText('1500')).toBeInTheDocument(); // All Items
    expect(await screen.findByText('1')).toBeInTheDocument(); // Low Stock
    expect(await screen.findByText('2')).toBeInTheDocument(); // Out of Stock

    // Verify alert sections render correctly
    expect(screen.getByText('Low Item')).toBeInTheDocument();
    expect(screen.getByText('Zero Item')).toBeInTheDocument();
  });
});
