const CACHE = 'purchase-assistant-shell-v3';
const PUBLIC_FILES = ['/offline.html', '/manifest.webmanifest', '/favicon.png', '/icon-192.png', '/icon-512.png'];
self.addEventListener('install', event => event.waitUntil(caches.open(CACHE).then(cache => cache.addAll(PUBLIC_FILES))));
self.addEventListener('activate', event => event.waitUntil(caches.keys().then(keys => Promise.all(keys.filter(key => key.startsWith('purchase-assistant-shell-') && key !== CACHE).map(key => caches.delete(key)))).then(() => self.clients.claim())));
self.addEventListener('message', event => { if (event.data === 'ACTIVATE_UPDATE') self.skipWaiting(); });
self.addEventListener('fetch', event => {
 const url = new URL(event.request.url);
 if (event.request.method !== 'GET' || url.origin !== self.location.origin || url.pathname.startsWith('/api/')) return;
 if (event.request.mode === 'navigate') event.respondWith(fetch(event.request).catch(() => caches.match('/offline.html')));
 else if (PUBLIC_FILES.includes(url.pathname)) event.respondWith(caches.match(event.request).then(cached => cached || fetch(event.request)));
});
