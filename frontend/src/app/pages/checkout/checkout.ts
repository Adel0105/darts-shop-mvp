import { Component, inject } from '@angular/core';
import { CommonModule, CurrencyPipe } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { Router, RouterLink } from '@angular/router';
import { CartService } from '../../core/cart.service';

@Component({
  selector: 'app-checkout',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './checkout.html',
  styleUrl: './checkout.scss',
})
export class Checkout {
  private fb = inject(FormBuilder);
  private http = inject(HttpClient);
  protected cartService = inject(CartService);
  private router = inject(Router);

  private readonly apiUrl = 'http://localhost:5231/api';

  form = this.fb.nonNullable.group({
    customerName: ['', [Validators.required, Validators.maxLength(120)]],
    phone: ['', [Validators.required, Validators.maxLength(40)]],
    address: ['', [Validators.required, Validators.maxLength(200)]],
    city: ['', [Validators.required, Validators.maxLength(100)]],
  });

  isSubmitting = false;
  successMessage = '';
  errorMessage = '';

  submit(): void {
    this.successMessage = '';
    this.errorMessage = '';

    if (this.cartService.lines().length === 0) {
      this.errorMessage = 'Your cart is empty.';
      return;
    }

    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const v = this.form.getRawValue();

    const payload = {
      customerName: v.customerName.trim(),
      phone: v.phone.trim(),
      address: v.address.trim(),
      city: v.city.trim(),
      items: this.cartService.lines().map((l) => ({
        productId: l.productId,
        quantity: l.quantity,
      })),
    };

    this.isSubmitting = true;

    this.http.post<{ id: number; total: number }>(`${this.apiUrl}/orders`, payload).subscribe({
      next: (res) => {
        this.cartService.clear();
        this.successMessage = `Order placed successfully. Order #${res.id}. Total: ${res.total.toFixed(2)} EUR.`;
        this.form.reset();
        this.isSubmitting = false;
      },
      error: (err) => {
        const msg =
          err?.error?.message ??
          (typeof err?.error === 'string' ? err.error : null) ??
          'Could not place order.';
        this.errorMessage = msg;
        this.isSubmitting = false;
      },
    });
  }

  goShop(): void {
    void this.router.navigate(['/shop']);
  }
}