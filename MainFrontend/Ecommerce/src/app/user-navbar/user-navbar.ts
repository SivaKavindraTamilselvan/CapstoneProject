import { Component, signal } from '@angular/core';
import { UserProductService } from '../services/user-product.Service';
import { UserProductCategoryModel } from '../models/user-product-category.model';
import { UserSubProductCategoryModel } from '../models/user-sub-category.model';

@Component({
  selector: 'app-user-navbar',
  imports: [],
  templateUrl: './user-navbar.html',
  styleUrl: './user-navbar.css',
})
export class UserNavbar {
  mobileSearchOpen = signal(false);
  selectedProductCategory = signal<number | null>(null);
  productCategoryModel = signal<UserProductCategoryModel[]>([]);
  subProductCategoryModel = signal<UserSubProductCategoryModel[]>([]);
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
  loadSubCategory() {
    const categoryId = this.selectedProductCategory();

    if (categoryId === null) {
      return;
    }
    this.userProductService.getSubCategory(categoryId).subscribe({
      next: (response: any)=>{
        this.subProductCategoryModel.set(response);
      },
      error: (error) => {
        console.error(error);
      }
    })
  }
  selectCategory(categoryId: number) {
    this.selectedProductCategory.set(categoryId);
    this.loadSubCategory();
  }
}
