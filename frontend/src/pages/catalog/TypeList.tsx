import { PageHeader, Card } from '../../components/ui';

export default function TypeList() {
  return (
    <div>
      <PageHeader title="Category Types" subtitle="Manage sub-classifications for categories" />
      <Card className="p-16 flex flex-col items-center justify-center text-center">
        <h3 className="text-lg font-medium text-gray-900 mb-2">Category Types Management</h3>
        <p className="text-gray-500 max-w-md">Types are managed when a specific Category is selected. The full standalone types view is under construction.</p>
      </Card>
    </div>
  );
}
