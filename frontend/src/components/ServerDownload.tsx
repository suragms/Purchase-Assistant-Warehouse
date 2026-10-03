import { useState } from 'react';
import apiClient from '../api/apiClient';
import { backupDeviceKey, canExport } from '../api/exportsApi';
export function ServerDownload({ path, filename, label, params }: { path: string; filename: string; label: string; params?: Record<string, unknown> }) {
  const [busy, setBusy] = useState(false), [error, setError] = useState('');
  if (!canExport()) return null;
  return <span className="inline-flex flex-col gap-1"><button className="border rounded px-3 py-3 bg-white disabled:opacity-50" disabled={busy} onClick={async () => {
    const scope = backupDeviceKey(); setBusy(true); setError('');
    try {
      const result = await apiClient.get<Blob>(path, { params, responseType: 'blob' });
      if (scope !== backupDeviceKey() || !canExport()) throw new Error();
      const url = URL.createObjectURL(result.data), link = document.createElement('a'); link.href = url; link.download = filename; link.click(); setTimeout(() => URL.revokeObjectURL(url), 1000);
    } catch { setError('Download failed. Check your filters and permissions, then retry.'); } finally { setBusy(false); }
  }}>{busy ? 'Preparing…' : label}</button>{error && <span role="alert" className="text-sm text-red-700">{error}</span>}</span>;
}
