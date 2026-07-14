import { Component, signal, computed, effect } from '@angular/core';
import { UserProductService } from '../../services/user-product.Service';
import { UserProductCategoryModel } from '../../models/user/product-category/user-product-category.model';
import { UserSubProductCategoryModel } from '../../models/user/product-category/user-sub-category.model';
import { Router, RouterLink, RouterOutlet } from '@angular/router';
import { AuthStateService } from '../../services/auth-State.Service';
import { NgClass, DatePipe } from '@angular/common';
import { NotificationService } from '../../services/notification.Service';
import { NotificationHubService } from '../../services/notificationHubService';
import { NotificationResponseModel } from '../../models/notification/notification.model';
import { NotificationFilterModel } from '../../models/notification/notification.filter';

@Component({
  selector: 'app-user-navbar',
  imports: [RouterOutlet, NgClass, RouterLink, DatePipe],
  templateUrl: './user-navbar.html',
  styleUrl: './user-navbar.css',
})
export class UserNavbar {
  mobileSearchOpen = signal(false);
  selectedProductCategory = signal<number | null>(null);
  productCategoryModel = signal<UserProductCategoryModel[]>([]);
  subProductCategoryModel = signal<UserSubProductCategoryModel[]>([]);

  notifications = signal<NotificationResponseModel[]>([]);
  unreadCount = computed(() => this.notifications().filter(n => !n.isRead).length);
  isNotificationDropdownOpen = signal(false);

  constructor(
      private userProductService: UserProductService, 
      private router: Router, 
      public authState: AuthStateService,
      private notificationService: NotificationService,
      private notificationHub: NotificationHubService
  ) {
      effect(() => {
          const incoming = this.notificationHub.latestNotification();
          if (incoming) {
              this.notifications.update(current => [incoming, ...current]);
          }
      }, { allowSignalWrites: true });
  }

  ngOnInit() {
    this.loadProductCategory();
    this.authState.validateSession();
    if (this.authState.isLoggedIn()) {
        this.loadNotifications();
    }
  }

  loadNotifications() {
      const filter = new NotificationFilterModel();
      filter.pageNumber = 1;
      filter.pageSize = 10;
      this.notificationService.getNotification(filter).subscribe({
          next: (res) => {
              this.notifications.set(res.items || []);
          }
      });
  }

  toggleNotificationDropdown() {
      this.isNotificationDropdownOpen.update(val => !val);
  }

  markAsRead(notification: NotificationResponseModel) {
      if (!notification.isRead) {
          this.notificationService.updateNotification(notification.notificationId).subscribe(() => {
              this.notifications.update(current => 
                  current.map(n => n.notificationId === notification.notificationId ? {...n, isRead: true} : n)
              );
          });
      }
  }

  deleteNotification(notification: NotificationResponseModel, event: Event) {
      event.stopPropagation();
      this.notificationService.deleteNotification(notification.notificationId).subscribe({
          next: () => {
              this.notifications.update(current => 
                  current.filter(n => n.notificationId !== notification.notificationId)
              );
          },
          error: (err) => console.error(err)
      });
  }

  clearAllNotifications() {
      if (this.notifications().length === 0) return;
      this.notificationService.clearAllNotifications().subscribe({
          next: () => {
              this.notifications.set([]);
          },
          error: (err) => console.error(err)
      });
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
    this.router.navigate(['/user/notifications']);
  }

  goToProducts(subCategoryId: number): void {
    this.router.navigate(['/user/subcategory', subCategoryId, 'products']);
  }
}
