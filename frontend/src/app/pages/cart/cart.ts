import { Component, inject } from '@angular/core';
import { CommonModule, CurrencyPipe } from '@angular/common';
import { RouterLink } from '@angular/router';
import { CartService } from '../../core/cart.service';

@Component({
  selector: 'app-cart',
  standalone: true,
  imports: [CommonModule, RouterLink, CurrencyPipe],
  templateUrl: './cart.html',
  styleUrl: './cart.scss',
})
export class Cart {
  protected cartService = inject(CartService);

  inc(productId: number, current: number, max: number): void {
    if (current >= max) return;
    this.cartService.setQuantity(productId, current + 1);
  }

  dec(productId: number, current: number): void {
    if (current <= 1) {
      this.cartService.remove(productId);
    } else {
      this.cartService.setQuantity(productId, current - 1);
    }
  }

  remove(productId: number): void {
    this.cartService.remove(productId);
  }
}
