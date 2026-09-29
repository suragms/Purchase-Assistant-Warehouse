import React, { useState } from 'react';
import { NavLink, Outlet, useNavigate } from 'react-router-dom';
import {
  PackageOpen, LayoutDashboard, Package, Users, Truck,
  ChevronDown, ChevronRight, Menu, X, LogOut, Search,
  Boxes, Building2
} from 'lucide-react';
import { useAuthStore } from '../stores/authStore';
import { cn } from '../lib/cn';
import { GlobalSearch } from '../components/search/GlobalSearch';

interface NavItem {
  label: string;
  icon: React.ReactNode;
  to?: string;
  children?: { label: string; to: string }[];
}

const navItems: NavItem[] = [
  { label: 'Dashboard', icon: <LayoutDashboard className="h-4 w-4" />, to: '/dashboard' },
  {
    label: 'Catalog', icon: <Package className="h-4 w-4" />, children: [
      { label: 'Items', to: '/catalog/items' },
      { label: 'Categories', to: '/catalog/categories' },
      { label: 'Types', to: '/catalog/types' },
      { label: 'Barcodes', to: '/catalog/barcodes' },
      { label: 'Duplicates', to: '/catalog/duplicates' },
    ]
  },
  { label: 'Suppliers', icon: <Truck className="h-4 w-4" />, to: '/suppliers' },
  { label: 'Brokers', icon: <Building2 className="h-4 w-4" />, to: '/brokers' },
  { label: 'Inventory', icon: <Boxes className="h-4 w-4" />, to: '/inventory' },
  { label: 'Users', icon: <Users className="h-4 w-4" />, to: '/users' },
];

const SidebarNavItem: React.FC<{ item: NavItem; onNavigate?: () => void }> = ({ item, onNavigate }) => {
  const [open, setOpen] = useState(false);

  if (item.children) {
    return (
      <div>
        <button
          onClick={() => setOpen((o) => !o)}
          className="w-full flex items-center justify-between gap-2 px-3 py-2 rounded-lg text-sm text-[#E2E8E6] hover:bg-white/10 transition-colors"
        >
          <span className="flex items-center gap-2">
            {item.icon}
            {item.label}
          </span>
          {open ? <ChevronDown className="h-3 w-3" /> : <ChevronRight className="h-3 w-3" />}
        </button>
        {open && (
          <div className="ml-6 mt-0.5 flex flex-col gap-0.5">
            {item.children.map((child) => (
              <NavLink
                key={child.to}
                to={child.to}
                onClick={onNavigate}
                className={({ isActive }) =>
                  cn(
                    'px-3 py-1.5 rounded-lg text-sm transition-colors',
                    isActive
                      ? 'bg-white/20 text-white font-medium'
                      : 'text-[#B8D4CF] hover:bg-white/10 hover:text-white'
                  )
                }
              >
                {child.label}
              </NavLink>
            ))}
          </div>
        )}
      </div>
    );
  }

  return (
    <NavLink
      to={item.to!}
      onClick={onNavigate}
      className={({ isActive }) =>
        cn(
          'flex items-center gap-2 px-3 py-2 rounded-lg text-sm transition-colors',
          isActive
            ? 'bg-white/20 text-white font-medium'
            : 'text-[#B8D4CF] hover:bg-white/10 hover:text-white'
        )
      }
    >
      {item.icon}
      {item.label}
    </NavLink>
  );
};

