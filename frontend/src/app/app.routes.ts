import { Routes } from '@angular/router';
import { Home} from './pages/home/home';
import {Shop} from './pages/shop/shop';
import { Login } from './pages/login/login';
import { Admin } from './pages/admin/admin';
import { authGuard } from './core/auth.guard';

// u routes:

import { ProductDetails } from './pages/product-details/product-details';
import { Cart } from './pages/cart/cart';
import { Checkout } from './pages/checkout/checkout';
export const routes: Routes = [
      { path: '', component: Home },
    { path: 'shop', component: Shop },
    { path: 'product/:id', component: ProductDetails },
    { path: 'cart', component: Cart },
    { path: 'checkout', component: Checkout },
    { path: 'login', component: Login },
    { path: 'admin', component: Admin, canActivate: [authGuard] },
];
