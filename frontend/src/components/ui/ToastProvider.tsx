import React, { createContext, useContext, useState, useCallback } from 'react';
import { X, CheckCircle, AlertCircle, Info } from 'lucide-react';
import { cn } from '../../lib/cn';

type ToastType = 'success' | 'error' | 'info';

interface Toast {
  id: string;
  type: ToastType;
  message: string;
}

interface ToastContextValue {
  showToast: (message: string, type?: ToastType) => void;
}

const ToastContext = createContext<ToastContextValue | null>(null);

export const useToast = () => {
  const ctx = useContext(ToastContext);
  if (!ctx) throw new Error('useToast must be used within ToastProvider');
  return ctx;
};

export const ToastProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  const [toasts, setToasts] = useState<Toast[]>([]);

  const showToast = useCallback((message: string, type: ToastType = 'success') => {
    const id = Math.random().toString(36).slice(2);
    setToasts((prev) => [...prev, { id, type, message }]);
    setTimeout(() => {
      setToasts((prev) => prev.filter((t) => t.id !== id));
    }, 4000);
  }, []);

  const remove = (id: string) => setToasts((prev) => prev.filter((t) => t.id !== id));

  return (
    <ToastContext.Provider value={{ showToast }}>
      {children}
      <div className="fixed bottom-4 right-4 z-50 flex flex-col gap-2 max-w-sm w-[calc(100%-2rem)] pointer-events-none">
        {toasts.map((t) => (
          <div
            key={t.id}
            role="alert"
            className={cn(
              'flex items-start gap-3 px-4 py-3 rounded-lg shadow-lg border pointer-events-auto',
              t.type === 'success' && 'bg-white border-emerald-200 text-emerald-800',
              t.type === 'error' && 'bg-white border-red-200 text-red-800',
              t.type === 'info' && 'bg-white border-blue-200 text-blue-800'
            )}
          >
            {t.type === 'success' && <CheckCircle className="h-5 w-5 text-emerald-500 mt-0.5 shrink-0" />}
            {t.type === 'error' && <AlertCircle className="h-5 w-5 text-red-500 mt-0.5 shrink-0" />}
            {t.type === 'info' && <Info className="h-5 w-5 text-blue-500 mt-0.5 shrink-0" />}
            <span className="flex-1 text-sm font-medium">{t.message}</span>
            <button
              onClick={() => remove(t.id)}
              className="shrink-0 text-gray-400 hover:text-gray-600"
              aria-label="Dismiss"
            >
              <X className="h-4 w-4" />
            </button>
          </div>
        ))}
      </div>
    </ToastContext.Provider>
  );
};
