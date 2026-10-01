// Display only: values come from authoritative backend responses.
export function formatMoney(value: number | null | undefined): string {
  if (value == null || !Number.isFinite(value)) return 'Owner only';
  return new Intl.NumberFormat('en-IN', { style: 'currency', currency: 'INR', maximumFractionDigits: 2 }).format(value);
}
