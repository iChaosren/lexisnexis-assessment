import { HttpErrorResponse } from '@angular/common/http';

// Shared display wording only; no error/validation policy is decided here.
export function apiErrorMessage(error: unknown): string {
  if (error instanceof HttpErrorResponse) {
    if (error.status === 501) {
      return 'HTTP 501: this endpoint is a TODO scaffold. No order behavior has been implemented.';
    }
    if (error.status === 0) {
      return 'API unavailable. Start the backend and check the development proxy.';
    }
    return `Request failed (HTTP ${error.status}). TODO: design helpful API error feedback.`;
  }
  return 'Request failed. TODO: investigate the error and decide the final feedback.';
}
