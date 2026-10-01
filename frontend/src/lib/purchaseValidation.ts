import { isAxiosError } from 'axios';

export function purchaseErrorMessage(error: unknown): string {
  if (!isAxiosError(error)) return 'The request failed. Please try again.';
  if (error.code === 'ECONNABORTED' || error.code === 'ETIMEDOUT') return 'The request timed out. Please try again.';
  const payload = error.response?.data;
  const safeMessage = typeof payload?.error === 'string' ? payload.error
    : payload?.error?.message ?? payload?.detail;
  if ([400, 404, 409].includes(error.response?.status ?? 0) && typeof safeMessage === 'string' && safeMessage.length <= 500
      && !/stack|exception|password|secret|api[_ -]?key|connectionstring| at .*line \d/i.test(safeMessage)) {
    if (safeMessage === 'PURCHASE_VERSION_CONFLICT' || safeMessage === 'STOCK_VERSION_CONFLICT')
      return 'This record changed. Refresh it and review the latest values before trying again.';
    if (!/^[A-Z_]+$/.test(safeMessage)) return safeMessage;
  }
  switch (error.response?.status) {
    case 400: return 'Check the supplier, items, quantities, and prices, then try again.';
    case 401: return 'Your session has expired. Please sign in again.';
    case 403: return 'You do not have permission to perform this action.';
    case 409: return 'This purchase may already exist or has changed. Check the purchase list before continuing.';
    case 429: return 'Too many requests. Wait a moment and try again.';
    case 500: case 502: case 503: case 504: return 'The service is temporarily unavailable. Please try again.';
    default: return 'Unable to connect. Check your connection and try again.';
  }
}

export const MAX_PURCHASE_VALUE = 99999999999999;
export const isValidQuantity = (value: number) => Number.isFinite(value) && value > 0 &&
  value <= MAX_PURCHASE_VALUE && Math.abs(value * 10000 - Math.round(value * 10000)) < 0.0001;
