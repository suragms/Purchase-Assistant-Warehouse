import { useState } from 'react';
import apiClient from '../api/apiClient';
import { purchaseErrorMessage } from '../lib/purchaseValidation';
import { CsvExportButton } from './CsvExportButton';
export function ExportControls({ start, end }: { start: string; end: string }) {
 const [busy, setBusy] = useState(''); const [error, setError] = useState('');
 async function download(file: string) { if (busy) return; setBusy(file); setError(''); try { const result = await apiClient.get(`/exports/${file}`, { params: { start, end }, responseType: 'blob' }); const url = URL.createObjectURL(result.data); const link = document.createElement('a'); link.href = url; link.download = file; link.click(); setTimeout(() => URL.revokeObjectURL(url), 10000); } catch (e) { setError(purchaseErrorMessage(e)); } finally { setBusy(''); } }
 return <div className="space-y-2"><div className="flex flex-wrap gap-2">{[['stock.xlsx', 'Stock XLSX'], ['purchases.pdf', 'Purchase PDF'], ['backup.zip', 'ZIP backup'], ['backup.json', 'JSON backup']].map(([file, label]) => <button key={file} disabled={!!busy} onClick={() => void download(file)} className="border rounded px-3 py-3 text-sm bg-white">{busy === file ? 'Preparing…' : label}</button>)}<CsvExportButton kind="report-suppliers" label="Supplier report CSV" params={{ start, end }} /><CsvExportButton kind="report-items" label="Item report CSV" params={{ start, end }} /></div>{error && <p role="alert" className="text-red-700">{error}</p>}</div>;
}
