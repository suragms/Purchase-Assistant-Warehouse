import { act, fireEvent, render, screen } from '@testing-library/react';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { beforeEach, expect, it, vi } from 'vitest';
import PurchasePayment from '../pages/purchases/PurchasePayment';
import { purchaseApi, type PurchaseOrderDto } from '../api/purchaseApi';
import { useAuthStore } from '../stores/authStore';
import { purchaseKeys } from '../lib/queryKeys';

vi.mock('../api/purchaseApi', async original => ({ ...await original<typeof import('../api/purchaseApi')>(), purchaseApi: { updatePayment: vi.fn() } }));
const order = { id: 'p1', version: 7, status: 1, paymentState: 1, paidAmount: 10, remainingAmount: 777, grandTotal: 100, items: [] } as unknown as PurchaseOrderDto;
const renderPayment = (input = order) => {
  const client = new QueryClient({ defaultOptions: { queries: { retry: false }, mutations: { retry: false } } });
  const invalidate = vi.spyOn(client, 'invalidateQueries');
  render(<QueryClientProvider client={client}><PurchasePayment order={input} /></QueryClientProvider>);
  return invalidate;
};
const start = () => fireEvent.click(screen.getByRole('button', { name: 'Record Payment' }));
const submit = () => fireEvent.submit(screen.getByRole('button', { name: 'Save Payment Total' }).closest('form')!);
beforeEach(() => {
  vi.resetAllMocks(); useAuthStore.setState({ user: { currentBusiness: { role: 'Owner', permissions: [] } } as never });
  vi.mocked(purchaseApi.updatePayment).mockResolvedValue(order);
});
it('shows the server balance and records a cumulative amount with the loaded version', async () => {
  const invalidate = renderPayment(); expect(screen.getByText('₹777.00')).toBeInTheDocument();
  start(); expect(purchaseApi.updatePayment).not.toHaveBeenCalled();
  fireEvent.change(screen.getByLabelText('Total paid to date'), { target: { value: '12.5' } }); submit();
  expect(await screen.findByRole('status')).toHaveTextContent('recorded');
  expect(purchaseApi.updatePayment).toHaveBeenCalledWith('p1', 12.5, 7);
  expect(invalidate).toHaveBeenCalledWith({ queryKey: purchaseKeys.all });
});
it('preserves the entered amount and shows a safe conflict message', async () => {
  vi.mocked(purchaseApi.updatePayment).mockRejectedValue({ isAxiosError: true, response: { status: 409, data: { error: 'PURCHASE_VERSION_CONFLICT' } } });
  renderPayment(); start(); fireEvent.change(screen.getByLabelText('Total paid to date'), { target: { value: '12.5' } }); submit();
  expect(await screen.findByRole('alert')).toHaveTextContent('record changed');
  expect(screen.getByLabelText('Total paid to date')).toHaveValue(12.5);
});
it('prevents duplicate payment submissions during a pending request', async () => {
  let resolve!: (value: PurchaseOrderDto) => void;
  vi.mocked(purchaseApi.updatePayment).mockReturnValue(new Promise(r => { resolve = r; }));
  renderPayment(); start(); submit(); submit(); await act(async () => {});
  expect(purchaseApi.updatePayment).toHaveBeenCalledTimes(1); await act(async () => resolve(order));
});
it.each(['-1', '0.00001'])('rejects invalid amount %s without a write', async value => {
  renderPayment(); start(); fireEvent.change(screen.getByLabelText('Total paid to date'), { target: { value } }); submit();
  expect(await screen.findByRole('alert')).toHaveTextContent('nonnegative paid total'); expect(purchaseApi.updatePayment).not.toHaveBeenCalled();
});
it('hides financial actions from managers even with purchase.edit permission', () => {
  useAuthStore.setState({ user: { currentBusiness: { role: 'Manager', permissions: ['purchase.edit'] } } as never });
  renderPayment(order);
  expect(screen.queryByRole('button', { name: 'Record Payment' })).not.toBeInTheDocument();
  expect(screen.getAllByText('Owner only')).toHaveLength(2);
});
it.each([0, 6])('does not offer payment for purchase status %s', status => {
  renderPayment({ ...order, status: status as 0 | 6 }); expect(screen.queryByRole('button', { name: 'Record Payment' })).not.toBeInTheDocument();
});
