import { useEffect, useState } from 'react';
export function PwaUpdate() {
 const [waiting, setWaiting] = useState<ServiceWorker | null>(null);
 useEffect(() => {
  if (!import.meta.env.PROD || !('serviceWorker' in navigator)) return;
  let disposed = false;
  void navigator.serviceWorker.register('/sw.js').then(registration => {
   if (disposed) return;
   if (registration.waiting) setWaiting(registration.waiting);
   registration.addEventListener('updatefound', () => { const installing = registration.installing; installing?.addEventListener('statechange', () => { if (!disposed && installing.state === 'installed' && navigator.serviceWorker.controller) setWaiting(installing); }); });
  }).catch(() => { /* Online application remains available if the browser disables installation. */ });
  return () => { disposed = true; };
 }, []);
 if (!waiting) return null;
 return <div role="status" className="fixed bottom-4 left-4 right-4 z-50 p-4 rounded border bg-white shadow-lg">An update is available. Save your work before reloading. <button className="underline p-3" onClick={() => { navigator.serviceWorker.addEventListener('controllerchange', () => window.location.reload(), { once: true }); waiting.postMessage('ACTIVATE_UPDATE'); }}>Reload to update</button></div>;
}
