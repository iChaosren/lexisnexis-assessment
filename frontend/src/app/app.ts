import { HttpClient } from '@angular/common/http';
import { Component, inject, signal } from '@angular/core';

@Component({
  selector: 'app-root',
  imports: [],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  private readonly http = inject(HttpClient);
  protected readonly health = signal('Not checked');

  protected checkHealth(): void {
    this.health.set('Checking...');
    this.http.get('/api/health', { responseType: 'text' }).subscribe({
      next: (result) => this.health.set(result),
      error: () => this.health.set('API unavailable. Start the backend and try again.')
    });
  }
}
