import { act, render } from '@testing-library/react';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { beforeEach, expect, it, vi } from 'vitest';
import { RealtimeUpdates } from '../components/RealtimeUpdates';
import { useAuthStore } from '../stores/authStore';
const hub = vi.hoisted(() => ({ connections: [] as { handlers: Record<string, (event?: unknown) => void>; start: ReturnType<typeof vi.fn>; stop: ReturnType<typeof vi.fn> }[] }));
vi.mock('@microsoft/signalr', () => ({ LogLevel: { None: 0 }, HubConnectionBuilder: class {
  withUrl() { return this; } withAutomaticReconnect() { return this; } configureLogging() { return this; }
  build() { const connection = { handlers: {} as Record<string, (event?: unknown) => void>, start: vi.fn().mockResolvedValue(undefined), stop: vi.fn().mockResolvedValue(undefined),
    on(name: string, callback: (event?: unknown) => void) { this.handlers[name] = callback; }, onreconnected(callback: () => void) { this.handlers.reconnected = callback; }, onclose(callback: () => void) { this.handlers.close = callback; } }; hub.connections.push(connection); return connection; }
} }));
beforeEach(() => {
  hub.connections.length = 0;
  useAuthStore.setState({ accessToken: 'token-a', user: { id: 'u1', name: 'A', email: 'a@example.test', businesses: [], currentBusiness: { businessId: 'a', businessName: 'A', role: 'Staff', permissions: ['purchase.view'] } } });
});
function view() { const cache = new QueryClient(); const invalidate = vi.spyOn(cache, 'invalidateQueries'); const result = render(<QueryClientProvider client={cache}><RealtimeUpdates /></QueryClientProvider>); return { ...result, invalidate }; }
it('refreshes purchase caches once and rejects duplicate IDs and foreign-tenant frames', () => {
  const { invalidate } = view(), receive = hub.connections[0].handlers.businessEvent;
  act(() => { receive({ id: '1', type: 'purchase.changed', businessId: 'a' }); receive({ id: '1', type: 'purchase.changed', businessId: 'a' }); receive({ id: '2', type: 'purchase.changed', businessId: 'foreign' }); });
  expect(invalidate.mock.calls.map(call => call[0]?.queryKey)).toEqual([['purchases'], ['dashboard'], ['reports']]);
});
it('refreshes the existing notification query family without polling or financial caches', () => {
  const { invalidate } = view(); act(() => hub.connections[0].handlers.businessEvent({ id: 'n1', type: 'notification.changed', businessId: 'a' }));
  expect(invalidate.mock.calls.map(call => call[0]?.queryKey)).toEqual([['notifications'], ['damage-reports']]);
});
it('refreshes stock, purchase, notification and dashboard data after reconnect', () => {
  const { invalidate } = view(); act(() => hub.connections[0].handlers.reconnected());
  expect(invalidate.mock.calls.map(call => call[0]?.queryKey)).toEqual([['stock'], ['purchases'], ['notifications'], ['dashboard']]);
});
it('stops the old subscription on a business switch and ignores stale callbacks', async () => {
  const { invalidate } = view(); const old = hub.connections[0];
  // Establish the original connection before switching, so this checks a live subscription.
  await act(async () => { await Promise.resolve(); });
  await act(async () => useAuthStore.setState(s => ({ accessToken: 'token-b', user: { ...s.user!, currentBusiness: { ...s.user!.currentBusiness!, businessId: 'b' } } })));
  expect(old.stop).toHaveBeenCalledTimes(1);
  act(() => { old.handlers.businessEvent({ id: 'old', type: 'purchase.changed', businessId: 'a' }); hub.connections[1].handlers.businessEvent({ id: 'new', type: 'notification.changed', businessId: 'b' }); });
  expect(invalidate.mock.calls.map(call => call[0]?.queryKey)).toEqual([['notifications'], ['damage-reports']]);
});
it('does not connect without an active selected business session', () => {
  useAuthStore.setState({ accessToken: null }); view(); expect(hub.connections).toHaveLength(0);
});
