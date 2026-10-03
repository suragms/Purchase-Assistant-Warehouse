import { test, expect } from '@playwright/test';
import { navigationFixture } from './navigation-fixture';

for (const width of [320, 768, 1440]) for (const role of ['Owner', 'Staff'] as const) {
  test(`Completion predictions ${role} at ${width}px`, async ({ page }) => {
    await page.setViewportSize({ width, height: 900 }); await navigationFixture(page, role);
    const errors: string[] = []; page.on('pageerror', error => errors.push(error.message));
    await page.route('**/api/v1/ml/**', async route => {
      const path = new URL(route.request().url()).pathname;
      await route.fulfill({ json: path.endsWith('/items') ? { items: [{ id: 'c1', name: 'Rice', itemCode: 'R1' }], totalCount: 1 } : {
        itemId: 'c1', itemName: 'Rice', unit: 'kg', currentStock: 10, status: 'ready', message: 'Forecast supports review.',
        model: 'ridge', modelVersion: 'fixture-only-model-not-production', trainedAt: '2026-10-02', metrics: { mae: 2, rmse: 3, wape: .15 },
        history: [{ date: '2026-10-02', quantity: 3 }], forecast: [{ date: '2026-10-03', quantity: 4, lower: 2, upper: 6 }],
        reorder: { quantity: 5, reorderDate: '2026-10-04', riskCategory: 'possible', reason: 'Review forecast demand and available stock.' }, anomalies: []
      } });
    });
    await page.goto('/ml'); await page.getByRole('combobox', { name: 'Item', exact: true }).selectOption('c1');
    await expect(page.getByRole('heading', { name: 'Daily forecast' })).toBeVisible();
    await expect(page.getByText('Review forecast demand and available stock.')).toBeVisible();
    await page.getByRole('combobox', { name: 'Forecast horizon', exact: true }).selectOption('30');
    await expect(page.getByText('Available stock: 10 kg')).toBeVisible();
    expect(await page.evaluate(() => document.documentElement.scrollWidth <= innerWidth + 1)).toBeTruthy(); expect(errors).toEqual([]);
    await page.screenshot({ path: `../TestResults/phase3-predictions-${role}-${width}.png`, fullPage: true });
  });
  test(`Completion invoice review ${role} at ${width}px`, async ({ page }) => {
    await page.setViewportSize({ width, height: 900 }); const calls = await navigationFixture(page, role);
    await page.route('**/api/v1/ai/invoice-text', route => route.fulfill({ json: { items: [{ name: 'Rice', quantity: 10, unit: 'kg', catalogItemId: 'c1', ...(role === 'Owner' ? { rate: 50 } : {}) }], unparsedLines: 0, source: 'local_text', message: 'Review every line. Nothing is saved.' } }));
    await page.goto('/purchases/new'); await page.getByText('Extract pasted invoice text', { exact: true }).click();
    await page.getByLabel('Invoice text', { exact: true }).fill('Rice 10 kg @ 50'); await page.getByRole('button', { name: 'Preview invoice lines' }).click();
    await expect(page.getByText('Review every line. Nothing is saved.')).toBeVisible();
    expect(calls.filter(x => x === 'POST /purchases')).toEqual([]);
    await page.getByRole('button', { name: 'Apply reviewed quantities to form' }).click();
    expect(calls.filter(x => x === 'POST /purchases')).toEqual([]);
    expect(await page.evaluate(() => document.documentElement.scrollWidth <= innerWidth + 1)).toBeTruthy();
  });
}
test('Owner audit and provider policy are usable and persist through a refetch', async ({ page }) => {
  await navigationFixture(page, 'Owner'); let enabled = true;
  await page.route('**/api/v1/settings/ai', async route => {
    if (route.request().method() === 'PUT') enabled = route.request().postDataJSON().enabled;
    await route.fulfill({ json: { enabled, providerOrder: ['OpenAI'], models: {}, timeoutSeconds: 8, retries: 0, version: 'fixture-version' } });
  });
  await page.route('**/api/v1/audit**', route => route.fulfill({ json: { items: [{ id: 'a1', userId: 'u1', eventType: 'CatalogItemModified', description: 'Rice updated', createdAt: '2026-10-03T10:00:00Z', metadataJson: '{"changes":{"name":"Rice"}}' }], totalCount: 1 } }));
  await page.goto('/settings'); await page.getByLabel('Enable purchase AI for this business').uncheck(); await page.getByRole('button', { name: 'Save AI routing' }).click();
  await expect(page.getByText('AI routing saved.')).toBeVisible(); expect(enabled).toBe(false);
  await page.goto('/audit'); await expect(page.getByText('CatalogItemModified')).toBeVisible(); await page.getByText('Changes', { exact: true }).click(); await expect(page.locator('pre')).toContainText('Rice');
});
test('Owner confirms WhatsApp recipient before a fixture delivery; accepted is not reported as delivered', async ({ page }) => {
  await navigationFixture(page, 'Owner'); let sent = false;
  await page.route('**/api/v1/purchases/p1/delivery/whatsapp', async route => {
    if (route.request().method() === 'POST') { const body = route.request().postDataJSON(); expect(body.confirmed).toBe(true); expect(body.recipient).toBe('919999999999'); sent = true; }
    await route.fulfill({ json: { ready: true, eligible: true, recipient: '919999999999', purchaseVersion: 1, delivery: sent ? { version: 'v1', status: 'accepted', attempts: 1 } : null } });
  });
  page.on('dialog', dialog => dialog.accept()); await page.goto('/purchases/p1');
  await expect(page.getByText('Recipient: +919999999999')).toBeVisible(); await page.getByRole('button', { name: 'Send purchase via WhatsApp' }).click();
  await expect(page.getByText(/Accepted by Meta; recipient delivery has not been verified/)).toBeVisible(); expect(sent).toBe(true);
});
