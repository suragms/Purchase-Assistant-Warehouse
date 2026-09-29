import { PageHeader, Card } from '../../components/ui';

export default function DuplicateReview() {
  return (
    <div>
      <PageHeader title="Duplicate Review" subtitle="Review potential duplicate items" />
      <Card className="p-16 flex flex-col items-center justify-center text-center">
        <h3 className="text-lg font-medium text-gray-900 mb-2">No Duplicates Found</h3>
        <p className="text-gray-500 max-w-md">The master catalog data is clean.</p>
      </Card>
    </div>
  );
}
