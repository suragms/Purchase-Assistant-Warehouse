import { PageHeader, Card } from '../components/ui';

export default function Dashboard() {
  return (
    <div>
      <PageHeader title="Dashboard" subtitle="Overview of your business" />
      <div className="grid sm:grid-cols-2 lg:grid-cols-4 gap-4">
        {[1, 2, 3, 4].map((i) => (
          <Card key={i} className="p-4 flex flex-col justify-between h-32">
            <span className="text-sm font-medium text-gray-500">Metric {i}</span>
            <span className="text-2xl font-bold text-gray-900">1,234</span>
          </Card>
        ))}
      </div>
    </div>
  );
}
