import { Component, signal, computed } from '@angular/core';
import { UserProductService } from '../../services/user-product.Service';
import { UserProductModel } from '../../models/user/product/user-product.model';
import { ActivatedRoute, Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { UserProductVariantModel } from '../../models/user/product/user-variant.model';
import { UserCartService } from '../../services/user-cart.Service';
import { AddCartItemModel } from '../../models/user/cart/add-cart,model';
import { UserFavoriteService } from '../../services/user-favorite.Service';
import { AddFavoriteItemModel } from '../../models/user/favorites/add-favorite.model';
import { RemoveFavoriteItemModel } from '../../models/user/favorites/remove-favorite.model';
import { ProductReviews } from '../user-product/product-reviews/product-reviews';
import { AuthService } from '../../services/auth.Service'; // confirm actual path
import { AuthStateService } from '../../services/auth-State.Service';

@Component({
  selector: 'app-user-product-details',
  imports: [CommonModule, ProductReviews],
  templateUrl: './user-product-details.html',
  styleUrl: './user-product-details.css',
})
export class UserProductDetails {
  product = signal<UserProductModel | null>(null);
  currentImageIndex = signal(0);
  selectedVariant = signal<UserProductVariantModel | null>(null);
  selectedMainAttrValue = signal<string | null>(null);

  errorMessage = signal<string | null>(null);
  successMessage = signal<string | null>(null);

  cartVariantIds = signal<Set<number>>(new Set());
  favoriteVariantIds = signal<Set<number>>(new Set());

  cartLoading = signal(false);
  favoriteLoading = signal(false);

  uniqueMainAttrValues = computed(() => {
    const p = this.product();
    if (!p) return [];
    const seen = new Set<string>();
    const result: string[] = [];
    p.productVariants.forEach(v => {
      const attr = v.attributes.find(a => a.attributeName === p.mainProductSubCategoryAttributeName);
      if (attr && !seen.has(attr.attributeValue)) {
        seen.add(attr.attributeValue);
        result.push(attr.attributeValue);
      }
    });
    return result;
  });

  filteredVariants = computed(() => {
    const p = this.product();
    const attrVal = this.selectedMainAttrValue();
    if (!p) return [];
    if (!attrVal) return p.productVariants;
    return p.productVariants.filter(v =>
      v.attributes.find(a =>
        a.attributeName === p.mainProductSubCategoryAttributeName &&
        a.attributeValue === attrVal
      )
    );
  });

  displayImages = computed(() => {
    const p = this.product();
    const variant = this.selectedVariant();
    if (!p) return [];

    const productImgs = p.productImages ?? [];
    const variantImgs = variant?.productImages ?? [];

    // Variant images first (more specific), then product-level images.
    // Dedup by imageUrl in case of overlap.
    const combined = [...variantImgs, ...productImgs];
    const seen = new Set<string>();
    return combined.filter(img => {
      if (seen.has(img.imageUrl)) return false;
      seen.add(img.imageUrl);
      return true;
    });
  });

  hasAvailableVariantForAttr(attrValue: string): boolean {
    const p = this.product();
    if (!p) return false;
    return p.productVariants.some(v =>
      v.isAvailableForSale &&
      v.attributes.find(a =>
        a.attributeName === p.mainProductSubCategoryAttributeName &&
        a.attributeValue === attrValue
      )
    );
  }

  otherAttributes(variant: UserProductVariantModel): string {
    const p = this.product();
    if (!p) return '';
    return variant.attributes
      .filter(a => a.attributeName !== p.mainProductSubCategoryAttributeName)
      .map(a => a.attributeValue)
      .join(' · ') || 'Base';
  }

  constructor(
    private userFavoriteService: UserFavoriteService,
    private userProductService: UserProductService,
    private route: ActivatedRoute,
    private userCartService: UserCartService,
    private router: Router,
    private authService: AuthStateService
  ) { }

  ngOnInit() {
    const productId = Number(this.route.snapshot.paramMap.get('id'));
    if (productId) this.loadProduct(productId);
    this.loadCartStatus();
    this.loadFavoriteStatus();
  }

  isLoggedIn(): boolean {
    return this.authService.isLoggedIn();
  }

  loadCartStatus(): void {
    if (!this.isLoggedIn()) {
      this.cartVariantIds.set(new Set());
      return;
    }
    this.cartLoading.set(true);
    this.userCartService.getCartItems().subscribe({
      next: (items: any) => {
        const list = items?.items ?? items ?? [];
        const ids = new Set<number>(list.map((i: any) => i.productVariantId));
        this.cartVariantIds.set(ids);
        this.cartLoading.set(false);
      },
      error: (err) => {
        console.error('Failed to load cart status', err);
        this.cartLoading.set(false);
      }
    });
  }

  loadFavoriteStatus(): void {
    if (!this.isLoggedIn()) {
      this.favoriteVariantIds.set(new Set());
      return;
    }
    this.favoriteLoading.set(true);
    this.userFavoriteService.getFavoriteItems().subscribe({
      next: (items: any) => {
        const list = items?.items ?? items ?? [];
        const ids = new Set<number>(list.map((i: any) => i.productVariantId));
        this.favoriteVariantIds.set(ids);
        this.favoriteLoading.set(false);
      },
      error: (err) => {
        console.error('Failed to load favorite status', err);
        this.favoriteLoading.set(false);
      }
    });
  }

  isInCart(variantId: number): boolean {
    return this.cartVariantIds().has(variantId);
  }

  isInFavorite(variantId: number): boolean {
    return this.favoriteVariantIds().has(variantId);
  }

  loadProduct(productId: number) {
    this.userProductService.getProductDetails(productId).subscribe({
      next: (response: any) => {
        this.product.set(response);
        console.log(response);

        const firstMainAttr = response.productVariants
          ?.find((v: UserProductVariantModel) =>
            v.attributes.find((a: any) => a.attributeName === response.mainProductSubCategoryAttributeName)
          )
          ?.attributes.find((a: any) => a.attributeName === response.mainProductSubCategoryAttributeName)
          ?.attributeValue ?? null;

        this.selectedMainAttrValue.set(firstMainAttr);

        const firstVariant = response.productVariants?.find(
          (v: UserProductVariantModel) => v.isAvailableForSale &&
            v.attributes.find((a: any) =>
              a.attributeName === response.mainProductSubCategoryAttributeName &&
              a.attributeValue === firstMainAttr
            )
        ) ?? null;
        this.selectedVariant.set(firstVariant);
      },
      error: (error) => console.error(error)
    });
  }

  selectVariant(variant: UserProductVariantModel) {
    if (!variant.isAvailableForSale) return;
    this.selectedVariant.set(variant);
    this.currentImageIndex.set(0);
  }

  selectMainAttr(value: string) {
    this.selectedMainAttrValue.set(value);
    const firstMatch = this.filteredVariants().find(v => v.isAvailableForSale) ?? null;
    this.selectedVariant.set(firstMatch);
    this.currentImageIndex.set(0);
  }

  prevImage() {
    const total = this.displayImages().length;
    this.currentImageIndex.update(i => (i - 1 + total) % total);
  }

  nextImage() {
    const total = this.displayImages().length;
    this.currentImageIndex.update(i => (i + 1) % total);
  }

  addToCart() {
    this.errorMessage.set(null);
    this.successMessage.set(null);

    if (!this.isLoggedIn()) {
      this.goToLogin();
      return;
    }

    const variant = this.selectedVariant();
    if (!variant) return;

    const addmodel = new AddCartItemModel();
    addmodel.productVariantId = variant.productVariantId;

    this.userCartService.addToCart(addmodel).subscribe({
      next: () => {
        this.cartVariantIds.update(ids => {
          const updated = new Set(ids);
          updated.add(variant.productVariantId);
          return updated;
        });
        this.successMessage.set('Added to cart');
      },
      error: (err) => {
        console.error(err);
        this.errorMessage.set(err.error?.message ?? 'Failed to add to cart');
      }
    });
  }

  addToFavorites() {
    this.errorMessage.set(null);
    this.successMessage.set(null);

    if (!this.isLoggedIn()) {
      this.goToLogin();
      return;
    }

    const variant = this.selectedVariant();
    if (!variant) return;

    const addmodel = new AddFavoriteItemModel();
    addmodel.productVariantId = variant.productVariantId;

    this.userFavoriteService.addToFavorite(addmodel).subscribe({
      next: () => {
        this.favoriteVariantIds.update(ids => {
          const updated = new Set(ids);
          updated.add(variant.productVariantId);
          return updated;
        });
        this.successMessage.set('Added to favorites');
      },
      error: (err) => {
        console.error(err);
        this.errorMessage.set(err.error?.message ?? 'Failed to add to favorites');
      }
    });
  }

  goToLogin(): void {
    this.router.navigate(['/login'], {
      queryParams: { returnUrl: this.router.url }
    });
  }

  goToCart(): void {
    this.router.navigate(['/user/cart']); // adjust to your actual route
  }

  goToFavorites(): void {
    this.router.navigate(['/user/favorite']); // adjust to your actual route
  }

  goBack(): void {
    this.router.navigate(['/user/products']);
  }
}