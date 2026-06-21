import { Component, signal } from '@angular/core';
import { UserProductService } from '../services/user-product.Service';
import { UserProductCategoryModel } from '../models/user-product-category.model';

@Component({
  selector: 'app-user-navbar',
  imports: [],
  templateUrl: './user-navbar.html',
  styleUrl: './user-navbar.css',
})
export class UserNavbar {
  productCategoryModel = signal<UserProductCategoryModel[]>([]);
  constructor(private userProductService: UserProductService) {

  }
  ngOnInit() {
    this.loadProductCategory();
  }
  loadProductCategory() {
    this.userProductService.getProductCategory().subscribe({
      next: (response: any) => {
        this.productCategoryModel.set(response);
        console.log(response);
      },
      error: (error) => {
        console.error(error);
      }
    });
  }
}
