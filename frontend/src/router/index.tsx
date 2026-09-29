import React from 'react';
import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import { AuthProvider } from '../auth/AuthProvider';
import { ProtectedRoute } from '../auth/Guards';
import { ToastProvider } from '../components/ui/ToastProvider';
import { AppShell } from '../layouts/AppShell';
import Login from '../pages/auth/Login';

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
const StockDashboard = React.lazy(() => import('../pages/stock/StockDashboard'));
const StockList = React.lazy(() => import('../pages/stock/StockList'));
const StockDetail = React.lazy(() => import('../pages/stock/StockDetail'));
const StockActivity = React.lazy(() => import('../pages/stock/StockActivity'));

const PurchaseDashboard = React.lazy(() => import('../pages/purchases/PurchaseDashboard'));
const PurchaseList = React.lazy(() => import('../pages/purchases/PurchaseList'));
const PurchaseForm = React.lazy(() => import('../pages/purchases/PurchaseForm'));
const PurchaseDetail = React.lazy(() => import('../pages/purchases/PurchaseDetail'));

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

                {/* Inventory / Stock — Phase 4 */}
                <Route path="inventory" element={<Navigate to="/inventory/overview" replace />} />
                <Route path="inventory/overview" element={<StockDashboard />} />
                <Route path="inventory/all" element={<StockList />} />
                <Route path="inventory/low-stock" element={<StockList />} />
                <Route path="inventory/out-of-stock" element={<StockList />} />
                <Route path="inventory/:id" element={<StockDetail />} />
                <Route path="inventory/:id/activity" element={<StockActivity />} />

                {/* Purchase Order Management — Phase 5 */}
                <Route path="purchases" element={<Navigate to="/purchases/overview" replace />} />
                <Route path="purchases/overview" element={<PurchaseDashboard />} />
                <Route path="purchases/list" element={<PurchaseList />} />
                <Route path="purchases/new" element={<PurchaseForm />} />
                <Route path="purchases/:id" element={<PurchaseDetail />} />
                <Route path="purchases/:id/edit" element={<PurchaseForm edit />} />

                <Route path="users" element={<div className="p-4">Phase 2 Users Interface</div>} />
              </Route>
            </Route>

          </Routes>
        </AuthProvider>
      </ToastProvider>
    </BrowserRouter>
  );
};
