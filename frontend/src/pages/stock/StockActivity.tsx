import { useParams, Link } from 'react-router-dom';
import { useQuery } from '@tanstack/react-query';
import { ArrowLeft } from 'lucide-react';
import { stockApi } from '../../api/stockApi';
import type { StockMovement } from '../../api/stockApi';
import { stockKeys } from '../../lib/queryKeys';

const MOVEMENT_LABELS: Record<string, { label: string; color: string }> = {
  AdjustmentIncrease: { label: 'Adjustment +', color: 'text-green-700 bg-green-50 border-green-200' },
  AdjustmentDecrease: { label: 'Adjustment −', color: 'text-red-700 bg-red-50 border-red-200' },
  PhysicalCount: { label: 'Physical Count', color: 'text-blue-700 bg-blue-50 border-blue-200' },
  Reconciliation: { label: 'Reconciliation', color: 'text-amber-700 bg-amber-50 border-amber-200' },
};

export default function StockActivity() {
  const { id } = useParams<{ id: string }>();
  const page = 1;

  const { data: item } = useQuery({
    queryKey: stockKeys.detail(id!),
    queryFn: () => stockApi.getDetail(id!),
    enabled: !!id,
  });

  const { data, isLoading } = useQuery({
    queryKey: stockKeys.activity(id!, { page }),
    queryFn: () => stockApi.getActivity(id!, page, 50),
    enabled: !!id,
  });

  return (
    <div className="space-y-5 max-w-3xl">
      <div>
        <Link
          to={`/inventory/${id}`}
          className="inline-flex items-center gap-1 text-sm text-gray-500 hover:text-[#0E4F46] mb-3"
        >
          <ArrowLeft className="h-4 w-4" /> {item?.name ?? 'Back'}
        </Link>
        <h1 className="text-2xl font-bold text-[#0E4F46]">Activity Log</h1>
        {item && (
          <p className="text-sm text-gray-400 mt-0.5">{item.itemCode} · {item.categoryName}</p>
        )}
      </div>

      {isLoading ? (
        <div className="flex justify-center py-16">
          <span className="animate-spin h-6 w-6 border-2 border-[#0E4F46] border-t-transparent rounded-full" />
        </div>
      ) : (
        <div className="space-y-3">
          {(data?.data ?? []).length === 0 && (
            <p className="text-sm text-gray-400 py-8 text-center">No stock movements recorded yet.</p>
          )}
          {(data?.data ?? []).map(m => (
            <MovementCard key={m.id} movement={m} unit={item?.defaultUnit ?? ''} />
          ))}
        </div>
      )}
    </div>
  );
}

function MovementCard({ movement: m, unit }: { movement: StockMovement; unit: string }) {
  const meta = MOVEMENT_LABELS[m.movementType] ?? { label: m.movementType, color: 'text-gray-700 bg-gray-50 border-gray-200' };
  const delta = m.quantityDelta;
  const sign = delta > 0 ? '+' : delta < 0 ? '' : '±';

  return (
    <div className="bg-white border border-gray-200 rounded-xl p-4">
      <div className="flex items-start justify-between gap-3 flex-wrap">
        <div className="flex items-center gap-2 flex-wrap">
          <span className={`text-xs font-medium px-2 py-0.5 rounded border ${meta.color}`}>
            {meta.label}
          </span>
          {m.reason && <span className="text-sm text-gray-600">{m.reason}</span>}
        </div>
        <div className="text-right">
          <p className={`text-sm font-bold ${delta > 0 ? 'text-green-600' : delta < 0 ? 'text-red-600' : 'text-gray-600'}`}>
            {sign}{delta} {unit}
          </p>
          <p className="text-xs text-gray-400">
            {m.quantityBefore} → {m.quantityAfter} {unit}
          </p>
        </div>
      </div>
      {m.notes && (
        <p className="mt-2 text-xs text-gray-500 bg-gray-50 rounded p-2">{m.notes}</p>
      )}
      <div className="mt-2 flex items-center justify-between text-xs text-gray-400">
        <span>By {m.createdByName}</span>
        <span>{new Date(m.createdAt).toLocaleString()}</span>
      </div>
    </div>
  );
}
