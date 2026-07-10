import { Component, signal } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { UserSubProductCategoryModel } from '../../models/user/product-category/user-sub-category.model';
import { UserProductService } from '../../services/user-product.Service';

@Component({
  selector: 'app-sub-category-mobile',
  imports: [],
  templateUrl: './sub-category-mobile.html',
  styleUrl: './sub-category-mobile.css',
})
export class SubCategoryMobile {
  selectedProductCategory = signal<number | null>(null);
  subProductCategoryModel = signal<UserSubProductCategoryModel[]>([]);

  constructor(private userProductService: UserProductService, private route: ActivatedRoute, private router: Router) {

  }


  ngOnInit() {
    const categoryId = Number(this.route.snapshot.paramMap.get('categoryId'));
    this.selectedProductCategory.set(categoryId);
    this.loadSubCategory();
  }

  loadSubCategory() {
    const categoryId = this.selectedProductCategory();
    if (categoryId === null) {
      return;
    }
    this.userProductService.getSubCategory(categoryId).subscribe({
      next: (response: any) => {
        this.subProductCategoryModel.set(response);
      },
      error: (error) => {
        console.error(error);
      },
    });
  }
  goToProducts(subCategoryId: number): void {
    this.router.navigate(['/user/subcategory', subCategoryId, 'products']);
  }
}