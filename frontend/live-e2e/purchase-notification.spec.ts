import { test, expect, type Page } from '@playwright/test';
import { HubConnectionBuilder, LogLevel } from '@microsoft/signalr';
import { readFileSync, writeFileSync } from 'node:fs';

// Every request and WebSocket reaches the actual disposable API; no routes mocked.
const api = 'http://localhost:5182/api/v1';
const session = JSON.parse(readFileSync('../TestResults/runtime-session.local.json', 'utf8')) as {
  database: string; businessId: string; foreignBusinessId: string; itemId: string; supplierId: string;
  password: string; actors: Record<string, { id: string; email: string }>;
};
if (!/^wa_runtime_[a-f0-9]{32}$/.test(session.database)) throw new Error('A disposable runtime fixture is required.');
type Event = { id: string; type: string; businessId: string; payload: { itemId: string | null; purchaseId: string | null } };
type RuntimeWindow = Window & { runtimeSockets: WebSocket[] };
async function signIn(page: Page, actor: string) {
  await page.goto('/login');
  await page.getByLabel('Email address').fill(session.actors[actor].email);
  await page.getByLabel('Password', { exact: true }).fill(session.password);
  const pending = page.waitForResponse(r => r.url().endsWith('/auth/login') && r.request().method() === 'POST');
  await page.getByRole('button', { name: 'Sign in', exact: true }).click();
  const result = await pending; expect(result.status()).toBe(200);
  await expect(page).not.toHaveURL(/login/);
  return (await result.json()).data.accessToken as string;
}

