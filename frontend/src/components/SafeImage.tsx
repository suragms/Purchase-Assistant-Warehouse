import { useEffect, useState } from 'react';
import apiClient from '../api/apiClient';
import { useAuthStore } from '../stores/authStore';

type ImageLoadState = { key: string; url?: string; failed?: boolean; loaded?: boolean };

export function SafeImage({ src, alt, authenticated = false }: { src?: string | null; alt: string; authenticated?: boolean }) {
  const scope = useAuthStore(s => `${s.user?.id}:${s.user?.currentBusiness?.businessId}`);
  const [attempt, setAttempt] = useState(0);
  const requestKey = `${src ?? ''}:${authenticated}:${scope}:${attempt}`;
  const [loadState, setLoadState] = useState<ImageLoadState>({ key: '' });
  const current = loadState.key === requestKey ? loadState : undefined;

  useEffect(() => {
    if (!src || !authenticated) return;
    let disposed = false;
    let objectUrl = '';
    void apiClient.get(src, { responseType: 'blob' })
      .then(response => {
        if (disposed) return;
        objectUrl = URL.createObjectURL(response.data);
        setLoadState({ key: requestKey, url: objectUrl });
      })
      .catch(() => { if (!disposed) setLoadState({ key: requestKey, failed: true }); });
    return () => { disposed = true; if (objectUrl) URL.revokeObjectURL(objectUrl); };
  }, [src, authenticated, scope, attempt, requestKey]);

  if (!src) return <span className="text-sm text-slate-500">No image</span>;

  let publicUrl: string | undefined;
  let unsafePublicUrl = false;
  if (!authenticated) {
    try {
      const parsed = new URL(src);
      if (parsed.protocol === 'https:' && !parsed.username && !parsed.password) publicUrl = src;
      else unsafePublicUrl = true;
    } catch { unsafePublicUrl = true; }
  }

  const failed = unsafePublicUrl || current?.failed;
  const url = authenticated ? current?.url : publicUrl;
  const loaded = current?.loaded ?? false;
  if (failed) return <div className="text-sm"><span>Image unavailable. </span><button type="button" className="underline min-h-12" onClick={() => setAttempt(value => value + 1)}>Retry image</button></div>;

  return <div className="max-w-full">{!loaded && <span role="status">Loading image…</span>}{url && <img src={url} alt={alt} referrerPolicy="no-referrer" onLoad={() => setLoadState({ key: requestKey, url, loaded: true })} onError={() => setLoadState({ key: requestKey, failed: true })} className={`max-w-full max-h-48 object-contain rounded ${loaded ? '' : 'hidden'}`} />}</div>;
}
