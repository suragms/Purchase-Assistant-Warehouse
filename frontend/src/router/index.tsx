import React from 'react';
import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import { AuthProvider } from '../auth/AuthProvider';
import { ProtectedRoute } from '../auth/Guards';
import Login from '../pages/auth/Login';

// Placeholder layouts
const AppShell = () => (
    <div className="flex flex-col h-screen">
        <header className="h-16 bg-[#0E4F46] text-white flex items-center px-4">
            <h1 className="font-bold">Purchase Assistant ERP</h1>
        </header>
        <div className="flex-1 flex overflow-hidden">
            <aside className="w-64 bg-white border-r border-[#E2E8E6] p-4 hidden md:block">
                Navigation
            </aside>
            <main className="flex-1 bg-[#F7F9F6] p-6 overflow-y-auto">
                <React.Suspense fallback={<div>Loading...</div>}>
                   <div id="portal-root">Welcome to the Dashboard!</div>
                </React.Suspense>
            </main>
        </div>
    </div>
);

export const AppRouter = () => {
  return (
    <BrowserRouter>
      <AuthProvider>
        <Routes>
          <Route path="/login" element={<Login />} />
          <Route path="/" element={<ProtectedRoute />}>
             <Route element={<AppShell />}>
                 <Route index element={<Navigate to="/dashboard" replace />} />
                 <Route path="dashboard" element={<div>Dashboard Metrics Overview</div>} />
                 {/* Extend routes here dynamically for purchases, inventory etc in Phase 3 */}
             </Route>
          </Route>
        </Routes>
      </AuthProvider>
    </BrowserRouter>
  );
};
