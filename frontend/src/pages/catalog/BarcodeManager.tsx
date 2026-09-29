import { PageHeader, Card } from '../../components/ui';

export default function BarcodeManager() {
  return (
    <div>
      <PageHeader title="Barcode Management" subtitle="Scan and assign barcodes to items" />
      <Card className="p-16 flex flex-col items-center justify-center text-center">
        <h3 className="text-lg font-medium text-gray-900 mb-2">Barcode Scanner</h3>
        <p className="text-gray-500 max-w-md">The standalone barcode assignment tool requires the Stock module configuration to be fully implemented in Phase 4.</p>
      </Card>
    </div>
  );
}
