import { useQuery } from '@tanstack/react-query';
import { useNavigate } from 'react-router-dom';
import { purchaseApi, PurchaseStatus, DeliveryState } from '../../api/purchaseApi';
import { purchaseKeys } from '../../lib/queryKeys';
import {
  ShoppingBag,
  Plus,
  Clock,
  Truck,
  CheckCircle,
  DollarSign
} from 'lucide-react';

export default function PurchaseDashboard() {
  const navigate = useNavigate();

  const { data: allPurchases, isLoading } = useQuery({
    queryKey: purchaseKeys.lists(),
    queryFn: () => purchaseApi.getPurchases(1, 100),
  });

  const orders = allPurchases?.data || [];

  const draftCount = orders.filter(o => o.status === PurchaseStatus.Draft).length;
  const confirmedCount = orders.filter(o => o.status === PurchaseStatus.Confirmed).length;
  const dispatchedCount = orders.filter(o => o.status === PurchaseStatus.Dispatched || o.status === PurchaseStatus.Arrived).length;

  const totalSpend = orders
    .filter(o => o.status !== PurchaseStatus.Cancelled)
    .reduce((sum, o) => sum + o.grandTotal, 0);

  const recentOrders = [...orders]
    .sort((a, b) => new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime())
    .slice(0, 5);

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-4">
        <div>
          <h1 className="text-2xl font-bold text-slate-900 tracking-tight flex items-center gap-2">
            <ShoppingBag className="w-7 h-7 text-indigo-600" />
            Purchase Engine Dashboard
          </h1>
          <p className="text-sm text-slate-500 mt-1">
            Overview of purchase orders, lifecycle progress, shipments, and inventory receipts.
          </p>
        </div>
        <div className="flex items-center gap-3">
          <button
            onClick={() => navigate('/purchases/list')}
            className="px-4 py-2 bg-white border border-slate-200 text-slate-700 font-medium rounded-lg shadow-sm hover:bg-slate-50 transition-colors"
          >
            View All Orders
          </button>
          <button
            onClick={() => navigate('/purchases/new')}
            className="inline-flex items-center gap-2 px-4 py-2 bg-indigo-600 hover:bg-indigo-700 text-white font-medium rounded-lg shadow-sm transition-colors"
          >
            <Plus className="w-4 h-4" /> New Purchase Order
          </button>
        </div>
      </div>

      {/* Metrics Grid */}
      <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4">
        <div className="bg-white p-5 rounded-xl shadow-sm border border-slate-200 flex items-center justify-between">
          <div>
            <p className="text-xs font-semibold uppercase tracking-wider text-slate-500">Draft Orders</p>
            <p className="text-2xl font-bold text-slate-900 mt-1">{draftCount}</p>
          </div>
          <div className="p-3 bg-slate-100 rounded-xl text-slate-600">
            <Clock className="w-6 h-6" />
          </div>
        </div>

        <div className="bg-white p-5 rounded-xl shadow-sm border border-slate-200 flex items-center justify-between">
          <div>
            <p className="text-xs font-semibold uppercase tracking-wider text-slate-500">Active / Confirmed</p>
            <p className="text-2xl font-bold text-blue-600 mt-1">{confirmedCount}</p>
          </div>
          <div className="p-3 bg-blue-50 rounded-xl text-blue-600">
            <CheckCircle className="w-6 h-6" />
          </div>
        </div>

        <div className="bg-white p-5 rounded-xl shadow-sm border border-slate-200 flex items-center justify-between">
          <div>
            <p className="text-xs font-semibold uppercase tracking-wider text-slate-500">In Transit / Dispatched</p>
            <p className="text-2xl font-bold text-amber-600 mt-1">{dispatchedCount}</p>
          </div>
          <div className="p-3 bg-amber-50 rounded-xl text-amber-600">
            <Truck className="w-6 h-6" />
          </div>
        </div>

        <div className="bg-white p-5 rounded-xl shadow-sm border border-slate-200 flex items-center justify-between">
          <div>
            <p className="text-xs font-semibold uppercase tracking-wider text-slate-500">Total Purchase Value</p>
            <p className="text-2xl font-bold text-emerald-600 mt-1">${totalSpend.toFixed(2)}</p>
          </div>
          <div className="p-3 bg-emerald-50 rounded-xl text-emerald-600">
            <DollarSign className="w-6 h-6" />
          </div>
        </div>
      </div>

      {/* Recent Orders Table */}
      <div className="bg-white rounded-xl shadow-sm border border-slate-200 overflow-hidden">
        <div className="p-6 border-b border-slate-200 flex items-center justify-between">
          <h2 className="text-lg font-semibold text-slate-900">Recent Purchase Orders</h2>
          <button
            onClick={() => navigate('/purchases/list')}
            className="text-sm font-medium text-indigo-600 hover:text-indigo-700"
          >
            View all →
          </button>
        </div>

        {isLoading ? (
          <div className="p-8 text-center text-slate-500">Loading recent purchases...</div>
        ) : recentOrders.length === 0 ? (
          <div className="p-12 text-center text-slate-500">
            <ShoppingBag className="w-12 h-12 mx-auto text-slate-300 mb-3" />
            <p className="font-medium">No purchase orders found</p>
          </div>
        ) : (
          <div className="overflow-x-auto">
            <table className="w-full text-left border-collapse">
              <thead>
                <tr className="bg-slate-50 border-b border-slate-200 text-xs font-semibold text-slate-600 uppercase tracking-wider">
                  <th className="py-3 px-4">Order #</th>
                  <th className="py-3 px-4">Supplier</th>
                  <th className="py-3 px-4">Status</th>
                  <th className="py-3 px-4">Delivery</th>
                  <th className="py-3 px-4 text-right">Grand Total</th>
                  <th className="py-3 px-4 text-right">Action</th>
                </tr>
              </thead>
              <tbody className="divide-y divide-slate-200 text-sm">
                {recentOrders.map((po) => (
                  <tr key={po.id} className="hover:bg-slate-50/80 transition-colors">
                    <td className="py-3 px-4 font-semibold text-slate-900">{po.orderNumber}</td>
                    <td className="py-3 px-4 text-slate-700">{po.supplierName}</td>
                    <td className="py-3 px-4">
                      <span className="px-2.5 py-0.5 rounded-full text-xs font-medium bg-slate-100 text-slate-700">
                        {Object.keys(PurchaseStatus).find(key => PurchaseStatus[key as keyof typeof PurchaseStatus] === po.status)}
                      </span>
                    </td>
                    <td className="py-3 px-4 text-xs font-medium text-slate-600">
                      {Object.keys(DeliveryState).find(key => DeliveryState[key as keyof typeof DeliveryState] === po.deliveryState)}
                    </td>
                    <td className="py-3 px-4 text-right font-semibold text-slate-900">
                      ${po.grandTotal.toFixed(2)}
                    </td>
                    <td className="py-3 px-4 text-right">
                      <button
                        onClick={() => navigate(`/purchases/${po.id}`)}
                        className="text-indigo-600 hover:text-indigo-800 font-medium text-xs"
                      >
                        Inspect
                      </button>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}
      </div>
    </div>
  );
}
