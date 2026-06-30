import { Component, signal } from '@angular/core';
import { UserProductService } from '../../services/user-product.Service';
import { UserProductCategoryModel } from '../../models/user/product-category/user-product-category.model';
import { UserProduct } from '../user-product/user-product';

@Component({
  selector: 'app-user-home',
  imports: [],
  templateUrl: './user-home.html',
  styleUrl: './user-home.css',
})
export class UserHome {
  productCategoryModel = signal<UserProductCategoryModel[]>([]);
  constructor(private userProductService:UserProductService){

  }
}
