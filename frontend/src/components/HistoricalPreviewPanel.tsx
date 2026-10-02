import { useState } from 'react';
import { historicalFixtureSuites, previewHistoricalFixture, type HistoricalPreview } from '../api/exportsApi';
import { useAuthStore } from '../stores/authStore';
import { purchaseErrorMessage } from '../lib/purchaseValidation';

const fieldLabels: Record<string, string> = { openingStock: 'Opening stock', businessDate: 'Business date', sellingRate: 'Selling rate (INR per purchase quantity unit)', historicalName: 'Historical item name', historicalUnit: 'Historical unit', kgPerUnit: 'Kg per unit', totalWeight: 'Total weight', normalizedQuantity: 'Normalized quantity' };
const labels: Record<string, string> = { sourceIdentifier: 'Source', sourceKind: 'Source kind', importIdentifier: 'Import identifier', rowIdentifier: 'Row identifier', sourceRowIdentifier: 'Source row', actor: 'Actor', recordedAt: 'Recorded at', sourceTimestamp: 'Original source time', priorRevision: 'Prior correction revision', correctionReason: 'Correction reason', currentStock: 'Current stock', physicalStock: 'Physical stock', catalogName: 'Catalog name', stockUnit: 'Stock unit', lineUnit: 'Purchase line unit', kgPerUnit: 'Kg per unit' };
export function HistoricalPreviewPanel() {
  const user = useAuthStore(s => s.user);
  const [fixture, setFixture] = useState<keyof typeof historicalFixtureSuites>('mixed');
  const [result, setResult] = useState<HistoricalPreview | null>(null);
  const [busy, setBusy] = useState(false), [error, setError] = useState('');
  if (!user?.currentBusiness || !['Owner', 'SuperAdmin'].includes(user.currentBusiness.role)) return null;
  const visible = result?.businessId === user.currentBusiness.businessId ? result : null;
  async function validate() {
    if (busy) return;
    setBusy(true); setError(''); setResult(null);
    try { setResult(await previewHistoricalFixture(fixture)); } catch (e) { setError(purchaseErrorMessage(e)); } finally { setBusy(false); }
  }
  return <section className="rounded-xl border bg-white p-4 space-y-4 min-w-0" aria-label="Historical data preview">
    <h2 className="text-lg font-semibold">Historical data preview</h2>
    <p className="font-medium text-emerald-800">Preview only — no data will be saved.</p>
    <p>Synthetic examples only. Review source facts, missing values and conflicts. Historical persistence/import remains unavailable pending trusted source-to-target mapping and correction contracts.</p>
    <label className="block">Synthetic fixture<select className="block w-full border rounded p-3 mt-1" disabled={busy} value={fixture} onChange={e => { setFixture(e.target.value as keyof typeof historicalFixtureSuites); setResult(null); setError(''); }}>
      {Object.entries(historicalFixtureSuites).map(([id, title]) => <option key={id} value={id}>{title}</option>)}
    </select></label>
    <button className="px-4 py-3 min-h-12 rounded bg-emerald-800 text-white disabled:opacity-50" disabled={busy} onClick={() => void validate()}>{busy ? 'Validating…' : 'Validate synthetic fixture'}</button>
    {busy && <p role="status">Checking synthetic source facts…</p>}{error && <p role="alert" className="text-red-700">{error}</p>}
    {!visible && !busy && !error && <p>Select an example and validate to review its results.</p>}
    {visible && <div className="space-y-4" aria-live="polite">
      <p role="status">{visible.summary.totalRows} synthetic rows checked. No data saved.</p>
      <dl className="grid grid-cols-2 sm:grid-cols-4 gap-3">{Object.entries({ Valid: visible.summary.validRows, Warnings: visible.summary.warningRows, Rejected: visible.summary.rejectedRows, Ambiguous: visible.summary.ambiguousRows, 'Not found': visible.summary.notFoundRows, 'Out of scope (rejected)': visible.summary.outOfScopeRows, Duplicates: visible.summary.duplicateRows }).map(([label, value]) => <div key={label} className="rounded bg-slate-50 p-3"><dt className="text-sm">{label}</dt><dd className="font-semibold">{value}</dd></div>)}</dl>
      <details className="border rounded p-3"><summary className="min-h-12 cursor-pointer">Values that remain unchanged</summary><ul className="list-disc pl-5">{visible.unchangedAreas.map(area => <li key={area}>{area}</li>)}</ul></details>
      {visible.rows.map((row, index) => <details key={row.case + index} className="border rounded p-3 min-w-0">
        <summary className="min-h-12 cursor-pointer break-words">{row.case} · {row.outcome} · {row.match}{row.duplicate ? ' · Duplicate' : ''}</summary>
        {!!row.reasons.length && <p className="break-words text-red-700">{row.reasons.join(' · ')}</p>}
        <h3 className="font-semibold mt-3">Provenance</h3>{row.provenance ? <dl className="space-y-1 text-sm">{Object.entries(row.provenance).map(([key, value]) => <div key={key} className="break-words"><dt className="inline font-medium">{labels[key] ?? key}: </dt><dd className="inline">{value ?? 'Not supplied'}</dd></div>)}</dl> : <p>Provenance rejected or unavailable.</p>}
        <h3 className="font-semibold mt-3">Proposed historical values</h3><div className="space-y-3 mt-2">{row.fields.map(f => <div key={f.field} className="rounded bg-slate-50 p-3 break-words">
          <p className="font-medium">{fieldLabels[f.field] ?? f.field}</p><p>{f.state} · {f.outcome}</p><p>Proposed: {f.proposedValue ?? 'NULL — no value inferred'}</p><p className="text-sm">{f.reasonCode}: {f.message}</p>{f.originalAllowedValue !== null && <p className="text-sm">Original allowed value: {f.originalAllowedValue} · source cell: {f.sourceCell}</p>}
        </div>)}</div>
        <h3 className="font-semibold mt-3">Unchanged synthetic current values</h3>{Object.keys(row.unchangedCurrentValues).length ? <dl className="text-sm">{Object.entries(row.unchangedCurrentValues).map(([key, value]) => <div key={key} className="break-words"><dt className="inline">{labels[key] ?? key}: </dt><dd className="inline">{value ?? 'NULL'}</dd></div>)}</dl> : <p>No scoped target resolved.</p>}
      </details>)}
      <p className="font-medium">Review ends here. All current values remain unchanged.</p>
    </div>}
  </section>;
}
