import { Component, signal, computed } from '@angular/core';
import { UserProductService } from '../../services/user-product.Service';
import { UserProductModel } from '../../models/user/product/user-product.model';
import { ActivatedRoute } from '@angular/router';
import { CommonModule } from '@angular/common';
import { UserProductVariantModel } from '../../models/user/product/user-variant.model';

@Component({
  selector: 'app-user-product-details',
  imports: [CommonModule],
  templateUrl: './user-product-details.html',
  styleUrl: './user-product-details.css',
})
export class UserProductDetails {
  product = signal<UserProductModel | null>(null);
  currentImageIndex = signal(0);
  selectedVariant = signal<UserProductVariantModel | null>(null);
  selectedMainAttrValue = signal<string | null>(null);

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

  constructor(private userProductService: UserProductService, private route: ActivatedRoute) {}

  ngOnInit() {
    const productId = Number(this.route.snapshot.paramMap.get('id'));
    if (productId) this.loadProduct(productId);
  }

  loadProduct(productId: number) {
    this.userProductService.getProductDetails(productId).subscribe({
      next: (response: any) => {
        this.product.set(response);
        console.log(response);
      },
      error: (error) => console.error(error)
    });
  }

  selectMainAttr(value: string) {
    this.selectedMainAttrValue.set(value);
    this.selectedVariant.set(null);
  }

  selectVariant(variant: UserProductVariantModel) {
    if (!variant.isAvailableForSale) return;
    this.selectedVariant.set(variant);
  }

  prevImage() {
    const total = this.product()?.productImages?.length ?? 0;
    this.currentImageIndex.update(i => (i - 1 + total) % total);
  }

  nextImage() {
    const total = this.product()?.productImages?.length ?? 0;
    this.currentImageIndex.update(i => (i + 1) % total);
  }
}