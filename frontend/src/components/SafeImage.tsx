import { useEffect, useState } from 'react';
import apiClient from '../api/apiClient';
import { useAuthStore } from '../stores/authStore';
export function SafeImage({ src, alt, authenticated = false }: { src?: string | null; alt: string; authenticated?: boolean }) {
  const scope = useAuthStore(s => `${s.user?.id}:${s.user?.currentBusiness?.businessId}`);
  const [url, setUrl] = useState(''); const [failed, setFailed] = useState(false); const [loaded, setLoaded] = useState(false); const [attempt, setAttempt] = useState(0);
  useEffect(() => {
    let disposed = false; let objectUrl = ''; setUrl(''); setFailed(false); setLoaded(false);
    if (!src) return;
    if (authenticated) {
      void apiClient.get(src, { responseType: 'blob' }).then(r => { if (!disposed) { objectUrl = URL.createObjectURL(r.data); setUrl(objectUrl); } }).catch(() => { if (!disposed) setFailed(true); });
    } else {
      try { const parsed = new URL(src); if (parsed.protocol !== 'https:' || parsed.username || parsed.password) throw new Error(); setUrl(src); } catch { setFailed(true); }
    }
    return () => { disposed = true; if (objectUrl) URL.revokeObjectURL(objectUrl); };
  }, [src, authenticated, scope, attempt]);
  if (!src) return <span className="text-sm text-slate-500">No image</span>;
  if (failed) return <div className="text-sm"><span>Image unavailable. </span><button type="button" className="underline min-h-12" onClick={() => setAttempt(v => v + 1)}>Retry image</button></div>;
  return <div className="max-w-full">{!loaded && <span role="status">Loading image…</span>}{url && <img src={url} alt={alt} referrerPolicy="no-referrer" onLoad={() => setLoaded(true)} onError={() => setFailed(true)} className={`max-w-full max-h-48 object-contain rounded ${loaded ? '' : 'hidden'}`} />}</div>;
}
