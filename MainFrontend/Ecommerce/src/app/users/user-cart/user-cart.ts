import { Component, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { UserCartService } from '../../services/user-cart.Service';
import { CartItemModel } from '../../models/user/cart/user-cart.models';
import { RemoveCartItemModel } from '../../models/user/cart/remove-cart.model';
import { UpdateCartItemModel } from '../../models/user/cart/update-cart.model';
import { Router } from '@angular/router';
import { UserProductService } from '../../services/user-product.Service';

@Component({
  selector: 'app-user-cart',
  imports: [CommonModule],
  templateUrl: './user-cart.html',
  styleUrl: './user-cart.css'
})
export class UserCart {
  cartItems = signal<CartItemModel[]>([]);
  isLoading = signal(false);
  error = signal<string | null>(null);

  totalAmount = computed(() =>
    this.cartItems().reduce((sum, item) => sum + item.price * item.qunatity, 0)
  );

  totalItems = computed(() =>
    this.cartItems().reduce((sum, item) => sum + item.qunatity, 0)
  );

  constructor(private userCartService: UserCartService,private router : Router,private userProductService: UserProductService) { }

  ngOnInit() {
    this.loadCart();
  }

  loadCart() {
    this.isLoading.set(true);
    this.error.set(null);
    this.userCartService.getCartItems().subscribe({
      next: (response) => {
        console.log(response);
        this.cartItems.set(response);
        this.isLoading.set(false);
      },
      error: (err) => {
        console.error(err);
        this.cartItems.set([]);
        this.isLoading.set(false);
      }
    });
  }

  removeItem(cartItemsId: number) {
    const request = new RemoveCartItemModel(cartItemsId);

    this.userCartService.removeFromCart(request).subscribe({
      next: () => {
        this.cartItems.update(items =>
          items.filter(i => i.cartItemsId !== cartItemsId)
        );
      },
      error: (err) => console.error(err)
    });
  }

  removeAllItem() {
    this.userCartService.removeAllFromCart().subscribe({
      next: () => {
        this.loadCart();
      },
      error: (err) => console.error(err)
    });
  }

  updateQuantity(cartItemsId: number, quantity: number) {
    if (quantity < 1) return;

    const request = new UpdateCartItemModel(cartItemsId, quantity);

    this.userCartService.updateQuantity(request).subscribe({
      next: () => {
        this.cartItems.update(items =>
          items.map(i =>
            i.cartItemsId === cartItemsId
              ? { ...i, qunatity: quantity }
              : i
          )
        );
      },
      error: (err) => console.error(err)
    });
  }
  goToCheckout(): void {
    this.router.navigate(['/user/checkout']);
  }
   goToProductDetails(productId : any){
    this.router.navigate(['/user/product-details',productId]);
  }
}