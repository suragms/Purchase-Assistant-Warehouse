import { defineConfig } from '@playwright/test';

// Explicit opt-in. The runner must prepare a disposable database/API/session.
// Ordinary browser regression continues to use playwright.config.ts.
export default defineConfig({
  testDir: './live-e2e', testMatch: 'purchase-notification.spec.ts',
  fullyParallel: false, workers: 1, timeout: 120_000,
  outputDir: './test-results/realtime-runtime',
  use: { baseURL: 'http://localhost:5181', channel: 'msedge', headless: true,
    viewport: { width: 393, height: 852 }, serviceWorkers: 'block',
    screenshot: 'only-on-failure', trace: 'retain-on-failure' },
});
