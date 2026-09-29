import { useState } from 'react';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { Bell, CheckCheck, ExternalLink } from 'lucide-react';
import { notificationApi, type NotificationDto } from '../api/notificationApi';
import { notificationKeys } from '../lib/queryKeys';
import { useNavigate } from 'react-router-dom';

export function NotificationBell() {
  const [open, setOpen] = useState(false);
  const navigate = useNavigate();
  const queryClient = useQueryClient();

  const { data: unreadData } = useQuery({
    queryKey: notificationKeys.unreadCount(),
    queryFn: () => notificationApi.getUnreadCount(),
    refetchInterval: 30000, // poll every 30s
  });

  const { data: listData } = useQuery({
    queryKey: notificationKeys.list({ page: 1, pageSize: 10 }),
    queryFn: () => notificationApi.getNotifications(1, 10, false),
    enabled: open,
  });

  const markReadMutation = useMutation({
    mutationFn: (id: string) => notificationApi.markAsRead(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: notificationKeys.all });
    },
  });

  const markAllReadMutation = useMutation({
    mutationFn: () => notificationApi.markAllAsRead(),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: notificationKeys.all });
    },
  });

  const unreadCount = unreadData ?? 0;
  const notifications = listData?.data || [];

  const handleNotificationClick = (n: NotificationDto) => {
    if (!n.isRead) {
      markReadMutation.mutate(n.id);
    }
    setOpen(false);
    if (n.referenceType === 'Purchase' && n.referenceId) {
      navigate(`/purchases/${n.referenceId}`);
    } else if (n.referenceType === 'CatalogItem' && n.referenceId) {
      navigate(`/inventory/all`);
    }
  };

  return (
    <div className="relative">
      <button
        onClick={() => setOpen(!open)}
        className="relative p-2 text-slate-600 hover:text-[#0E4F46] hover:bg-slate-100 rounded-lg transition-colors"
        aria-label="Notifications"
      >
        <Bell className="w-5 h-5" />
        {unreadCount > 0 && (
          <span className="absolute top-1 right-1 w-4 h-4 bg-rose-500 text-white text-[10px] font-bold rounded-full flex items-center justify-center">
            {unreadCount > 9 ? '9+' : unreadCount}
          </span>
        )}
      </button>

      {open && (
        <div className="absolute right-0 mt-2 w-80 sm:w-96 bg-white border border-slate-200 rounded-xl shadow-xl z-50 overflow-hidden">
          <div className="p-4 border-b border-slate-100 flex items-center justify-between bg-slate-50">
            <div className="flex items-center gap-2">
              <h3 className="font-semibold text-slate-900 text-sm">Notifications</h3>
              {unreadCount > 0 && (
                <span className="px-2 py-0.5 bg-indigo-100 text-indigo-700 text-xs font-semibold rounded-full">
                  {unreadCount} new
                </span>
              )}
            </div>
            <div className="flex items-center gap-2">
              {unreadCount > 0 && (
                <button
                  onClick={() => markAllReadMutation.mutate()}
                  className="text-xs font-medium text-indigo-600 hover:text-indigo-800 flex items-center gap-1"
                >
                  <CheckCheck className="w-3.5 h-3.5" /> Mark all read
                </button>
              )}
            </div>
          </div>

          <div className="max-h-96 overflow-y-auto divide-y divide-slate-100">
            {notifications.length === 0 ? (
              <div className="p-8 text-center text-slate-400 text-sm">
                No notifications found.
              </div>
            ) : (
              notifications.map((n) => (
                <div
                  key={n.id}
                  onClick={() => handleNotificationClick(n)}
                  className={`p-3.5 hover:bg-slate-50 cursor-pointer transition-colors flex items-start gap-3 ${
                    !n.isRead ? 'bg-indigo-50/40' : ''
                  }`}
                >
                  <div
                    className={`w-2 h-2 rounded-full mt-2 shrink-0 ${
                      !n.isRead ? 'bg-indigo-600' : 'bg-transparent'
                    }`}
                  />
                  <div className="flex-1 min-w-0">
                    <p className={`text-xs font-semibold text-slate-900 truncate ${!n.isRead ? 'font-bold' : ''}`}>
                      {n.title}
                    </p>
                    <p className="text-xs text-slate-600 mt-0.5 line-clamp-2">{n.message}</p>
                    <p className="text-[10px] text-slate-400 mt-1">
                      {new Date(n.createdAt).toLocaleString()}
                    </p>
                  </div>
                  {n.referenceId && (
                    <ExternalLink className="w-4 h-4 text-slate-400 shrink-0 self-center" />
                  )}
                </div>
              ))
            )}
          </div>

          <div className="p-3 border-t border-slate-100 bg-slate-50 text-center">
            <button
              onClick={() => {
                setOpen(false);
                navigate('/notifications');
              }}
              className="text-xs font-semibold text-indigo-600 hover:text-indigo-800"
            >
              View all notifications →
            </button>
          </div>
        </div>
      )}
    </div>
  );
}
