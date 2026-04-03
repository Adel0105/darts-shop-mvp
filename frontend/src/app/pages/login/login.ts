import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../core/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './login.html',
  styleUrl: './login.scss',
})
export class Login {
  private fb = inject(FormBuilder);
  private auth = inject(AuthService);
  private router = inject(Router);

  form = this.fb.nonNullable.group({
    username: ['', [Validators.required]],
    password: ['', [Validators.required]],
  });

  error = '';
  isSubmitting = false;

  ngOnInit(): void {
    if (this.auth.isLoggedIn()) {
      void this.router.navigate(['/admin']);
    }
  }

  submit(): void {
    this.error = '';
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const v = this.form.getRawValue();
    this.isSubmitting = true;

    this.auth.login(v.username, v.password).subscribe({
      next: () => {
        this.isSubmitting = false;
        void this.router.navigate(['/admin']);
      },
      error: () => {
        this.error = 'Invalid username or password.';
        this.isSubmitting = false;
      },
    });
  }
}