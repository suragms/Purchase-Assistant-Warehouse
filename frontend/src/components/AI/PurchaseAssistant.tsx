import React, { useState } from 'react';
import { purchaseIntentApi, type PurchaseIntentCandidateDto, type PurchaseIntentItemCandidateDto } from '../../api/purchaseIntentApi';
import { Button, Textarea, Card } from '../ui';

interface PurchaseAssistantProps {
  onDraftConfirmed: (items: PurchaseIntentItemCandidateDto[], supplierId?: string) => void;
}

export const PurchaseAssistant: React.FC<PurchaseAssistantProps> = ({ onDraftConfirmed }) => {
  const [prompt, setPrompt] = useState('');
  const [candidate, setCandidate] = useState<PurchaseIntentCandidateDto | null>(null);
  const [loading, setLoading] = useState(false);

  const handleParse = async () => {
    setLoading(true);
    try {
      const result = await purchaseIntentApi.parseIntent(prompt);
      setCandidate(result);
    } catch (e) {
      console.error(e);
    } finally {
      setLoading(false);
    }
  };

  const handleConfirm = () => {
    if (candidate) {
      onDraftConfirmed(candidate.items, candidate.supplierId);
      setCandidate(null);
      setPrompt('');
    }
  };

  return (
    <Card className="p-4 mb-6">
      <h2 className="text-xl font-bold mb-4">AI Purchase Assistant</h2>
      <Textarea
        label="Enter purchase request"
        value={prompt}
        onChange={(e) => setPrompt(e.target.value)}
        placeholder="e.g., Buy 10 units of Rice from Supplier X"
      />
      <Button onClick={handleParse} disabled={loading} className="mt-2" loading={loading}>
        Analyze Intent
      </Button>

      {candidate && (
        <Card className="mt-4 p-4 border-indigo-200">
          <h3 className="font-bold text-lg mb-2">Intent Analysis: {candidate.status}</h3>
          {candidate.message && <p className="text-sm text-gray-600 mb-4">{candidate.message}</p>}

          <div className="space-y-4">
            {candidate.items.map((item, index) => (
              <div key={index} className="flex justify-between items-center p-2 bg-gray-50 rounded">
                <span>{item.catalogItemName || item.itemCode}</span>
                <span className="font-medium">{item.requestedQuantity} {item.unitOfMeasure}</span>
              </div>
            ))}
          </div>

          <div className="flex gap-2 mt-4">
            <Button variant="primary" onClick={handleConfirm}>Confirm Draft</Button>
            <Button variant="secondary" onClick={() => setCandidate(null)}>Discard</Button>
          </div>
        </Card>
      )}
    </Card>
  );
};
