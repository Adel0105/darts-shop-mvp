import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { RouterLink } from '@angular/router';
import { Product } from '../../core/models/product.model';
import { ChangeDetectorRef } from '@angular/core';

type AdminOrderItem = {
  productId: number;
  quantity: number;
  unitPrice: number;
};

type AdminOrder = {
  id: number;
  customerName: string;
  phone: string;
  address: string;
  city: string;
  createdAt: string;
  total: number;
  status: 'New' | 'Processing' | 'Done' | string;
  items: AdminOrderItem[];
};

@Component({
  selector: 'app-admin',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './admin.html',
  styleUrl: './admin.scss',
})
export class Admin implements OnInit {
  private http = inject(HttpClient);
  private apiUrl = 'http://localhost:5231/api';

  products: Product[] = [];
  stockDraft: Record<number, number> = {};
  error = '';
  success = '';
  orders: AdminOrder[] = [];
  statusDraft: Record<number, string> = {};
  isSavingStock: Record<number, boolean> = {};
  private cdr = inject(ChangeDetectorRef);

  ngOnInit(): void {
    this.error = '';
    this.success = '';
    this.loadProducts();
    this.loadOrders();
  }

  onStockInput(productId: number, event: Event): void {
    const el = event.target as HTMLInputElement;
    const v = Number(el.value);
    if (!Number.isNaN(v) && v >= 0) {
      this.stockDraft[productId] = v;
    }
  }

  onStatusChange(orderId: number, event: Event): void {
    const el = event.target as HTMLSelectElement;
    this.statusDraft[orderId] = el.value;
  }

  loadProducts(): void {
    this.http.get<Product[]>(`${this.apiUrl}/products`).subscribe({
      next: (data) => {
        this.products = data;
        this.stockDraft = {};
        for (const p of data) {
          this.stockDraft[p.id] = p.stock;
        }
        this.cdr.markForCheck();
      },
      error: () => {
        this.error = 'Could not load products.';
      },
    });
  }

  saveStock(productId: number): void {
    this.error = '';
    this.success = '';
    if (this.isSavingStock[productId]) return;
    const newStock = Number(this.stockDraft[productId]);
    if (Number.isNaN(newStock) || newStock < 0) {
      this.error = 'Stock must be a number ≥ 0.';
      return;
    }

    const ok = confirm(`Set stock to ${newStock}?`);
    if (!ok) return;

    this.isSavingStock[productId] = true;
    this.http
      .patch<{ id: number; stock: number }>(
        `${this.apiUrl}/admin/products/${productId}/stock`,
        { stock: newStock }
      )
      .subscribe({
        next: () => {
          this.success = 'Stock updated.';

          const p = this.products.find((x) => x.id === productId);
          if (p) {
            p.stock = newStock;
            this.stockDraft[productId] = newStock;
          }
          this.isSavingStock[productId] = false;
          this.cdr.detectChanges();
        },
        error: () => {
          this.error = 'Could not update stock. Are you logged in as admin?';
          this.isSavingStock[productId] = false;
        },
      });
  }

  loadOrders(): void {
    this.http.get<AdminOrder[]>(`${this.apiUrl}/admin/orders`).subscribe({
      next: (data) => {
        this.orders = data;
        this.statusDraft = {};
        for (const o of data) {
          this.statusDraft[o.id] = o.status;
        }
        this.cdr.markForCheck();
      },
      error: () => {
        this.error = 'Could not load orders. Are you logged in as admin?';
      },
    });
  }

  saveOrderStatus(orderId: number): void {
    this.error = '';
    this.success = '';

    const status = this.statusDraft[orderId];
    if (!status) {
      this.error = 'Select a status.';
      return;
    }

    const ok = confirm(`Set order status to "${status}"?`);
    if (!ok) return;

    this.http
      .patch<{ id: number; status: string }>(
        `${this.apiUrl}/admin/orders/${orderId}/status`,
        { status }
      )
      .subscribe({
        next: () => {
          this.success = 'Order status updated.';

          const o = this.orders.find((x) => x.id === orderId);
          if (o) {
            o.status = status;
            this.statusDraft[orderId] = status;
          }
          this.cdr.detectChanges();
        },
        error: () => {
          this.error = 'Could not update order status.';
        },
      });
  }
}
