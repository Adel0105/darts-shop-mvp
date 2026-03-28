import { ChangeDetectorRef, Component, OnInit, inject } from '@angular/core';
import { CommonModule, CurrencyPipe } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { ProductService } from '../../core/product.service';
import { Product } from '../../core/models/product.model';
import { CartService } from '../../core/cart.service';
@Component({
  selector: 'app-product-details',
  standalone: true,
  imports: [CommonModule, RouterLink, CurrencyPipe],
  templateUrl: './product-details.html',
  styleUrl: './product-details.scss',
})
export class ProductDetails implements OnInit {
  private route = inject(ActivatedRoute);
  private productService = inject(ProductService);
  private cdr = inject(ChangeDetectorRef);
  private cartService=inject(CartService);
  product: Product | null = null;
  isLoading = true;
  error = '';
  notFound = false;

  ngOnInit(): void {
    const idParam = this.route.snapshot.paramMap.get('id');
    const id = idParam ? Number(idParam) : NaN;

    if (Number.isNaN(id) || id < 1) {
      this.notFound = true;
      this.isLoading = false;
      return;
    }

    this.productService.getProductById(id).subscribe({
      next: (p) => {
        this.product = p;
        this.isLoading = false;
        this.cdr.markForCheck();
      },
      error: (err) => {
        this.isLoading = false;
        if (err.status === 404) {
          this.notFound = true;
        } else {
          this.error = 'Greška pri učitavanju proizvoda.';
        }
        this.cdr.markForCheck();
      },
    });
  }
  addToCart(): void {
  if (!this.product || this.product.stock < 1) return;
  this.cartService.add(this.product, 1);
  }
  
}