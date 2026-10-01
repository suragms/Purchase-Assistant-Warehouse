import { defineConfig } from '@playwright/test';
export default defineConfig({
  testDir: './e2e',
  fullyParallel: false,
  workers: 1,
  use: { baseURL: 'http://127.0.0.1:5179', channel: 'msedge', headless: true, screenshot: 'only-on-failure', trace: 'retain-on-failure' },
  webServer: { command: 'npm run build && npm run preview -- --host 127.0.0.1 --port 5179', url: 'http://127.0.0.1:5179', reuseExistingServer: false },
});
