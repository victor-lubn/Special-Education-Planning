export interface Address {
addressLine1: string;
addressLine2: string;
addressLine3: string;
locality?: string;
province?: string;
postalCode: string;
country?: string;
}

export interface Suggestions { 
  suggestion: string;
  format: string;
}