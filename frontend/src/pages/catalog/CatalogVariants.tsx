import { useRef, useState } from 'react';
import { useMutation, useQueryClient } from '@tanstack/react-query';
import { catalogApi, type CatalogVariant } from '../../api/catalogApi';
import { catalogKeys } from '../../lib/queryKeys';
import { useAuthStore } from '../../stores/authStore';
import { MAX_PURCHASE_VALUE } from '../../lib/purchaseValidation';
import { Button, Card, ConfirmDialog, Input } from '../../components/ui';

export default function CatalogVariants({ itemId, variants = [] }: { itemId: string; variants?: CatalogVariant[] }) {
  const client = useQueryClient();
  const business = useAuthStore(s => s.user?.currentBusiness);
  const can = (permission: string) => business?.role === 'Owner' || business?.role === 'SuperAdmin' || business?.permissions?.includes(permission);
  const [open, setOpen] = useState(false);
  const [editing, setEditing] = useState<CatalogVariant | null>(null);
  const [deleting, setDeleting] = useState<CatalogVariant | null>(null);
  const [name, setName] = useState('');
  const [weight, setWeight] = useState('');
  const [error, setError] = useState('');
  const busy = useRef(false);
  const refresh = () => client.invalidateQueries({ queryKey: catalogKeys.detail(itemId) });
  const message = (err: unknown) => {
    const normalized = (err as { normalized?: string | { message?: string } })?.normalized;
    return typeof normalized === 'string' ? normalized : normalized?.message || 'Could not save the variant. Check your connection and try again.';
  };
  const save = useMutation({
    mutationFn: (input: { name: string; kgPerUnit: number | null }) => editing
      ? catalogApi.updateVariant(itemId, { ...editing, ...input }) : catalogApi.createVariant(itemId, input),
    onSuccess: () => { setOpen(false); setEditing(null); setError(''); void refresh(); },
    onError: err => setError(message(err)),
    onSettled: () => { busy.current = false; }
  });
  const remove = useMutation({
    mutationFn: (variant: CatalogVariant) => catalogApi.deleteVariant(itemId, variant),
    onSuccess: () => { setDeleting(null); setError(''); void refresh(); },
    onError: err => { setDeleting(null); setError(message(err)); },
    onSettled: () => { busy.current = false; }
  });
  const start = (variant: CatalogVariant | null) => {
    setEditing(variant); setName(variant?.name || ''); setWeight(variant?.kgPerUnit?.toString() || ''); setError(''); setOpen(true);
  };
  return <Card className="p-6">
    <div className="flex flex-wrap items-center justify-between gap-3 border-b pb-2 mb-4">
      <h3 className="text-lg font-medium text-[#0F172A]">Variants</h3>
      {can('catalog.create') && !open && <Button variant="secondary" onClick={() => start(null)}>Add Variant</Button>}
    </div>
    {error && <div role="alert" className="mb-4 text-sm text-red-700 break-words">{error}
      <Button variant="ghost" disabled={save.isPending || remove.isPending} onClick={() => { setOpen(false); setEditing(null); setError(''); void refresh(); }}>Reload variants</Button>
    </div>}
    {open && <form className="mb-5 space-y-4" onSubmit={event => {
      event.preventDefault(); if (busy.current) return;
      const kgPerUnit = weight.trim() ? Number(weight) : null;
      if (!name.trim() || name.trim().length > 150 || (kgPerUnit !== null && (!Number.isFinite(kgPerUnit) || kgPerUnit <= 0 || kgPerUnit > MAX_PURCHASE_VALUE || Math.abs(kgPerUnit * 10000 - Math.round(kgPerUnit * 10000)) > 0.000001))) {
        setError('Enter a name and an optional positive weight with at most four decimal places.'); return;
      }
      busy.current = true; setError(''); save.mutate({ name: name.trim(), kgPerUnit });
    }}>
      <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
        <Input label="Variant name" value={name} maxLength={512} required disabled={save.isPending} onChange={e => setName(e.target.value)} />
        <Input label="Variant kg per unit" type="number" min="0.0001" max={MAX_PURCHASE_VALUE} step="0.0001" value={weight} disabled={save.isPending} onChange={e => setWeight(e.target.value)} hint="Optional default weight" />
      </div>
      <div className="flex flex-wrap gap-2">
        <Button type="submit" loading={save.isPending}>Save Variant</Button>
        <Button type="button" variant="ghost" disabled={save.isPending} onClick={() => { setOpen(false); setError(''); }}>Cancel</Button>
      </div>
    </form>}
    {variants.length === 0 ? <p className="py-4 text-sm text-gray-500">No variants yet.</p> : <ul className="divide-y">
      {variants.map(variant => <li key={variant.id} className="py-3 flex flex-wrap items-center justify-between gap-3">
        <div className="min-w-0 break-words"><p className="text-sm font-medium">{variant.name}</p>
          <p className="text-xs text-gray-500">{variant.kgPerUnit != null ? `${variant.kgPerUnit} kg per unit` : 'No default weight'}</p></div>
        <div className="flex flex-wrap gap-2">
          {can('catalog.edit') && <Button variant="ghost" disabled={open || remove.isPending} aria-label={`Edit variant ${variant.name}`} onClick={() => start(variant)}>Edit</Button>}
          {business?.role === 'Owner' && <Button variant="danger" disabled={open || remove.isPending} aria-label={`Delete variant ${variant.name}`} onClick={() => setDeleting(variant)}>Delete</Button>}
        </div>
      </li>)}
    </ul>}
    <ConfirmDialog open={!!deleting} title={`Delete ${deleting?.name || 'variant'}?`} description="This removes the variant from this item." confirmLabel="Delete Variant"
      loading={remove.isPending} onCancel={() => setDeleting(null)} onConfirm={() => { if (deleting && !busy.current) { busy.current = true; remove.mutate(deleting); } }} />
  </Card>;
}
