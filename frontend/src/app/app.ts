import { Component, signal } from '@angular/core';
import { RouterLink,RouterOutlet } from '@angular/router';
import { AuthService } from './core/auth.service';
import { inject } from '@angular/core';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet,RouterLink],
  templateUrl: './app.html',
  styleUrl: './app.scss'
})
export class App {
  protected readonly title = signal('frontend');
  protected auth = inject(AuthService);
  logout(): void {
    this.auth.logout();
  }
}
