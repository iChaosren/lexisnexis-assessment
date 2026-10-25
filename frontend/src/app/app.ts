import { Component, DestroyRef, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { ApiService } from './api.service';

@Component({
  selector: 'app-root',
  // Template imports make directives/components available to this template.
  // They are different from app.config.ts providers and inject(ApiService).
  imports: [RouterLink, RouterLinkActive, RouterOutlet],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  // Ask Angular's injector for the shared service; do not construct it with new.
  private readonly api = inject(ApiService);
  private readonly destroyRef = inject(DestroyRef);
  protected readonly health = signal('Not checked');

  protected checkHealth(): void {
    this.health.set('Checking...');
    this.api.health().pipe(takeUntilDestroyed(this.destroyRef)).subscribe({
      next: (result) => this.health.set(result),
      error: () => this.health.set('API unavailable. Start the backend and try again.')
    });
  }
}
