import { useEffect, useState } from 'react';
import { Link, useLocation } from 'react-router-dom';
import { useAuthStore } from '../stores/authStore';
import { backupDeviceKey, dailyAutoBackup } from '../api/exportsApi';
export function BackupReminder() {
  const user = useAuthStore(s => s.user); const key = backupDeviceKey(); const location = useLocation();
  const [month] = useState(() => { const now = new Date(); return `${now.getFullYear()}-${String(now.getMonth() + 1).padStart(2, '0')}`; });
  const [dismissed, setDismissed] = useState(() => { try { return localStorage.getItem(key + ':dismissed-month') === month; } catch { return false; } });
  const [error, setError] = useState('');
  useEffect(() => {
    let stopped = false;
    try {
      void dailyAutoBackup().catch(() => { if (!stopped) setError('Automatic backup download failed. Retry from Export & Backup.'); });
    } catch { /* Manual downloads remain available. */ }
    return () => { stopped = true; };
  }, [key, month]);
  const owner = location.pathname === '/settings' && ['Owner', 'SuperAdmin'].includes(user?.currentBusiness?.role ?? '');
  return <>{error && <p role="alert" className="p-3 text-red-700">{error} <Link className="underline" to="/settings/backup">Export & Backup</Link></p>}{owner && !dismissed && <aside className="rounded-lg border bg-emerald-50 p-3 mb-4 flex items-start gap-3"><div className="min-w-0 flex-1"><p className="font-semibold">Back up your data</p><p className="text-sm">Download stock and purchase reports to keep a local copy.</p><Link className="underline inline-block py-2" to="/settings/backup">Export & Backup</Link></div><button className="p-3 min-h-12" aria-label="Dismiss until next month" onClick={() => { try { localStorage.setItem(key + ':dismissed-month', month); } catch { /* Dismiss this session. */ } setDismissed(true); }}>×</button></aside>}</>;
}
