import { act, fireEvent, render, screen } from '@testing-library/react';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { beforeEach, expect, it, vi } from 'vitest';
import CatalogVariants from '../pages/catalog/CatalogVariants';
import { catalogApi, type CatalogVariant } from '../api/catalogApi';
import { useAuthStore } from '../stores/authStore';
import { catalogKeys } from '../lib/queryKeys';

vi.mock('../api/catalogApi', () => ({ catalogApi: { createVariant: vi.fn(), updateVariant: vi.fn(), deleteVariant: vi.fn() } }));
const bag: CatalogVariant = { id: 'v1', name: 'Bag', kgPerUnit: 25, rowVersion: 'version1', isActive: true };
const renderVariants = (variants: CatalogVariant[] = []) => {
  const client = new QueryClient({ defaultOptions: { queries: { retry: false }, mutations: { retry: false } } });
  const invalidate = vi.spyOn(client, 'invalidateQueries');
  render(<QueryClientProvider client={client}><CatalogVariants itemId="c1" variants={variants} /></QueryClientProvider>);
  return invalidate;
};
beforeEach(() => {
  vi.resetAllMocks();
  useAuthStore.setState({ user: { id: 'u1', currentBusiness: { businessId: 'b1', role: 'Owner', permissions: [] } } as never });
  vi.mocked(catalogApi.createVariant).mockResolvedValue(bag);
  vi.mocked(catalogApi.updateVariant).mockResolvedValue({ ...bag, rowVersion: 'version2' });
  vi.mocked(catalogApi.deleteVariant).mockResolvedValue();
});
it('creates a weighted variant and refreshes the existing detail query', async () => {
  const invalidate = renderVariants(); expect(screen.getByText('No variants yet.')).toBeInTheDocument();
  fireEvent.click(screen.getByRole('button', { name: 'Add Variant' }));
  fireEvent.change(screen.getByLabelText('Variant name'), { target: { value: ' Bag ' } });
  fireEvent.change(screen.getByLabelText('Variant kg per unit'), { target: { value: '25' } });
  fireEvent.click(screen.getByRole('button', { name: 'Save Variant' }));
  await screen.findByRole('button', { name: 'Add Variant' });
  expect(catalogApi.createVariant).toHaveBeenCalledWith('c1', { name: 'Bag', kgPerUnit: 25 });
  expect(invalidate).toHaveBeenCalledWith({ queryKey: catalogKeys.detail('c1') });
});
it('updates using the loaded version', async () => {
  renderVariants([bag]); fireEvent.click(screen.getByRole('button', { name: 'Edit variant Bag' }));
  fireEvent.change(screen.getByLabelText('Variant name'), { target: { value: 'Box' } });
  fireEvent.click(screen.getByRole('button', { name: 'Save Variant' }));
  await screen.findByRole('button', { name: 'Add Variant' });
  expect(catalogApi.updateVariant).toHaveBeenCalledWith('c1', { ...bag, name: 'Box' });
});
it('requires explicit confirmation before deletion and passes the version', async () => {
  renderVariants([bag]); fireEvent.click(screen.getByRole('button', { name: 'Delete variant Bag' }));
  expect(catalogApi.deleteVariant).not.toHaveBeenCalled();
  fireEvent.click(screen.getByRole('button', { name: 'Delete Variant' }));
  await act(async () => {}); expect(catalogApi.deleteVariant).toHaveBeenCalledWith('c1', bag);
});
it('shows safe backend errors and preserves the unsaved name', async () => {
  vi.mocked(catalogApi.createVariant).mockRejectedValue({ normalized: 'A variant with this name already exists for this item.' });
  renderVariants(); fireEvent.click(screen.getByRole('button', { name: 'Add Variant' }));
  fireEvent.change(screen.getByLabelText('Variant name'), { target: { value: 'Bag' } });
  fireEvent.click(screen.getByRole('button', { name: 'Save Variant' }));
  expect(await screen.findByRole('alert')).toHaveTextContent('already exists');
  expect(screen.getByLabelText('Variant name')).toHaveValue('Bag');
});
it('guards repeated submission while a network request is pending', async () => {
  let resolve!: (v: CatalogVariant) => void;
  vi.mocked(catalogApi.createVariant).mockReturnValue(new Promise(r => { resolve = r; }));
  renderVariants(); fireEvent.click(screen.getByRole('button', { name: 'Add Variant' }));
  fireEvent.change(screen.getByLabelText('Variant name'), { target: { value: 'Bag' } });
  const form = screen.getByRole('button', { name: 'Save Variant' }).closest('form')!;
  fireEvent.submit(form); fireEvent.submit(form);
  await act(async () => {}); expect(catalogApi.createVariant).toHaveBeenCalledTimes(1);
  await act(async () => resolve(bag));
});
it('respects separate edit permissions and owner-only deletion in the UI', () => {
  useAuthStore.setState({ user: { currentBusiness: { role: 'Manager', permissions: ['catalog.edit'] } } as never });
  renderVariants([bag]); expect(screen.getByRole('button', { name: 'Edit variant Bag' })).toBeInTheDocument();
  expect(screen.queryByRole('button', { name: 'Add Variant' })).not.toBeInTheDocument();
  expect(screen.queryByRole('button', { name: 'Delete variant Bag' })).not.toBeInTheDocument();
});
