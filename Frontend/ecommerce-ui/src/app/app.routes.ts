import { Routes } from '@angular/router';
import { PaymentComponent } from './pages/payment/payment';

export const routes: Routes = [
  {
    path: 'payment',
    component: PaymentComponent
  },
  {
    path: '',
    redirectTo: 'payment',
    pathMatch: 'full'
  }
];