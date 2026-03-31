import { Component, Inject, signal } from '@angular/core';
import { RouterLink,RouterOutlet, Route } from '@angular/router';
import { AuthService } from './core/auth.service';
import { Router } from '@angular/router';
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
  private router=inject(Router);
  logout(): void {

     this.auth.logout();
    this.router.navigate(['/login']);

  }
}
