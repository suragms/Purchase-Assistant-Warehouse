import React from 'react';
import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import { AuthProvider } from '../auth/AuthProvider';
import { ProtectedRoute } from '../auth/Guards';
import { ToastProvider } from '../components/ui/ToastProvider';
import { AppShell } from '../layouts/AppShell';
import Login from '../pages/auth/Login';

// Placeholder empty components for lazy loading
const Dashboard = React.lazy(() => import('../pages/Dashboard'));
const CatalogList = React.lazy(() => import('../pages/catalog/CatalogList'));
const CatalogForm = React.lazy(() => import('../pages/catalog/CatalogForm'));
const CatalogDetail = React.lazy(() => import('../pages/catalog/CatalogDetail'));
const CategoryList = React.lazy(() => import('../pages/catalog/CategoryList'));
const TypeList = React.lazy(() => import('../pages/catalog/TypeList'));
const BarcodeManager = React.lazy(() => import('../pages/catalog/BarcodeManager'));
const DuplicateReview = React.lazy(() => import('../pages/catalog/DuplicateReview'));
const SupplierList = React.lazy(() => import('../pages/suppliers/SupplierList'));
const BrokerList = React.lazy(() => import('../pages/brokers/BrokerList'));

export const AppRouter = () => {
  return (
    <BrowserRouter>
      <ToastProvider>
        <AuthProvider>
          <Routes>
            <Route path="/login" element={<Login />} />

            <Route path="/" element={<ProtectedRoute />}>
              <Route element={<AppShell />}>
                <Route index element={<Navigate to="/dashboard" replace />} />
                <Route path="dashboard" element={<Dashboard />} />

                {/* Catalog Scope */}
                <Route path="catalog/items" element={<CatalogList />} />
                <Route path="catalog/items/new" element={<CatalogForm />} />
                <Route path="catalog/items/:id" element={<CatalogDetail />} />
                <Route path="catalog/items/:id/edit" element={<CatalogForm edit />} />
                <Route path="catalog/categories" element={<CategoryList />} />
                <Route path="catalog/types" element={<TypeList />} />
                <Route path="catalog/barcodes" element={<BarcodeManager />} />
                <Route path="catalog/duplicates" element={<DuplicateReview />} />

                {/* Suppliers & Brokers */}
                <Route path="suppliers" element={<SupplierList />} />
                <Route path="brokers" element={<BrokerList />} />

                {/* Stubs */}
                <Route path="inventory" element={<div className="p-4">Phase 4 Placeholder</div>} />
                <Route path="users" element={<div className="p-4">Phase 2 Users Interface</div>} />
              </Route>
            </Route>

          </Routes>
        </AuthProvider>
      </ToastProvider>
    </BrowserRouter>
  );
};
