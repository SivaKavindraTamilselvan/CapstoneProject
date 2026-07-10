import { Routes } from '@angular/router';
import { AddAddress } from '../address/add-address/add-address';
import { ProfilePage } from '../profile-page/profile-page';

export const SharedRoutes: Routes = [
  {
    path: 'add-address',
    component: AddAddress,
  },
];