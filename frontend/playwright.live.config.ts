import { defineConfig } from '@playwright/test';
// Explicit opt-in: the local API and isolated accounts must already be running.
export default defineConfig({
  testDir: './live-e2e', fullyParallel: false, workers: 1,
  outputDir: './test-results/live',
  use: { baseURL: 'http://localhost:5179', channel: 'msedge', headless: true, screenshot: 'only-on-failure', trace: 'retain-on-failure' },
  webServer: { command: 'npm run build && npm run preview -- --host 127.0.0.1 --port 5179', url: 'http://localhost:5179', reuseExistingServer: false },
});
