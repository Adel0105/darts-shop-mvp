import { Injectable, computed, inject, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map, tap } from 'rxjs/operators';
const STORAGE_KEY = 'darts-shop-auth-token';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private http = inject(HttpClient);
  private apiUrl = 'http://localhost:5231/api';

  private tokenSignal = signal<string | null>(this.readTokenFromStorage());

  readonly token = this.tokenSignal.asReadonly();
  readonly isLoggedIn = computed(() => !!this.tokenSignal());

  

// ...
  login(username: string, password: string): Observable<void> {
    return this.http
      .post<{ token: string }>(`${this.apiUrl}/auth/login`, { username, password })
      .pipe(
        tap((res) => {
          localStorage.setItem(STORAGE_KEY, res.token);
          this.tokenSignal.set(res.token);
        }),
        map(() => void 0)
      );
  }

  logout(): void {
    localStorage.removeItem(STORAGE_KEY);
    this.tokenSignal.set(null);
  }

  private readTokenFromStorage(): string | null {
    try {
      return localStorage.getItem(STORAGE_KEY);
    } catch {
      return null;
    }
  }
}