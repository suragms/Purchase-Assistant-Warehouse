import { test, expect, type Page } from '@playwright/test';

const sizes = [[390, 844], [393, 852], [412, 915], [1366, 768], [1440, 900], [1920, 1080]];
const preview = { businessId: 'b1', label: 'Preview only — no data will be saved.', synthetic: true, writesPerformed: false, persistenceAvailable: false, confirmationAvailable: false,
  summary: { totalRows: 5, validRows: 1, warningRows: 1, rejectedRows: 1, ambiguousRows: 1, notFoundRows: 1, outOfScopeRows: 1, duplicateRows: 1 },
  unchangedAreas: ['Current and physical stock', 'Purchases and purchase lines', 'Financial totals'],
  rows: ['VALID', 'WARNING', 'REJECTED', 'AMBIGUOUS', 'NOT_FOUND'].map((outcome, i) => ({ case: 'synthetic-' + outcome, rowIdentifier: 'row-' + i, match: i < 3 ? 'MATCHED' : outcome, outcome, duplicate: i === 2, reasons: i === 2 ? ['DUPLICATE_PROVENANCE'] : [],
    provenance: { sourceIdentifier: 'synthetic-source', sourceKind: 'SyntheticFixture', importIdentifier: 'synthetic-import', rowIdentifier: 'row-' + i, sourceRowIdentifier: 'source-row-' + i, actor: 'u1', recordedAt: '2026-10-02T00:00:00Z', sourceTimestamp: null, priorRevision: null, correctionReason: null },
    unchangedCurrentValues: i < 3 ? { currentStock: '8.125', physicalStock: '7.5', catalogName: 'Synthetic current item' } : {},
    fields: [{ field: 'historicalName', state: 'KNOWN', outcome: 'VALID', proposedValue: 'സിന്തറ്റിക് അരി · اختبار · 米 🌾', reasonCode: 'SOURCE_VALUE_VALIDATED', message: 'Historical metadata only.', originalAllowedValue: 'സിന്തറ്റിക് അരി · اختبار · 米 🌾', sourceCell: 'synthetic-cell' },
      { field: 'sellingRate', state: i === 1 ? 'UNKNOWN' : 'KNOWN', outcome: i === 1 ? 'WARNING' : 'VALID', proposedValue: i === 1 ? null : '0', reasonCode: i === 1 ? 'UNKNOWN' : 'SOURCE_VALUE_VALIDATED', message: 'No value inferred.', originalAllowedValue: i === 1 ? null : '0', sourceCell: 'synthetic-cell' }] })) };
async function fixture(page: Page, role: string) {
  const calls: string[] = []; let fail = false;
  await page.route('**/api/v1/**', async route => {
    const request = route.request(), path = new URL(request.url()).pathname.replace('/api/v1', ''); calls.push(request.method() + ' ' + path);
    if (path === '/auth/refresh') return route.fulfill({ json: { data: { accessToken: 'synthetic-browser-session', user: { id: 'u1', name: 'Synthetic reviewer', email: 'synthetic@test.local', businesses: [], currentBusiness: { businessId: 'b1', businessName: 'Harisree Agency', role, permissions: ['reports.view', 'catalog.edit', 'purchase.edit', 'purchase.view'] } } } } });
    if (path === '/exports/historical/preview') { expect(request.postDataJSON()).toEqual({ fixtureId: expect.any(String) }); return route.fulfill(fail ? { status: 503, json: { message: 'Preview unavailable. Try again.' } } : { json: preview }); }
    const data = path === '/settings/business' ? { name: 'Harisree Agency', version: 'v1', hasUploadedLogo: false, logoUploadAvailable: false } :
      path === '/settings/profile' ? { id: 'u1', name: 'Synthetic reviewer', email: 'synthetic@test.local' } :
      path === '/settings/notifications' ? { notificationsEnabled: false, notificationKinds: [] } :
      path === '/operations/owner-dashboard' ? { lowStockCount: 0, outOfStockCount: 0, pendingDamageCount: 0, exceptions: [], staffPerformance: [] } :
      path === '/notifications/unread-count' ? { count: 0 } : [];
    await route.fulfill({ json: data });
  });
  return { calls, fail: (value: boolean) => { fail = value; } };
}
for (const role of ['Owner', 'Manager', 'Staff']) for (const [width, height] of sizes) test(role + ' historical preview at ' + width + 'x' + height, async ({ page }, info) => {
  await page.setViewportSize({ width, height }); const errors: string[] = []; page.on('pageerror', e => errors.push(e.message)); const api = await fixture(page, role); await page.goto('/settings');
  await expect(page.getByRole('heading', { name: 'Settings', exact: true })).toBeVisible();
  const panel = page.getByRole('region', { name: 'Historical data preview' });
  if (role !== 'Owner') { await expect(panel).toHaveCount(0); expect(api.calls.some(x => x.includes('/historical/'))).toBe(false); }
  else {
    await expect(panel).toBeVisible(); await expect(panel.getByText('Preview only — no data will be saved.')).toBeVisible();
    await panel.getByRole('button', { name: 'Validate synthetic fixture' }).click(); await expect(panel.getByText('5 synthetic rows checked. No data saved.')).toBeVisible();
    await panel.locator('summary').filter({ hasText: 'synthetic-WARNING' }).click(); await expect(panel.getByText('UNKNOWN · WARNING')).toBeVisible(); await expect(panel.getByText('Proposed: NULL — no value inferred')).toBeVisible();
    await expect(panel.getByRole('button', { name: /confirm|commit|save/i })).toHaveCount(0);
    await panel.scrollIntoViewIfNeeded(); await page.screenshot({ path: info.outputPath('historical-preview.png'), fullPage: true });
    await panel.getByRole('combobox').selectOption('missing'); await expect(panel.getByText('5 synthetic rows checked. No data saved.')).toHaveCount(0);
    // The existing realtime negotiation is a connection handshake, independent of historical preview.
    expect(api.calls.filter(x => x.startsWith('POST') && !x.includes('/auth/') && x !== 'POST /realtime/negotiate')).toEqual(['POST /exports/historical/preview']);
  }
  expect(await page.evaluate(() => document.documentElement.scrollWidth <= innerWidth && document.querySelector('main')!.scrollWidth <= document.querySelector('main')!.clientWidth)).toBe(true); expect(errors).toEqual([]);
});
test('historical preview recovers from failure and scoped SuperAdmin can review', async ({ page }) => {
  const api = await fixture(page, 'SuperAdmin'); await page.goto('/settings'); const panel = page.getByRole('region', { name: 'Historical data preview' }); api.fail(true);
  await panel.getByRole('button').click(); await expect(panel.getByRole('alert')).toBeVisible(); api.fail(false); await panel.getByRole('button').click(); await expect(panel.getByText('5 synthetic rows checked. No data saved.')).toBeVisible();
});
