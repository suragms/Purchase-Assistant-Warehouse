import { act, render, screen } from '@testing-library/react';
import { QueryClient, QueryClientProvider, useQuery } from '@tanstack/react-query';
import { beforeEach, expect, it, vi } from 'vitest';
import { StrictMode, useState } from 'react';
import { AuthProvider } from '../auth/AuthProvider';
import { useAuthStore } from '../stores/authStore';
import apiClient from '../api/apiClient';
import type { User } from '../types/auth';

vi.mock('../api/apiClient', () => ({ default: { post: vi.fn() } }));
const user = (businessId: string, role = 'Owner'): User => ({ id: 'u1', name: 'User', email: 'test@example.test', businesses: [],
  currentBusiness: { businessId, businessName: businessId, role, permissions: ['catalog.view'] } } as User);
beforeEach(() => { vi.resetAllMocks(); useAuthStore.getState().setSession('session-a', user('a'));
  vi.mocked(apiClient.post).mockResolvedValue({ data: { data: { accessToken: 'refreshed', user: user('a') } } }); });
function Page() {
  const [draft, setDraft] = useState('');
  const data = useQuery({ queryKey: ['catalog', 'list'], queryFn: async () => useAuthStore.getState().user?.currentBusiness?.businessId });
  return <><p>Business data: {data.data || 'loading'}</p><button onClick={() => setDraft('private draft')}>Edit draft</button><p>{draft}</p></>;
}
const mount = () => {
  const client = new QueryClient({ defaultOptions: { queries: { retry: false, staleTime: Infinity } } });
  render(<QueryClientProvider client={client}><AuthProvider><Page /></AuthProvider></QueryClientProvider>); return client;
};
it('clears tenant data and local draft state before a different business renders', async () => {
  const client = mount(); await screen.findByText('Business data: a');
  await act(async () => screen.getByRole('button', { name: 'Edit draft' }).click()); expect(screen.getByText('private draft')).toBeInTheDocument();
  await act(async () => useAuthStore.getState().setSession('session-b', user('b')));
  await screen.findByText('Business data: b'); expect(screen.queryByText('private draft')).not.toBeInTheDocument();
  expect(client.getQueryData(['catalog', 'list'])).toBe('b'); expect(apiClient.post).toHaveBeenCalledTimes(1);
});
it('clears financial cache on a role change and clears all queries on logout', async () => {
  const client = mount(); await screen.findByText('Business data: a'); client.setQueryData(['reports'], { grandTotal: 123 });
  await act(async () => useAuthStore.getState().setSession('new-token', user('a', 'Manager')));
  expect(client.getQueryData(['reports'])).toBeUndefined();
  await act(async () => useAuthStore.getState().logout()); expect(client.getQueryData(['catalog', 'list'])).toBeUndefined();
});
it('preserves cache when only the access token rotates in the same scope', async () => {
  const client = mount(); await screen.findByText('Business data: a'); client.setQueryData(['reports'], { count: 2 });
  await act(async () => useAuthStore.getState().setSession('rotated-token', user('a')));
  expect(client.getQueryData(['reports'])).toEqual({ count: 2 });
});
it('performs one startup refresh under Strict Mode', async () => {
  const client = new QueryClient();
  render(<StrictMode><QueryClientProvider client={client}><AuthProvider><Page /></AuthProvider></QueryClientProvider></StrictMode>);
  await screen.findByText('Business data: a'); expect(apiClient.post).toHaveBeenCalledTimes(1);
});
