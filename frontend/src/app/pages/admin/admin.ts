import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
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
  imports: [CommonModule],
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
    this.loadProducts();
    this.loadOrders();
  }

  loadProducts(): void {
    this.error = '';
    this.success = '';
    this.http.get<Product[]>(`${this.apiUrl}/products`).subscribe({
      next: (data) => {
        this.products = data;
        this.stockDraft = {};
        for (const p of data) this.stockDraft[p.id] = p.stock;
      },
      error: () => {
        this.error = 'Greška pri učitavanju proizvoda.';
      },
    });
  }
saveStock(productId: number): void {
  this.error = '';
  this.success = '';
  if (this.isSavingStock[productId]) return;
  const newStock = Number(this.stockDraft[productId]);
  if (Number.isNaN(newStock) || newStock < 0) {
    this.error = 'Stock mora biti broj >= 0.';
    return;
  }

  const ok = confirm(`Potvrdi promjenu stock-a na: ${newStock}?`);
  if (!ok) return;

  this.isSavingStock[productId] = true;
  this.http
    .patch<{ id: number; stock: number }>(
      `${this.apiUrl}/admin/products/${productId}/stock`,
      { stock: newStock }
    )
    .subscribe({
      next: () => {
        this.success = 'Stock uspješno ažuriran.';

        const p = this.products.find(x => x.id === productId);
        if (p) {
          p.stock = newStock;              // odmah update u tabeli
          this.stockDraft[productId] = newStock; // da dugme postane disabled
        }
        this.isSavingStock[productId] = false;
        this.cdr.detectChanges();
      },
      error: () => {
        this.error = 'Greška pri ažuriranju stock-a.';
        this.isSavingStock[productId] = false;
      }
    });
}
  

loadOrders(): void {
  this.error = '';
  this.success = '';
  this.http.get<AdminOrder[]>(`${this.apiUrl}/admin/orders`).subscribe({
    next: (data) => {
      this.orders = data;
      this.statusDraft = {};
      for (const o of data) this.statusDraft[o.id] = o.status;
    },
    error: () => {
      this.error = 'Greška pri učitavanju narudžbi.';
    },
  });
}

saveOrderStatus(orderId: number): void {
  this.error = '';
  this.success = '';

  const status = this.statusDraft[orderId];
  if (!status) {
    this.error = 'Odaberi status.';
    return;
  }

  const ok = confirm(`Potvrdi promjenu statusa na: ${status}?`);
  if (!ok) return;

  this.http
    .patch<{ id: number; status: string }>(
      `${this.apiUrl}/admin/orders/${orderId}/status`,
      { status }
    )
    .subscribe({
      next: () => {
        this.success = 'Status narudžbe ažuriran.';

        const o = this.orders.find(x => x.id === orderId);
        if (o) {
          o.status = status;               // odmah update u tabeli
          this.statusDraft[orderId] = status; // da dugme postane disabled
        }
        this.cdr.detectChanges();
      },
      error: () => {
        this.error = 'Greška pri ažuriranju statusa.';
      }
    });
}
}