test('actual purchase and notification delivery, authorization, UI, reconnect and reload', async ({ page, request }, info) => {
  await page.addInitScript(() => {
    const Native = window.WebSocket; (window as RuntimeWindow).runtimeSockets = [];
    window.WebSocket = class extends Native { constructor(url: string | URL, protocols?: string | string[]) { super(url, protocols); (window as RuntimeWindow).runtimeSockets.push(this); } };
  });
  const browserEvents: Event[] = [];
  const unreadRequests: string[] = [];
  page.on('request', r => { if (r.url() === api + '/notifications/unread-count') unreadRequests.push(r.url()); });
  page.on('websocket', socket => {
    if (!socket.url().includes('/realtime')) return;
    socket.on('framereceived', frame => { for (const part of frame.payload.toString().split('\u001e').filter(Boolean)) {
      try { const message = JSON.parse(part); if (message.target === 'businessEvent') browserEvents.push(message.arguments[0]); } catch { /* SignalR transport control frame. */ }
    } });
  });
  const login = async (actor: string) => {
    const response = await request.post(api + '/auth/login', { data: { email: session.actors[actor].email, password: session.password } });
    expect(response.status()).toBe(200); return (await response.json()).data.accessToken as string;
  };
  const owner = await login('owner'); const headers = { Authorization: 'Bearer ' + owner };
  const input = { supplierId: session.supplierId, notes: 'Runtime initial purchase', items: [{ catalogItemId: session.itemId, orderedQuantity: 2, unit: 'PCS', unitPrice: 4.5 }] };
  const preview = await request.post(api + '/purchases/preview', { headers, data: input }); expect(preview.status()).toBe(200);
  const created = await request.post(api + '/purchases', { headers, data: { ...input, previewToken: (await preview.json()).previewToken } }); expect(created.status()).toBe(201);
  let order = await created.json();
  writeFileSync('../TestResults/runtime-purchase-scope.json', JSON.stringify({ purchaseId: order.id, businessId: session.businessId }));
  const manager = await signIn(page, 'manager');
  await page.goto('/purchases/' + order.id);
  await expect(page.getByText(input.notes, { exact: true })).toBeVisible();
  const online = () => page.evaluate(() => (window as RuntimeWindow).runtimeSockets.filter(s => s.url.includes('/realtime') && s.readyState === WebSocket.OPEN).length);
  await expect.poll(online).toBe(1);
  const foreign = await login('foreign'), limited = await login('limited');
  const delivered: Record<string, Event[]> = { manager: [], foreign: [], limited: [] };
  const connect = async (token: string, actor: string) => {
    const connection = new HubConnectionBuilder().withUrl(api + '/realtime', { accessTokenFactory: () => token })
      .withAutomaticReconnect([0, 100, 1000]).configureLogging(LogLevel.None).build();
    connection.on('businessEvent', (event: Event) => delivered[actor].push(event)); await connection.start(); return connection;
  };
  const connections = await Promise.all([connect(manager, 'manager'), connect(foreign, 'foreign'), connect(limited, 'limited')]);
  const managerHeaders = { Authorization: 'Bearer ' + manager }, foreignHeaders = { Authorization: 'Bearer ' + foreign }, limitedHeaders = { Authorization: 'Bearer ' + limited };
  const family = (actor: string, type: string) => delivered[actor].filter(e => e.type === type);
  const browserFamily = (type: string) => browserEvents.filter(e => e.type === type);
  const update = async (notes: string) => {
    const payload = { ...input, notes, expectedVersion: order.version };
    const review = await request.post(api + '/purchases/preview', { headers, data: payload }); expect(review.status()).toBe(200);
    const response = await request.put(api + '/purchases/' + order.id, { headers, data: { ...payload, previewToken: (await review.json()).previewToken } });
    expect(response.status()).toBe(200); order = await response.json(); return order;
  };
  const notify = async () => {
    const response = await request.post(api + '/purchases/' + order.id + '/damage-reports', { headers,
      data: { catalogItemId: session.itemId, qtyDamaged: 1, damageType: 'Damaged', emitNotification: true, notes: 'Disposable runtime notification' } });
    expect(response.status()).toBe(201); return response.json();
  };
  try {
    expect((await request.post(api + '/realtime/negotiate?negotiateVersion=1')).status()).toBe(401);
    expect((await request.get(api + '/purchases/' + order.id, { headers: limitedHeaders })).status()).toBe(403);
    expect((await request.get(api + '/purchases/' + order.id, { headers: foreignHeaders })).status()).toBe(404);
    expect((await request.put(api + '/purchases/' + order.id, { headers: limitedHeaders, data: input })).status()).toBe(403);
    expect((await request.get(api + '/notifications', { headers: limitedHeaders })).status()).toBe(200);
    await update('Runtime purchase updated once');
    await expect(page.getByText('Runtime purchase updated once', { exact: true })).toBeVisible();
    await expect.poll(() => family('manager', 'purchase.changed').length).toBe(1);
    await expect.poll(() => browserFamily('purchase.changed').length).toBe(1);
    const purchaseEvent = family('manager', 'purchase.changed')[0];
    expect(purchaseEvent.businessId).toBe(session.businessId); expect(purchaseEvent.payload.purchaseId).toBe(order.id);
    expect(browserFamily('purchase.changed')[0].id).toBe(purchaseEvent.id);
    expect(Object.keys(purchaseEvent.payload).sort()).toEqual(['itemId', 'purchaseId']);
    const report = await notify();
    await expect.poll(() => family('manager', 'notification.changed').length).toBe(1);
    await expect.poll(() => family('limited', 'notification.changed').length).toBe(1);
    await expect(page.getByRole('button', { name: 'Notifications', exact: true })).toHaveAccessibleDescription('1 unread notifications');
    const notificationList = await request.get(api + '/notifications', { headers: managerHeaders }); expect(notificationList.status()).toBe(200);
    const notification = (await notificationList.json()).data.find((n: { referenceId: string }) => n.referenceId === report.id); expect(notification).toBeTruthy();
    expect((await (await request.get(api + '/notifications', { headers: limitedHeaders })).json()).data).toEqual([]);
    expect((await (await request.get(api + '/notifications', { headers: foreignHeaders })).json()).data).toEqual([]);
    await page.getByRole('button', { name: 'Notifications', exact: true }).click();
    await page.getByRole('button', { name: /Damage reported/ }).click();
    await expect(page.getByRole('button', { name: 'Notifications', exact: true })).not.toHaveAttribute('aria-describedby');
    await expect.poll(() => family('manager', 'notification.changed').length).toBe(2);
    expect((await (await request.get(api + '/notifications', { headers: managerHeaders })).json()).data[0].isRead).toBe(true);
    await page.context().setOffline(true);
    await page.evaluate(() => (window as RuntimeWindow).runtimeSockets.filter(s => s.url.includes('/realtime')).forEach(s => s.close()));
    await expect.poll(online).toBe(0);
    await update('Runtime purchase missed while offline'); await notify();
    await expect.poll(() => family('manager', 'purchase.changed').length).toBe(2);
    await page.context().setOffline(false); await expect.poll(online, { timeout: 20_000 }).toBe(1);
    await expect(page.getByText('Runtime purchase missed while offline', { exact: true })).toBeVisible();
    await expect(page.getByRole('button', { name: 'Notifications', exact: true })).toHaveAccessibleDescription('1 unread notifications');
    await page.reload(); await expect.poll(online).toBe(1);
    await expect(page.getByText('Runtime purchase missed while offline', { exact: true })).toBeVisible();
    await expect(page.getByRole('button', { name: 'Notifications', exact: true })).toHaveAccessibleDescription('1 unread notifications');
    const beforeReloadEvents = browserFamily('purchase.changed').length;
    await update('Runtime purchase after reload');
    await expect(page.getByText('Runtime purchase after reload', { exact: true })).toBeVisible();
    await expect.poll(() => family('manager', 'purchase.changed').length).toBe(3);
    await expect.poll(() => browserFamily('purchase.changed').length).toBe(beforeReloadEvents + 1);
    await page.getByRole('button', { name: 'Notifications', exact: true }).click();
    await page.getByRole('button', { name: 'Mark all read' }).click();
    await expect(page.getByRole('button', { name: 'Notifications', exact: true })).not.toHaveAttribute('aria-describedby');
    await expect.poll(() => family('manager', 'notification.changed').length).toBe(4);
    // Longer than the old 30-second polling interval, with no route/focus change.
    await page.keyboard.press('Escape'); const stableCount = unreadRequests.length;
    await new Promise(resolve => setTimeout(resolve, 32_000));
    expect(unreadRequests).toHaveLength(stableCount);
    expect(family('manager', 'purchase.changed')).toHaveLength(3);
    expect(family('limited', 'purchase.changed')).toHaveLength(0);
    expect(delivered.foreign).toHaveLength(0);
    expect(family('limited', 'notification.changed')).toHaveLength(4);
    expect(new Set(delivered.manager.map(e => e.id)).size).toBe(delivered.manager.length);
    expect(new Set(browserEvents.map(e => e.id)).size).toBe(browserEvents.length);
    expect(delivered.manager.every(e => e.businessId === session.businessId)).toBe(true);
    expect(family('manager', 'notification.changed').every(e => e.payload.itemId === null && e.payload.purchaseId === null)).toBe(true);
    await page.screenshot({ path: info.outputPath('actual-realtime-purchase-notification.png') });
    writeFileSync('../TestResults/runtime-live-delivery.json', JSON.stringify({ database: session.database, businessId: session.businessId,
      purchaseId: order.id, purchaseDeliveries: 3, notificationDeliveries: 4, foreignDeliveries: 0, unauthorizedPurchaseDeliveries: 0,
      sameBusinessNotificationPolicyVerified: true, anonymousHandshake: 401, perUserNotificationIsolation: true,
      reconnect: true, reload: true, badgeCreatedAndRead: true, duplicates: 0, idleObservationSeconds: 32, idleUnreadRequests: 0 }, null, 2));
  } finally { await Promise.all(connections.map(c => c.stop())); }
});
