import { Routes } from '@angular/router';
import { Home} from './pages/home/home';
import {Shop} from './pages/shop/shop';
import { ProductDetails } from './pages/product-details/product-details';
import { Cart } from './pages/cart/cart';
import { Checkout } from './pages/checkout/checkout';
export const routes: Routes = [
      { path: '', component: Home },
    { path: 'shop', component: Shop },
    { path: 'product/:id', component: ProductDetails },
    { path: 'cart', component: Cart },
    { path: 'checkout', component: Checkout },
];
