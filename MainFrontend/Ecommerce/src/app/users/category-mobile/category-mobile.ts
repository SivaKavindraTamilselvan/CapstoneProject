import { Component, signal } from '@angular/core';
import { UserNavbar } from '../user-navbar/user-navbar';
import { UserProductCategoryModel } from '../../models/user/product-category/user-product-category.model';
import { UserSubProductCategoryModel } from '../../models/user/product-category/user-sub-category.model';
import { UserProductService } from '../../services/user-product.Service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-category-mobile',
  imports: [UserNavbar],
  templateUrl: './category-mobile.html',
  styleUrl: './category-mobile.css',
})
export class CategoryMobile {
  selectedProductCategory = signal<number | null>(null);
  productCategoryModel = signal<UserProductCategoryModel[]>([]);
  subProductCategoryModel = signal<UserSubProductCategoryModel[]>([]);
  constructor(private userProductService: UserProductService, private router: Router) {

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

  selectCategory(categoryId: number) {
    this.router.navigate(['/categories', categoryId, 'subcategories']);
  }
}
