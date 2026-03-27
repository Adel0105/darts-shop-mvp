import { Component, OnInit,OnDestroy, inject } from '@angular/core';
import { CommonModule, CurrencyPipe } from '@angular/common';
import { RouterLink } from '@angular/router';
import { ProductService } from '../../core/product.service';
import { Product } from '../../core/models/product.model';
import { Category } from '../../core/models/category.model';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged,takeUntil } from 'rxjs/operators';
@Component({
  selector: 'app-shop',
  standalone: true,
  imports: [CommonModule, RouterLink, CurrencyPipe],
  templateUrl: './shop.html',
  styleUrl: './shop.scss',
})
export class Shop implements OnInit,OnDestroy {
  private destroy$=new Subject<void>();
  private productService = inject(ProductService);
  sortOption:'none' | 'priceAsc' | 'priceDesc' | 'nameAsc'='none';
  products: Product[] = [];
  filteredProducts:Product[]=[];
  categories:Category[]=[];
  selectedCategoryId:number | null=null;
  isLoading = true;
  error = '';
  searchText = '';      // odmah u inputu
  searchQuery = ''; 
  private searchSubject = new Subject<string>();
  
  ngOnInit(): void {
    this.searchSubject
  .pipe(
    debounceTime(300),
    distinctUntilChanged(),
    takeUntil(this.destroy$)
  )
  .subscribe((q) => {
    this.searchQuery = q.trim();
    this.applyFilter();
  });
    this.loadProducts();
  }
  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }
  private loadProducts(): void {

    this.isLoading=true;

    this.error='';

    this.productService.getCategories().subscribe({
      next:(cats)=>{
        this.categories=cats;
      },
      error:()=>{
        this.error="Greska pri ucitavanju kategorija!";
      },

    });
    this.productService.getProducts().subscribe({
      next: (data) => {
        this.products = data;
        this.applyFilter();
        this.isLoading = false;
      },
      error: () => {
        this.error = 'Greška pri učitavanju proizvoda.';
        this.isLoading = false;
      },
    });
  }
  onCategoryChange(value:string): void{
    this.selectedCategoryId=value? Number(value):null;
    this.applyFilter();
  }
  clearCategoryFilter(): void {
    this.selectedCategoryId = null;
    this.sortOption='none';
    this.searchQuery='';
    this.searchText='';
    this.searchSubject.next('');
    this.applyFilter();
  }
  private applyFilter():void{
    let result:Product[];
    if(!this.selectedCategoryId){
      result=[...this.products];
    }else{result=this.products.filter(
      (p)=>p.categoryId===this.selectedCategoryId
    );
    }
    if(this.searchQuery){
      const q=this.searchQuery.toLocaleLowerCase();
      result=result.filter((p)=>p.name.toLocaleLowerCase().includes(q));
    }
    //sortiranje 
    if(this.sortOption==='priceAsc'){result=[...result].sort((a,b)=>a.price-b.price)}
    else if(this.sortOption==='priceDesc'){result=[...result.sort((a,b)=>b.price-a.price)]}
    else if (this.sortOption === 'nameAsc') {
    result = [...result].sort((a, b) => a.name.localeCompare(b.name));
  }
  this.filteredProducts=result;
  }

  onSortChange(value:string):void{
    this.sortOption=(value as 'none' | 'priceAsc' | 'priceDesc' |'nameAsc');
    this.applyFilter();
  }

  onSearchChange(value: string): void {
    this.searchText=value;
    this.searchSubject.next(value);
  }

}