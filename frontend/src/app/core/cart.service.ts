import { Injectable, computed, signal } from '@angular/core';
import { Product } from './models/product.model';
import { CartLine } from './models/cart-line.model';

const STORAGE_KEY = 'darts-shop-cart';

@Injectable({ providedIn: 'root' })
export class CartService {
  private readonly _lines = signal<CartLine[]>(this.loadFromStorage());

  readonly lines = this._lines.asReadonly();

  readonly itemCount = computed(() =>
    this._lines().reduce((sum, l) => sum + l.quantity, 0)
  );

  readonly subtotal = computed(() =>
    this._lines().reduce((sum, l) => sum + l.unitPrice * l.quantity, 0)
  );

  add(product: Product, qty = 1): void {
    if (product.stock < 1) return;

    const nextQty = Math.max(1, Math.floor(qty));
    const lines = [...this._lines()];
    const idx = lines.findIndex((l) => l.productId === product.id);

    if (idx >= 0) {
      const line = { ...lines[idx] };
      line.quantity = Math.min(line.quantity + nextQty, line.maxStock);
      lines[idx] = line;
    } else {
      lines.push({
        productId: product.id,
        name: product.name,
        imageUrl: product.imageUrl,
        unitPrice: product.price,
        quantity: Math.min(nextQty, product.stock),
        maxStock: product.stock,
      });
    }

    this.setLines(lines);
  }

  setQuantity(productId: number, qty: number): void {
    const q = Math.max(1, Math.floor(qty));
    const lines = this._lines()
      .map((l) =>
        l.productId === productId
          ? { ...l, quantity: Math.min(q, l.maxStock) }
          : l
      )
      .filter((l) => l.quantity > 0);

    this.setLines(lines);
  }

  remove(productId: number): void {
    this.setLines(this._lines().filter((l) => l.productId !== productId));
  }

  clear(): void {
    this.setLines([]);
  }

  private setLines(lines: CartLine[]): void {
    this._lines.set(lines);
    try {
      localStorage.setItem(STORAGE_KEY, JSON.stringify(lines));
    } catch {
      /* ignore quota / private mode */
    }
  }

  private loadFromStorage(): CartLine[] {
    try {
      const raw = localStorage.getItem(STORAGE_KEY);
      if (!raw) return [];
      const parsed = JSON.parse(raw) as CartLine[];
      return Array.isArray(parsed) ? parsed : [];
    } catch {
      return [];
    }
  }
}