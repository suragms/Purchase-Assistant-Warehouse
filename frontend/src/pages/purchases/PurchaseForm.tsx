import { useState, useEffect } from 'react';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { useNavigate, useParams } from 'react-router-dom';
import { purchaseApi, type UpsertPurchaseOrderDto } from '../../api/purchaseApi';
import { supplierApi } from '../../api/supplierApi';
import { brokerApi } from '../../api/brokerApi';
import { catalogApi } from '../../api/catalogApi';
import { purchaseKeys } from '../../lib/queryKeys';
import { ShoppingBag, Plus, Trash2, ArrowLeft, Save } from 'lucide-react';
import { PurchaseAssistant } from '../../components/AI/PurchaseAssistant';
import type { PurchaseIntentItemCandidateDto } from '../../api/purchaseIntentApi';

interface PurchaseFormProps {
  edit?: boolean;
}

export default function PurchaseForm({ edit = false }: PurchaseFormProps) {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const queryClient = useQueryClient();

  const [supplierId, setSupplierId] = useState('');
  const [brokerId, setBrokerId] = useState('');
  const [notes, setNotes] = useState('');
  const [taxTotal, setTaxTotal] = useState<number>(0);
  const [items, setItems] = useState<Array<{ catalogItemId: string; orderedQuantity: number; unitPrice: number; notes?: string }>>([
    { catalogItemId: '', orderedQuantity: 1, unitPrice: 0, notes: '' }
  ]);

  const { data: suppliersData } = useQuery({
    queryKey: ['suppliers', 'list'],
    queryFn: () => supplierApi.getSuppliers(1, 100),
  });

  const { data: brokersData } = useQuery({
    queryKey: ['brokers', 'list'],
    queryFn: () => brokerApi.getBrokers(1, 100),
  });

  const { data: catalogData } = useQuery({
    queryKey: ['catalog', 'list'],
    queryFn: () => catalogApi.getItems(1, 200),
  });

  const { data: existingOrder } = useQuery({
    queryKey: purchaseKeys.detail(id!),
    queryFn: () => purchaseApi.getPurchaseById(id!),
    enabled: edit && !!id,
  });

  useEffect(() => {
    if (edit && existingOrder) {
      setSupplierId(existingOrder.supplierId);
      setBrokerId(existingOrder.brokerId || '');
      setNotes(existingOrder.notes || '');
      setTaxTotal(existingOrder.taxTotal);
      setItems(existingOrder.items.map(i => ({
        catalogItemId: i.catalogItemId,
        orderedQuantity: i.orderedQuantity,
        unitPrice: i.unitPrice,
        notes: i.notes || ''
      })));
    }
  }, [edit, existingOrder]);

  const handleDraftConfirmed = (candidateItems: PurchaseIntentItemCandidateDto[], inferredSupplierId?: string) => {
    setItems(candidateItems.map(i => ({
      catalogItemId: i.catalogItemId || '',
      orderedQuantity: i.requestedQuantity,
      unitPrice: 0,
      notes: ''
    })));
    if (inferredSupplierId) setSupplierId(inferredSupplierId);
  };

  const createMutation = useMutation({
    mutationFn: (dto: UpsertPurchaseOrderDto) => purchaseApi.createPurchase(dto),
    onSuccess: (res) => {
      queryClient.invalidateQueries({ queryKey: purchaseKeys.all });
      navigate(`/purchases/${res.id}`);
    },
  });

  const updateMutation = useMutation({
    mutationFn: (dto: UpsertPurchaseOrderDto) => purchaseApi.updatePurchase(id!, dto),
    onSuccess: (res) => {
      queryClient.invalidateQueries({ queryKey: purchaseKeys.all });
      navigate(`/purchases/${res.id}`);
    },
  });

  const subtotal = items.reduce((acc, item) => acc + (item.orderedQuantity * item.unitPrice), 0);
  const grandTotal = subtotal + Number(taxTotal || 0);

  const handleAddItem = () => {
    setItems([...items, { catalogItemId: '', orderedQuantity: 1, unitPrice: 0, notes: '' }]);
  };

  const handleRemoveItem = (index: number) => {
    if (items.length === 1) return;
    setItems(items.filter((_, i) => i !== index));
  };

  const handleItemChange = (index: number, field: string, value: any) => {
    const updated = [...items];
    updated[index] = { ...updated[index], [field]: value };
    setItems(updated);
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!supplierId) {
      alert('Please select a supplier.');
      return;
    }
    if (items.length === 0 || items.some(i => !i.catalogItemId || i.orderedQuantity <= 0)) {
      alert('Please ensure all items have a catalog item and valid ordered quantity.');
      return;
    }

    const dto: UpsertPurchaseOrderDto = {
      supplierId,
      brokerId: brokerId || undefined,
      notes: notes || undefined,
      taxTotal: Number(taxTotal),
      items: items.map(i => ({
        catalogItemId: i.catalogItemId,
        orderedQuantity: Number(i.orderedQuantity),
        unitPrice: Number(i.unitPrice),
        notes: i.notes || undefined
      }))
    };

    if (edit && id) {
      await updateMutation.mutateAsync(dto);
    } else {
      await createMutation.mutateAsync(dto);
    }
  };

  return (
    <div className="max-w-4xl mx-auto space-y-6">
      <div className="flex items-center gap-4">
        <button
          onClick={() => navigate(-1)}
          className="p-2 text-slate-600 hover:text-indigo-600 hover:bg-white rounded-lg border border-slate-200 shadow-sm transition-colors"
        >
          <ArrowLeft className="w-5 h-5" />
        </button>
        <div>
          <h1 className="text-2xl font-bold text-slate-900 tracking-tight flex items-center gap-2">
            <ShoppingBag className="w-7 h-7 text-indigo-600" />
            {edit ? 'Edit Purchase Order' : 'Create Purchase Order'}
          </h1>
          <p className="text-sm text-slate-500 mt-1">
            {edit ? 'Modify draft order details and line items.' : 'Draft a new purchase order for a supplier.'}
          </p>
        </div>
      </div>

      <PurchaseAssistant onDraftConfirmed={handleDraftConfirmed} />

      <form onSubmit={handleSubmit} className="space-y-6">
        {/* Main Details Card */}
        <div className="bg-white p-6 rounded-xl shadow-sm border border-slate-200 grid grid-cols-1 md:grid-cols-2 gap-6">
          <div>
            <label className="block text-sm font-medium text-slate-700 mb-1">Supplier *</label>
            <select
              required
              value={supplierId}
              onChange={(e) => setSupplierId(e.target.value)}
              className="w-full px-3 py-2 text-sm bg-slate-50 border border-slate-200 rounded-lg focus:outline-none focus:ring-2 focus:ring-indigo-500"
            >
              <option value="">Select Supplier...</option>
              {suppliersData?.data?.map(s => (
                <option key={s.id} value={s.id}>{s.name}</option>
              ))}
            </select>
          </div>

          <div>
            <label className="block text-sm font-medium text-slate-700 mb-1">Broker (Optional)</label>
            <select
              value={brokerId}
              onChange={(e) => setBrokerId(e.target.value)}
              className="w-full px-3 py-2 text-sm bg-slate-50 border border-slate-200 rounded-lg focus:outline-none focus:ring-2 focus:ring-indigo-500"
            >
              <option value="">Select Broker...</option>
              {brokersData?.data?.map(b => (
                <option key={b.id} value={b.id}>{b.name}</option>
              ))}
            </select>
          </div>

          <div className="md:col-span-2">
            <label className="block text-sm font-medium text-slate-700 mb-1">Notes / Terms</label>
            <textarea
              rows={2}
              value={notes}
              onChange={(e) => setNotes(e.target.value)}
              placeholder="Delivery instructions, payment terms, etc."
              className="w-full px-3 py-2 text-sm bg-slate-50 border border-slate-200 rounded-lg focus:outline-none focus:ring-2 focus:ring-indigo-500"
            />
          </div>
        </div>

        {/* Line Items Card */}
        <div className="bg-white p-6 rounded-xl shadow-sm border border-slate-200 space-y-4">
          <div className="flex items-center justify-between">
            <h2 className="text-lg font-semibold text-slate-900">Purchase Items</h2>
            <button
              type="button"
              onClick={handleAddItem}
              className="inline-flex items-center gap-1.5 px-3 py-1.5 bg-indigo-50 hover:bg-indigo-100 text-indigo-700 font-medium text-sm rounded-lg transition-colors"
            >
              <Plus className="w-4 h-4" /> Add Item
            </button>
          </div>

          <div className="space-y-3">
            {items.map((item, index) => (
              <div key={index} className="p-4 bg-slate-50 border border-slate-200 rounded-xl grid grid-cols-1 md:grid-cols-12 gap-3 items-end">
                <div className="md:col-span-5">
                  <label className="block text-xs font-medium text-slate-600 mb-1">Catalog Item *</label>
                  <select
                    required
                    value={item.catalogItemId}
                    onChange={(e) => handleItemChange(index, 'catalogItemId', e.target.value)}
                    className="w-full px-3 py-2 text-sm bg-white border border-slate-200 rounded-lg focus:outline-none focus:ring-2 focus:ring-indigo-500"
                  >
                    <option value="">Select item...</option>
                    {catalogData?.data?.map(c => (
                      <option key={c.id} value={c.id}>{c.itemCode} - {c.name}</option>
                    ))}
                  </select>
                </div>

                <div className="md:col-span-2">
                  <label className="block text-xs font-medium text-slate-600 mb-1">Qty *</label>
                  <input
                    type="number"
                    min="0.0001"
                    step="any"
                    required
                    value={item.orderedQuantity}
                    onChange={(e) => handleItemChange(index, 'orderedQuantity', parseFloat(e.target.value) || 0)}
                    className="w-full px-3 py-2 text-sm bg-white border border-slate-200 rounded-lg focus:outline-none focus:ring-2 focus:ring-indigo-500"
                  />
                </div>

                <div className="md:col-span-2">
                  <label className="block text-xs font-medium text-slate-600 mb-1">Unit Price ($) *</label>
                  <input
                    type="number"
                    min="0"
                    step="0.01"
                    required
                    value={item.unitPrice}
                    onChange={(e) => handleItemChange(index, 'unitPrice', parseFloat(e.target.value) || 0)}
                    className="w-full px-3 py-2 text-sm bg-white border border-slate-200 rounded-lg focus:outline-none focus:ring-2 focus:ring-indigo-500"
                  />
                </div>

                <div className="md:col-span-2">
                  <label className="block text-xs font-medium text-slate-600 mb-1">Line Total</label>
                  <div className="px-3 py-2 text-sm font-semibold text-slate-900 bg-white border border-slate-200 rounded-lg">
                    ${(item.orderedQuantity * item.unitPrice).toFixed(2)}
                  </div>
                </div>

                <div className="md:col-span-1 flex justify-end">
                  <button
                    type="button"
                    disabled={items.length === 1}
                    onClick={() => handleRemoveItem(index)}
                    className="p-2 text-slate-400 hover:text-rose-600 disabled:opacity-30 transition-colors"
                  >
                    <Trash2 className="w-5 h-5" />
                  </button>
                </div>
              </div>
            ))}
          </div>

          {/* Totals Calculation */}
          <div className="pt-4 border-t border-slate-200 flex flex-col items-end space-y-2 text-sm">
            <div className="flex justify-between w-64">
              <span className="text-slate-600">Subtotal:</span>
              <span className="font-medium text-slate-900">${subtotal.toFixed(2)}</span>
            </div>
            <div className="flex justify-between w-64 items-center">
              <span className="text-slate-600">Tax Total:</span>
              <input
                type="number"
                min="0"
                step="0.01"
                value={taxTotal}
                onChange={(e) => setTaxTotal(parseFloat(e.target.value) || 0)}
                className="w-28 px-2 py-1 text-right text-sm bg-slate-50 border border-slate-200 rounded-lg focus:outline-none focus:ring-2 focus:ring-indigo-500"
              />
            </div>
            <div className="flex justify-between w-64 text-base font-bold pt-2 border-t border-slate-200">
              <span className="text-slate-900">Grand Total:</span>
              <span className="text-indigo-600">${grandTotal.toFixed(2)}</span>
            </div>
          </div>
        </div>

        {/* Submit Button */}
        <div className="flex justify-end gap-3">
          <button
            type="button"
            onClick={() => navigate(-1)}
            className="px-4 py-2.5 border border-slate-200 text-slate-700 font-medium rounded-lg hover:bg-slate-50 transition-colors"
          >
            Cancel
          </button>
          <button
            type="submit"
            disabled={createMutation.isPending || updateMutation.isPending}
            className="inline-flex items-center gap-2 px-6 py-2.5 bg-indigo-600 hover:bg-indigo-700 text-white font-medium rounded-lg shadow-sm transition-colors disabled:opacity-50"
          >
            <Save className="w-4 h-4" />
            {edit ? 'Update Purchase Order' : 'Create Purchase Order'}
          </button>
        </div>
      </form>
    </div>
  );
}
