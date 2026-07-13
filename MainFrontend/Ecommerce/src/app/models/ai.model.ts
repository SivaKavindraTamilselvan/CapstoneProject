export interface AiValidationResult {
  isValid: boolean;
  confidence: number;
  issues: string[];
  recommendation: 'ACCEPT' | 'REJECT';
}