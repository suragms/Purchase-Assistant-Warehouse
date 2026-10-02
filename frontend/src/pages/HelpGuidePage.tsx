import { Link } from 'react-router-dom';
import { PageHeader } from '../components/ui';
import { useAuthStore } from '../stores/authStore';
import { hasPermission } from '../auth/Guards';
// Adapted from the reference's static help_guide_page, using the existing web actions/routes.
export default function HelpGuidePage() {
  const user = useAuthStore(s => s.user); const role = user?.currentBusiness?.role;
  const owner = ['Owner', 'Admin', 'SuperAdmin'].includes(role ?? '');
  const guides = [
    { title: 'Home dashboard', permission: undefined, owner: true, steps: ['Review purchases, low stock and pending deliveries.', 'Open a section to see its records.'], route: '/dashboard' },
    { title: 'Add a purchase', permission: 'purchase.create', owner: true, steps: ['Choose the supplier and items.', 'Enter quantities and prices, review the preview, then save.'], route: '/purchases/new' },
    { title: 'Barcode lookup', permission: 'catalog.view', steps: ['Enter the barcode to find its item.', 'Camera scanning and photo recognition are not available in this web app.'], route: '/catalog/barcodes' },
    { title: 'Stock', permission: 'stock.view', steps: ['Open an item to review system and physical stock.', 'Record a physical count when you have stock-edit access.'], route: '/inventory/all' },
    { title: 'Print barcode labels', permission: 'catalog.view', owner: true, steps: ['The reference supports printable barcode labels.', 'Label PDF generation is not available in this web app.'] },
    { title: 'Export & Backup', permission: 'reports.view', owner: true, steps: ['Download stock Excel, monthly purchase PDF, business JSON or a purchase ZIP.', 'Keep a local copy. Restore validation does not change your data.'], route: '/settings/backup' },
    { title: 'Add staff', permission: 'users.manage', owner: true, steps: ['Open Users and choose Add user.', 'Enter the staff details and select the Staff role.'], route: '/users' },
    { title: 'Receive a delivery', permission: 'purchase.view', staff: true, steps: ['Open the arriving purchase and check its items.', 'Verify received quantities and report damage when necessary.'], route: '/purchases/list' },
  ];
  return <div className="space-y-6 min-w-0"><PageHeader title="How to use this app" subtitle="Warehouse Assistant · Harisree Agency warehouse guide" /><Link to="/settings" className="underline inline-block min-h-12">Back to Settings</Link>
    {guides.filter(g => (!g.owner || owner) && (!g.staff || role === 'Staff') && hasPermission(user, g.permission)).map(g => <details key={g.title} className="rounded-xl border bg-white p-4"><summary className="font-semibold cursor-pointer min-h-12">{g.title}</summary><ol className="list-decimal pl-6 space-y-3">{g.steps.map(step => <li key={step}>{step}</li>)}</ol>{g.route && <Link className="underline inline-block py-3" to={g.route}>Try it · {g.title}</Link>}</details>)}
  </div>;
}
