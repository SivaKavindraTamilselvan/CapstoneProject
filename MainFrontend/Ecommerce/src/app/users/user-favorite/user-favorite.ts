import { Component, computed, signal } from '@angular/core';
import { Router } from '@angular/router';
import { UserProductService } from '../../services/user-product.Service';
import { FavoriteItemModel } from '../../models/user/favorites/user-favorite.model';
import { UserFavoriteService } from '../../services/user-favorite.Service';
import { RemoveFavoriteItemModel } from '../../models/user/favorites/remove-favorite.model';
import { CommonModule } from '@angular/common';
import { UserCartService } from '../../services/user-cart.Service';
import { AddCartItemModel } from '../../models/user/cart/add-cart,model';

@Component({
  selector: 'app-user-favorite',
  imports: [CommonModule],
  templateUrl: './user-favorite.html',
  styleUrl: './user-favorite.css',
})
export class UserFavorite {
  favoriteItems = signal<FavoriteItemModel[]>([]);
  isLoading = signal(false);
  error = signal<string | null>(null);

  constructor(
    private userFavoriteService: UserFavoriteService, 
    private router: Router, 
    private userProductService: UserProductService,
    private userCartService: UserCartService
  ) { }

  ngOnInit() {
    this.loadFavorite();
  }

  loadFavorite() {
    this.isLoading.set(true);
    this.error.set(null);
    this.userFavoriteService.getFavoriteItems().subscribe({
      next: (response) => {
        console.log(response);
        this.favoriteItems.set(response);
        this.isLoading.set(false);
      },
      error: (err) => {
        console.error(err);
        this.favoriteItems.set([]);
        this.isLoading.set(false);
      }
    });
  }

  removeItem(cartItemsId: number) {
    const request = new RemoveFavoriteItemModel(cartItemsId);

    this.userFavoriteService.removeFromFavorite(request).subscribe({
      next: () => {
        this.favoriteItems.update(items =>
          items.filter(i => i.favoritesItemsId !== cartItemsId)
        );
      },
      error: (err) => console.error(err)
    });
  }

  goToProductDetails(productId: any) {
    this.router.navigate(['/user/product-details', productId]);
  }

  addToCart(productVariantId: number) {
    const addmodel = new AddCartItemModel();
    addmodel.productVariantId = productVariantId;
    this.userCartService.addToCart(addmodel).subscribe({
      next: () => console.log('Added to cart'),
      error: (err) => console.error(err)
    });
  }
}
