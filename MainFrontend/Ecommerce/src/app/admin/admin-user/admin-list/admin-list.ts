import { Component, signal, computed } from '@angular/core';
import { Route, Router } from '@angular/router';
import { PagedResponse } from '../../../models/paged-response.model';
import { AdminUserService } from '../../../services/admin-user.Service';
import { AdminUserModel } from '../../../models/admin/admin-user/admin-user.model';
import { AdminUserFilter } from '../../../models/admin/admin-user/admin-user.filter';
import { email, form, pattern } from '@angular/forms/signals';

@Component({
  selector: 'app-admin-list',
  imports: [],
  templateUrl: './admin-list.html',
  styleUrl: './admin-list.css',
})
export class AdminList {
  constructor(private route: Router, private adminUserService: AdminUserService) {

  }

  ngOnInit(): void {
    this.loadAdminUser();
  }

  adminUsers = signal<PagedResponse<AdminUserModel> | null>(null);

  totalPages = computed(() => this.adminUsers()?.totalPages ?? 1);

  filterPanelOpen = signal<boolean>(false);
  adminUserFilter = signal(new AdminUserFilter());

  errorMessage = signal<string | null>(null);
  successMessage = signal<string | null>(null);

  filtererrorMessage = signal<string | null>(null);
  filterapplied = signal(false);

  filterForm = form(this.adminUserFilter, (path) => {
    email(path.email, { message: 'Enter a valid email address.' });
    pattern(path.phoneNumber, /^[1-9]{1}[0-9]{9}$/, { message: 'Enter a valid phone number.' });
  });

  adminRoleOption = [
    { id: 1, label: 'Overall Admin' },
    { id: 2, label: 'Vendor Admin' },
    { id: 3, label: 'Product Admin' },
    { id: 4, label: 'Order Admin' },
    { id: 5, label: 'Coupons Logistic Admin' },
    { id: 6, label: 'Return Admin' },
    { id: 7, label: 'Refund Admin' },
    { id: 8, label: 'Exchange Admin' },
    { id: 9, label: 'Payment Admin' }
  ]


  loadAdminUser() {
    this.adminUserService.getAdminUser(this.adminUserFilter()).subscribe({
      next: (response: any) => {
        this.adminUsers.set(response);
      },
      error: (error) => {
        if (error.status == 404) {
          this.adminUsers.set({
            items: [],
            totalCount: 0,
            pageNumber: 1,
            pageSize: 10,
            totalPages: 1
          });
        }
      }
    })
  }

  goToPage(page: number): void {
    if (page < 1 || page > this.totalPages()) {
      return;
    }
    this.adminUserFilter.update(filter => ({ ...filter, pageNumber: page }));
    this.loadAdminUser();
  }

  nextPage(): void {
    this.goToPage(this.adminUserFilter().pageNumber + 1);
  }
  
  previousPage(): void {
    this.goToPage(this.adminUserFilter().pageNumber - 1);
  }

  onPageSizeChange(event: Event): void {
    const value = Number((event.target as HTMLInputElement).value);
    this.adminUserFilter.update(filter => ({ ...filter, pageNumber: 1, pageSize: value }));
    this.loadAdminUser();
  }

  toggleFilterPanel(): void {
    this.filterPanelOpen.update((open) => !open);
  }

  closeFilterPanel(): void {
     if (this.filtererrorMessage()) {
      return;
    }
    this.filterapplied.set(true);
    this.filterPanelOpen.set(false);

  }

  onAdminRoleChange(event: Event): void {
    const v = (event.target as HTMLSelectElement).value;
    this.adminUserFilter.update(filter => ({ ...filter, adminRoleId: v ? Number(v) : null }));
  }

  onStatusChange(event: Event): void {
    const value = (event.target as HTMLSelectElement).value;
    this.adminUserFilter.update(filter => ({ ...filter, status: value === '' ? null : value === 'true' }));
  }

  onPhoneNumberChange(event: Event): void {
    const value = (event.target as HTMLInputElement).value.trim();
    this.adminUserFilter.update(filter => ({ ...filter, phoneNumber: value }));
  }

  onEmailChange(event: Event): void {
    const value = (event.target as HTMLInputElement).value.trim().toLowerCase();
    this.adminUserFilter.update(filter => ({ ...filter, email: value }));
  }

  applyFilter(): void {
    if (this.filterForm().invalid()) {
      return;
    }
    this.adminUserFilter.update(filter => ({ ...filter, pageNumber: 1 }));
    this.loadAdminUser();
    this.closeFilterPanel();
  }
  resetFilter(): void {
    this.adminUserFilter.set(new AdminUserFilter());
    this.adminUserFilter.update(filter => ({ ...filter, pageNumber: 1 }));
    this.loadAdminUser();
    this.filterapplied.set(false);
  }
  viewAdminUser(productId: number) {
    this.route.navigate(['/admin/users', productId]);
  }
}
