import { useState } from 'react';
import { useQuery } from '@tanstack/react-query';
import { Link } from 'react-router-dom';
import apiClient from '../api/apiClient';
import { formatMoney } from '../lib/formatMoney';

export function SupplierHistory({ id }: { id: string }) {
  const [page, setPage] = useState(1); const [from, setFrom] = useState(''); const [to, setTo] = useState('');
  const params = { page, from: from ? `${from}T00:00:00Z` : undefined, to: to ? `${to}T23:59:59.999Z` : undefined };
  const query = useQuery({ queryKey: ['supplier-history', id, params], queryFn: async () => (await apiClient.get<{ items: { id: string; orderNumber: string; createdAt: string; status: string; grandTotal?: number; paidAmount?: number }[]; totalCount: number; totalSpend?: number; totalPaid?: number }>(`/catalog/suppliers/${id}/history`, { params })).data });
  return <div className="space-y-3"><div className="grid sm:grid-cols-2 gap-3"><label>From (UTC)<input className="block border p-2 w-full" type="date" value={from} onChange={e => { setFrom(e.target.value); setPage(1); }} /></label><label>To (UTC)<input className="block border p-2 w-full" type="date" value={to} onChange={e => { setTo(e.target.value); setPage(1); }} /></label></div>
    {query.isPending && <p role="status">Loading supplier purchases…</p>}{query.isError && <p role="alert">History unavailable. <button className="underline" onClick={() => void query.refetch()}>Retry</button></p>}
    {query.data && <><p>{query.data.totalCount} purchases (draft and cancelled orders excluded)</p>{query.data.totalSpend !== undefined && <p>Total {formatMoney(query.data.totalSpend)} · Paid {formatMoney(query.data.totalPaid)}</p>}{query.data.items.length === 0 && <p>No purchases in this range.</p>}{query.data.items.map(p => <div className="border-t pt-2" key={p.id}><Link className="underline" to={`/purchases/${p.id}`}>{p.orderNumber}</Link><p>{p.createdAt.slice(0, 10)} · {p.status}{p.grandTotal !== undefined && ` · ${formatMoney(p.grandTotal)}`}</p></div>)}<div className="flex gap-3"><button disabled={page === 1} onClick={() => setPage(p => p - 1)}>Previous</button><span>{page}</span><button disabled={page * 50 >= query.data.totalCount} onClick={() => setPage(p => p + 1)}>Next</button></div></>}
  </div>;
}