export const AppShell: React.FC = () => {
  const [mobileOpen, setMobileOpen] = useState(false);
  const [searchOpen, setSearchOpen] = useState(false);
  const { user, logout } = useAuthStore();
  const navigate = useNavigate();

  const handleLogout = () => {
    logout();
    navigate('/login');
  };

  // Global Ctrl+K / Cmd+K shortcut
  React.useEffect(() => {
    const handler = (e: KeyboardEvent) => {
      if ((e.ctrlKey || e.metaKey) && e.key === 'k') {
        e.preventDefault();
        setSearchOpen(true);
      }
    };
    window.addEventListener('keydown', handler);
    return () => window.removeEventListener('keydown', handler);
  }, []);

  const Sidebar = ({ mobile = false }: { mobile?: boolean }) => (
    <div className={cn('flex flex-col h-full', mobile ? 'w-72' : 'w-64')}>
      {/* Logo */}
      <div className="flex items-center gap-2 px-4 py-5 border-b border-white/10">
        <PackageOpen className="h-7 w-7 text-[#4ECDC4]" />
        <span className="font-bold text-white text-lg leading-tight">Purchase<br />Assistant</span>
      </div>

      {/* Business badge */}
      {user?.currentBusiness && (
        <div className="px-4 py-2 border-b border-white/10">
          <p className="text-xs text-[#8FC4BC]">Business</p>
          <p className="text-sm font-medium text-white truncate">{user.currentBusiness.businessName}</p>
        </div>
      )}

      {/* Nav */}
      <nav className="flex-1 overflow-y-auto px-3 py-4 flex flex-col gap-1" aria-label="Main navigation">
        {navItems.map((item) => (
          <SidebarNavItem key={item.label} item={item} onNavigate={() => setMobileOpen(false)} />
        ))}
      </nav>

      {/* User */}
      <div className="border-t border-white/10 px-4 py-3 flex items-center gap-3">
        <div className="flex-1 min-w-0">
          <p className="text-sm font-medium text-white truncate">{user?.name}</p>
          <p className="text-xs text-[#8FC4BC] truncate">{user?.currentBusiness?.role}</p>
        </div>
        <button
          onClick={handleLogout}
          className="text-[#8FC4BC] hover:text-white transition-colors"
          aria-label="Sign out"
        >
          <LogOut className="h-4 w-4" />
        </button>
      </div>
    </div>
  );

  return (
    <div className="flex h-screen bg-[#F7F9F6] overflow-hidden">
      {/* Desktop Sidebar */}
      <div className="hidden md:flex flex-col bg-[#0E4F46] shrink-0">
        <Sidebar />
      </div>

      {/* Mobile drawer */}
      {mobileOpen && (
        <div className="fixed inset-0 z-40 flex md:hidden">
          <div
            className="absolute inset-0 bg-black/40"
            onClick={() => setMobileOpen(false)}
            aria-hidden="true"
          />
          <div className="relative bg-[#0E4F46] shadow-xl z-50">
            <button
              className="absolute top-3 right-3 text-white"
              onClick={() => setMobileOpen(false)}
              aria-label="Close navigation"
            >
              <X className="h-5 w-5" />
            </button>
            <Sidebar mobile />
          </div>
        </div>
      )}

      {/* Main content */}
      <div className="flex-1 flex flex-col min-w-0">
        {/* Top bar */}
        <header className="h-14 bg-white border-b border-[#E2E8E6] flex items-center gap-3 px-4 shrink-0">
          <button
            className="md:hidden text-[#475569] hover:text-[#0E4F46]"
            onClick={() => setMobileOpen(true)}
            aria-label="Open navigation"
          >
            <Menu className="h-5 w-5" />
          </button>

          {/* Search trigger */}
          <button
            onClick={() => setSearchOpen(true)}
            className="flex items-center gap-2 flex-1 max-w-md text-left text-sm text-gray-400 bg-gray-50 border border-[#E2E8E6] rounded-lg px-3 py-1.5 hover:border-[#159A8A] transition-colors focus:outline-none focus:ring-2 focus:ring-[#159A8A]"
            aria-label="Open search"
          >
            <Search className="h-4 w-4 shrink-0" />
            <span className="flex-1">Search…</span>
            <kbd className="hidden sm:inline-flex text-xs bg-white border border-gray-200 rounded px-1.5 py-0.5 text-gray-400">
              ⌘K
            </kbd>
          </button>

          <div className="ml-auto flex items-center gap-2">
            <span className="hidden sm:block text-sm text-[#475569] font-medium">{user?.name}</span>
          </div>
        </header>

        {/* Page content */}
        <main className="flex-1 overflow-y-auto">
          <div className="max-w-screen-2xl mx-auto px-4 sm:px-6 py-6">
            <React.Suspense
              fallback={
                <div className="flex items-center justify-center py-24">
                  <span className="animate-spin h-8 w-8 border-2 border-[#0E4F46] border-t-transparent rounded-full" />
                </div>
              }
            >
              <Outlet />
            </React.Suspense>
          </div>
        </main>
      </div>

      {/* Global Search modal */}
      <GlobalSearch open={searchOpen} onClose={() => setSearchOpen(false)} />
    </div>
  );
};
