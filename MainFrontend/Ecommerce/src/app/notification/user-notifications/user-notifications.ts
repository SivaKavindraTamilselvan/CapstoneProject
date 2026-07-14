import { Component, computed, effect, inject, signal } from '@angular/core';
import { NotificationHubService } from '../../services/notificationHubService';
import { PagedResponse } from '../../models/paged-response.model';
import { NotificationResponseModel } from '../../models/notification/notification.model';
import { Router } from '@angular/router';
import { NotificationService } from '../../services/notification.Service';
import { NotificationFilterModel } from '../../models/notification/notification.filter';
import { NgClass } from '@angular/common';

@Component({
  selector: 'app-user-notifications',
  imports: [NgClass],
  templateUrl: './user-notifications.html',
  styleUrl: './user-notifications.css',
})
export class UserNotifications {
  private hub = inject(NotificationHubService);
  notifications = signal<PagedResponse<NotificationResponseModel> | null>(null);

  notificationTypeId = signal<number | undefined>(undefined);
  isRead = signal<boolean | undefined>(undefined);
  minCreatedAt = signal<Date | undefined>(undefined);
  maxCreatedAt = signal<Date | undefined>(undefined);
  pageNumber = signal<number>(1);
  pageSize = signal<number>(10);

  filterPanelOpen = signal<boolean>(false);
  successMessage = signal<string | null>(null);
  errorMessage = signal<string | null>(null);
  selectedNotification = signal<number | null>(null);

  filtererrorMessage = signal<string | null>(null);
  progress = signal(false);
  filterapplied = signal(false);

  toggleFilterPanel(): void {
    const wasOpen = this.filterPanelOpen();
    this.filterPanelOpen.update((open) => !open);
    if (wasOpen && !this.filterapplied()) {
      this.resetFilter();
    }
  }

  closeFilterPanel(): void {
    this.filterPanelOpen.set(false);
  }

  totalPages = computed(() => this.notifications()?.totalPages ?? 1);

  constructor(private route: Router, private notificationService: NotificationService) {
    
    effect(() => {
        console.log('Hub status:', this.hub.connectionStatus());

      const incoming = this.hub.latestNotification();
      if (!incoming) return;

      this.notifications.update(current => {
        if (!current) return current;
        return {
          ...current,
          items: [incoming, ...current.items],
          totalCount: current.totalCount + 1,
        };
      });
    });
   }

  ngOnInit(): void {
    this.loadNotification();
  }

  private buildFilter(): NotificationFilterModel {
    return {
      maxCreatedAt: this.maxCreatedAt(),
      minCreatedAt: this.minCreatedAt(),
      isRead: this.isRead(),
      notificationTypeId: this.notificationTypeId(),
      pageNumber: this.pageNumber(),
      pageSize: this.pageSize(),
    };
  }

  loadNotification(): void {
    this.notificationService.getNotification(this.buildFilter()).subscribe({
      next: (response: any) => {
        this.notifications.set(response);
      },
      error: (error) => {
        console.log(error);
        if (error.status === 404) {
          this.notifications.set({
            items: [],
            pageNumber: 1,
            pageSize: 10,
            totalCount: 0,
            totalPages: 0
          });
        }
        else if (error.status === 400 && error.error?.errors) {
          const messages = Object.values(error.error.errors)
            .flat()
            .join(", ");

          this.errorMessage.set(messages);
        }
        else {
          this.errorMessage.set(error.errorMessage);
        }
      },
    });
  }

  updateNotification(id: number): void {
    this.notificationService.updateNotification(id).subscribe({
      next: (updatedNotification) => {

        this.notifications.update(current => {
          if (!current) return current;

          return {
            ...current,
            items: current.items.map(item =>
              item.notificationId === updatedNotification.notificationId
                ? {
                  ...item,
                  isRead: !item.isRead,
                  readAt: !item.isRead ? new Date().toISOString() : null
                }
              : item
            )
          };
        });

      }
    });
  }
  applyFilter(): void {
    if (this.filtererrorMessage()) {
      return;
    }
    this.filterapplied.set(true);
    this.pageNumber.set(1);
    this.loadNotification();
    this.closeFilterPanel();
  }

  resetFilter(): void {
    this.filtererrorMessage.set("");
    this.isRead.set(undefined);
    this.notificationTypeId.set(undefined);
    this.maxCreatedAt.set(undefined);
    this.minCreatedAt.set(undefined);
    this.pageNumber.set(1);
    this.loadNotification();
  }

  goToPage(page: number): void {
    if (page < 1 || page > this.totalPages()) return;
    this.pageNumber.set(page);
    this.loadNotification();
  }

  nextPage(): void {
    this.goToPage(this.pageNumber() + 1);
  }

  previousPage(): void {
    this.goToPage(this.pageNumber() - 1);
  }

  onPageSizeChange(event: Event): void {
    const value = Number((event.target as HTMLSelectElement).value);
    this.pageSize.set(value);
    this.pageNumber.set(1);
    this.loadNotification();
  }

  onNotificationTypeChange(event: Event): void {
    const value = (event.target as HTMLSelectElement).value;
    this.notificationTypeId.set(value ? Number(value) : undefined);
  }

  onIsReadChange(event: Event): void {
    const value = (event.target as HTMLSelectElement).value;

    if (value === "") {
      this.isRead.set(undefined);
    } else {
      this.isRead.set(value === "true");
    }
  }

  onMinCreatedAtChange(event: Event): void {
    const value = (event.target as HTMLInputElement).value;
    this.minCreatedAt.set(value ? new Date(value) : undefined);
  }

  onMaxCreatedAtChange(event: Event): void {
    const value = (event.target as HTMLInputElement).value;
    this.maxCreatedAt.set(value ? new Date(value) : undefined);
  }

  formatNotificationDate(createdAt: string | Date): string {
  const date = new Date(createdAt);
  const today = new Date();

  const isSameDate =
    date.getFullYear() === today.getFullYear() &&
    date.getMonth() === today.getMonth() &&
    date.getDate() === today.getDate();

  if (isSameDate) {
    return date.toLocaleTimeString('en-US', {
      hour: '2-digit',
      minute: '2-digit'
    });
  } else {
    return date.toLocaleDateString('en-GB', {
      day: '2-digit',
      month: 'short',
      year: 'numeric'
    });
  }
}
}
