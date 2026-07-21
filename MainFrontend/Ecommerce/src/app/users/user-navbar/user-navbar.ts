import { Component, signal } from '@angular/core';
import { UserProductService } from '../../services/user-product.Service';
import { UserProductCategoryModel } from '../../models/user/product-category/user-product-category.model';
import { UserSubProductCategoryModel } from '../../models/user/product-category/user-sub-category.model';
import { NavigationEnd, Router, RouterLink, RouterOutlet } from '@angular/router';
import { AuthStateService } from '../../services/auth-State.Service';
import { NgClass } from '@angular/common';
import { filter } from 'rxjs';

@Component({
  selector: 'app-user-navbar',
  imports: [RouterOutlet, NgClass, RouterLink],
  templateUrl: './user-navbar.html',
  styleUrl: './user-navbar.css',
})
export class UserNavbar {
  mobileSearchOpen = signal(false);
  showSearch = signal(true);
  selectedProductCategory = signal<number | null>(null);
  productCategoryModel = signal<UserProductCategoryModel[]>([]);
  subProductCategoryModel = signal<UserSubProductCategoryModel[]>([]);

  constructor(private userProductService: UserProductService, private router: Router, public authState: AuthStateService) {
    this.router.events.pipe(
      filter((event): event is NavigationEnd => event instanceof NavigationEnd)
    ).subscribe((event) => {
      const isCategoryOrSubcategoryPage =
        event.urlAfterRedirects.includes('/user/category/') ||
        event.urlAfterRedirects.includes('/user/subcategory/');

      const isPlainProductsPage = event.urlAfterRedirects.includes('/user/products');

      this.showSearch.set(isCategoryOrSubcategoryPage || isPlainProductsPage);

      if (!isCategoryOrSubcategoryPage) {
        this.selectedProductCategory.set(null);
        this.subProductCategoryModel.set([]);
      }
    });
  }

  ngOnInit() {
    this.loadProductCategory();
    this.authState.validateSession();
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

  logout() {
    this.authState.logout();
    this.router.navigate(["/login"]);
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
      }
    })
  }

  onSearch(event: Event): void {
    const value = (event.target as HTMLInputElement).value;
    this.userProductService.navbarSearchTerm.set(value);
  }

  selectCategory(categoryId: number) {
    this.selectedProductCategory.set(categoryId);
    this.loadSubCategory();
  }

  goToCategories(): void {
    this.router.navigate(['/user/categories']);
  }

  goToLogin() {
    this.router.navigate(["/login"]);
  }

  goToHome(): void {
    this.router.navigate(['/']);
  }

  goToCart(): void {
    this.router.navigate(['/user/cart']);
  }

  goToFavorites(): void {
    this.router.navigate(['/user/favorite']);
  }

  goToOrders(): void {
    this.router.navigate(['/user/orders']);
  }

  goToProfile(): void {
    this.router.navigate(['/user/profile']);
  }

  goToNotifications(): void {
    this.router.navigate(['/user/notification']);
  }

  goToProducts(subCategoryId: number): void {
    this.router.navigate(['/user/subcategory', subCategoryId, 'products']);
  }

  goToCategoryProducts(categoryId: number): void {
    this.router.navigate(['/user/category', categoryId, 'products']);
  }

  onCategoryClick(categoryId: number): void {
    this.selectCategory(categoryId);
    this.goToCategoryProducts(categoryId);
  }
}