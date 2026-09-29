import React, { useState } from 'react';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { Plus, Edit, Trash2 } from 'lucide-react';
import { catalogApi, type Supplier } from '../../api/catalogApi';
import { supplierKeys } from '../../lib/queryKeys';
import { PageHeader, Button, Card, Skeleton, ErrorState, ConfirmDialog, Modal, Input, Textarea, Badge } from '../../components/ui';
import { useToast } from '../../components/ui/ToastProvider';

export default function SupplierList() {
  const queryClient = useQueryClient();
  const { showToast } = useToast();

  const [modalOpen, setModalOpen] = useState(false);
  const [editingSupplier, setEditingSupplier] = useState<Supplier | null>(null);

  // Form State
  const [name, setName] = useState('');
  const [phone, setPhone] = useState('');
  const [address, setAddress] = useState('');
  const [notes, setNotes] = useState('');
  const [isActive, setIsActive] = useState(true);
  const [errorMsg, setErrorMsg] = useState('');

  const [deleteOpen, setDeleteOpen] = useState(false);
  const [supplierIdToDelete, setSupplierIdToDelete] = useState<string | null>(null);

  const { data, isLoading, error, refetch } = useQuery({
    queryKey: supplierKeys.lists(),
    queryFn: () => catalogApi.getSuppliers(),
  });

  const saveMutation = useMutation({
    mutationFn: (payload: Partial<Supplier>) => {
      if (editingSupplier) return catalogApi.updateSupplier(editingSupplier.id, payload);
      return catalogApi.createSupplier(payload);
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: supplierKeys.lists() });
      queryClient.invalidateQueries({ queryKey: ['search'] });
      showToast(`Supplier ${editingSupplier ? 'updated' : 'created'}`);
      handleClose();
    },
    onError: (err: any) => {
      if (err.normalized === 'SUPPLIER_EXISTS') {
        setErrorMsg('Supplier name already exists.');
      } else {
        setErrorMsg('Failed to save supplier.');
      }
    }
  });

  const deleteMutation = useMutation({
    mutationFn: (id: string) => catalogApi.deleteSupplier(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: supplierKeys.lists() });
      queryClient.invalidateQueries({ queryKey: ['search'] });
      showToast('Supplier deleted');
      setDeleteOpen(false);
    },
    onError: (err: any) => {
      showToast(err.normalized?.message || err.normalized === 'SUPPLIER_IN_USE' ? 'Cannot delete supplier that is in use.' : 'Failed to delete', 'error');
      setDeleteOpen(false);
    }
  });

  const handleOpenNew = () => {
    setEditingSupplier(null);
    setName('');
    setPhone('');
    setAddress('');
    setNotes('');
    setIsActive(true);
    setErrorMsg('');
    setModalOpen(true);
  };

  const handleOpenEdit = (s: Supplier) => {
    setEditingSupplier(s);
    setName(s.name);
    setPhone(s.phone || '');
    setAddress(s.address || '');
    setNotes(s.notes || '');
    setIsActive(s.isActive);
    setErrorMsg('');
    setModalOpen(true);
  };

  const handleClose = () => setModalOpen(false);

  const handleSave = (e: React.FormEvent) => {
    e.preventDefault();
    if (!name.trim()) {
      setErrorMsg('Name is required');
      return;
    }
    saveMutation.mutate({ name, phone, address, notes, isActive });
  };

  return (
    <div>
      <PageHeader
        title="Suppliers"
        subtitle="Manage product suppliers"
        actions={
          <Button icon={<Plus className="h-4 w-4" />} onClick={handleOpenNew}>New Supplier</Button>
        }
      />

      <Card className="overflow-hidden">
        {isLoading ? (
          <div className="p-4 space-y-4">
            {[1, 2, 3].map(i => <Skeleton key={i} className="h-12 w-full" />)}
          </div>
        ) : error ? (
          <ErrorState onRetry={() => refetch()} />
        ) : (
          <table className="min-w-full text-sm text-left">
            <thead className="bg-gray-50 text-[#475569] font-medium border-b border-[#E2E8E6]">
              <tr>
                <th className="px-4 py-3">Supplier Name</th>
                <th className="px-4 py-3">Phone</th>
                <th className="px-4 py-3 text-right">Linked Items</th>
                <th className="px-4 py-3">Status</th>
                <th className="px-4 py-3 text-right">Actions</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-[#E2E8E6]">
              {data?.map(s => (
                <tr key={s.id} className="hover:bg-gray-50">
                  <td className="px-4 py-3 font-medium text-[#0F172A]">{s.name}</td>
                  <td className="px-4 py-3 text-gray-500">{s.phone || '—'}</td>
                  <td className="px-4 py-3 text-right text-gray-500">{s.linkedItemsCount}</td>
                  <td className="px-4 py-3">
                    {s.isActive ? <Badge variant="green">Active</Badge> : <Badge variant="gray">Inactive</Badge>}
                  </td>
                  <td className="px-4 py-3 text-right space-x-2">
                    <Button variant="ghost" size="sm" onClick={() => handleOpenEdit(s)}>
                      <Edit className="h-4 w-4" />
                    </Button>
                    <Button variant="ghost" size="sm" className="text-red-600 hover:text-red-700 hover:bg-red-50" onClick={() => { setSupplierIdToDelete(s.id); setDeleteOpen(true); }}>
                      <Trash2 className="h-4 w-4" />
                    </Button>
                  </td>
                </tr>
              ))}
              {data?.length === 0 && (
                <tr>
                  <td colSpan={5} className="px-4 py-8 text-center text-gray-500">
                    No suppliers found.
                  </td>
                </tr>
              )}
            </tbody>
          </table>
        )}
      </Card>

      <Modal
        open={modalOpen}
        onClose={handleClose}
        title={editingSupplier ? "Edit Supplier" : "New Supplier"}
      >
        <form onSubmit={handleSave} className="space-y-4">
          {errorMsg && <div className="text-sm text-red-600">{errorMsg}</div>}

          <Input
            label="Supplier Name"
            value={name}
            onChange={e => setName(e.target.value)}
            required
          />
          <Input
            label="Phone Number"
            value={phone}
            onChange={e => setPhone(e.target.value)}
          />
          <Textarea
            label="Address"
            value={address}
            onChange={e => setAddress(e.target.value)}
          />
          <Textarea
            label="Notes"
            value={notes}
            onChange={e => setNotes(e.target.value)}
          />
          <div className="flex items-center gap-2 mt-4">
            <input
              type="checkbox"
              id="isActiveSupplier"
              checked={isActive}
              onChange={e => setIsActive(e.target.checked)}
              className="h-4 w-4 rounded border-gray-300 text-[#159A8A]"
            />
            <label htmlFor="isActiveSupplier" className="text-sm font-medium">Active</label>
          </div>

          <div className="flex justify-end gap-3 pt-4 border-t">
            <Button type="button" variant="ghost" onClick={handleClose}>Cancel</Button>
            <Button type="submit" loading={saveMutation.isPending}>Save</Button>
          </div>
        </form>
      </Modal>

      <ConfirmDialog
        open={deleteOpen}
        title="Delete Supplier"
        description="Are you sure you want to delete this supplier? This action cannot be undone."
        confirmLabel="Delete"
        loading={deleteMutation.isPending}
        onConfirm={() => deleteMutation.mutate(supplierIdToDelete!)}
        onCancel={() => setDeleteOpen(false)}
      />
    </div>
  );
}
