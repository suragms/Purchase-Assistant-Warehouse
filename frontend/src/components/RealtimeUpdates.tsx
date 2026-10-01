import { useEffect } from 'react';
import { HubConnectionBuilder, LogLevel } from '@microsoft/signalr';
import { useQueryClient } from '@tanstack/react-query';
import { useAuthStore } from '../stores/authStore';
import apiClient from '../api/apiClient';
export function RealtimeUpdates() {
 const cache = useQueryClient(); const user = useAuthStore(s => s.user); const token = useAuthStore(s => s.accessToken); const business = user?.currentBusiness?.businessId;
 useEffect(() => {
  if (!business || !token) return;
  let stopped = false; let retry: ReturnType<typeof setTimeout> | undefined;
  const connection = new HubConnectionBuilder().withUrl(`${apiClient.defaults.baseURL}/realtime`, { accessTokenFactory: () => useAuthStore.getState().accessToken ?? '' }).withAutomaticReconnect([0, 2000, 10000, 30000]).configureLogging(LogLevel.None).build();
  const seen = new Set<string>();
  connection.on('businessEvent', (event: { id: string; type: string; businessId: string }) => {
   if (stopped || event.businessId !== business || seen.has(event.id)) return;
   seen.add(event.id); if (seen.size > 200) seen.delete(seen.values().next().value!);
   const keys = event.type.startsWith('stock.') ? ['stock', 'catalog', 'dashboard', 'reports', 'operations'] : event.type === 'purchase.changed' ? ['purchases', 'dashboard', 'reports'] : event.type === 'notification.changed' ? ['notifications', 'damage-reports'] : [];
   for (const key of keys) void cache.invalidateQueries({ queryKey: [key] });
  });
  connection.onreconnected(() => { for (const key of ['stock', 'purchases', 'notifications', 'dashboard']) void cache.invalidateQueries({ queryKey: [key] }); });
  const start = async () => { try { await connection.start(); if (stopped) await connection.stop(); } catch { if (!stopped) retry = setTimeout(() => void start(), 30000); } };
  connection.onclose(() => { if (!stopped) retry = setTimeout(() => void start(), 30000); });
  void start();
  return () => { stopped = true; clearTimeout(retry); void connection.stop(); };
 }, [business, user?.id, token, cache]);
 return null;
}
