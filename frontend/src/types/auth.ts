export interface User {
  id: string;
  name: string;
  email: string;
  currentBusiness: BusinessContext | null;
  businesses: BusinessSummary[];
}

export interface BusinessContext {
  businessId: string;
  businessName: string;
  role: string;
  permissions: string[];
}

export interface BusinessSummary {
  businessId: string;
  businessName: string;
  role: string;
}

export interface AuthResponse {
  data: {
    accessToken: string;
    expiresAt: number;
    user: User;
  };
}
