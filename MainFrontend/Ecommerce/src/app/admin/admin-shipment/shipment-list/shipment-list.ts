import { Component, computed, signal } from '@angular/core';
import { ShipmentModel } from '../../../models/admin/admin-shipment/admin-shipment.model';
import { PagedResponse } from '../../../models/paged-response.model';
import { AdminShipmentService } from '../../../services/admin-shipment.Service';
import { Router } from '@angular/router';
import { ShipmentFilter } from '../../../models/admin/admin-shipment/shipment.filter';
import { DatePipe } from '@angular/common';

@Component({
  selector: 'app-shipment-list',
  imports: [DatePipe],
  templateUrl: './shipment-list.html',
  styleUrl: './shipment-list.css',
})
export class ShipmentList {
  shipments = signal<PagedResponse<ShipmentModel> | null>(null);


  shipmentTypeId = signal<number | null>(null);
  shipmentStatusId = signal<number | null>(null);
  orderId = signal<number | null>(null);

  courierName = signal<string>('');
  pickUpAddressId = signal<number | null>(null);

  trackingNumber = signal<string>('');

  fromDate = signal<string>('');
  toDate = signal<string>('');

  pageNumber = signal<number>(1);
  pageSize = signal<number>(10);

  totalPages = computed(() => this.shipments()?.totalPages ?? 1);

  filterPanelOpen = signal<boolean>(false);

  constructor(
    private router: Router,
    private shipmentService: AdminShipmentService
  ) {}

  ngOnInit(): void {
    this.loadShipments();
  }


  loadShipments(): void {
    this.shipmentService.getShipment(this.buildFilter()).subscribe({
      next: (res: PagedResponse<ShipmentModel>) => {
        this.shipments.set(res);
        console.log(res);
      },
      error: (err) => {
        console.error(err);

        this.shipments.set({
          items: [],
          totalCount: 0,
          pageNumber: this.pageNumber(),
          pageSize: this.pageSize(),
          totalPages: 1
        });
      }
    });
  }

  private buildFilter(): ShipmentFilter {
    return {
      shipmentTypeId: this.shipmentTypeId() ?? undefined,
      shipmentStatusId: this.shipmentStatusId() ?? undefined,
      orderId: this.orderId() ?? undefined,

      courierName: this.courierName() || undefined,
      pickUpAddressId: this.pickUpAddressId() ?? undefined,

      trackingNumber: this.trackingNumber() || undefined,

      fromDate: this.fromDate() || undefined,
      toDate: this.toDate() || undefined,

      pageNumber: this.pageNumber(),
      pageSize: this.pageSize()
    };
  }

  toggleFilterPanel(): void {
    this.filterPanelOpen.update(v => !v);
  }

  closeFilterPanel(): void {
    this.filterPanelOpen.set(false);
  }

  applyFilters(): void {
    this.pageNumber.set(1);
    this.loadShipments();
    this.closeFilterPanel();
  }

  resetFilters(): void {
    this.shipmentTypeId.set(null);
    this.shipmentStatusId.set(null);
    this.orderId.set(null);

    this.courierName.set('');
    this.pickUpAddressId.set(null);

    this.trackingNumber.set('');

    this.fromDate.set('');
    this.toDate.set('');

    this.pageNumber.set(1);
    this.pageSize.set(10);

    this.loadShipments();
    this.closeFilterPanel();
  }

  goToPage(page: number): void {
    if (page < 1 || page > this.totalPages()) return;

    this.pageNumber.set(page);
    this.loadShipments();
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
    this.loadShipments();
  }

  onShipmentTypeChange(event: Event): void {
    const v = (event.target as HTMLSelectElement).value;
    this.shipmentTypeId.set(v ? Number(v) : null);
  }

  onStatusChange(event: Event): void {
    const v = (event.target as HTMLSelectElement).value;
    this.shipmentStatusId.set(v ? Number(v) : null);
  }

  onOrderIdInput(event: Event): void {
    const v = (event.target as HTMLInputElement).value;
    this.orderId.set(v ? Number(v) : null);
  }

  onCourierNameInput(event: Event): void {
    this.courierName.set((event.target as HTMLInputElement).value);
  }

  onTrackingInput(event: Event): void {
    this.trackingNumber.set((event.target as HTMLInputElement).value);
  }

  onFromDateInput(event: Event): void {
    this.fromDate.set((event.target as HTMLInputElement).value);
  }

  onToDateInput(event: Event): void {
    this.toDate.set((event.target as HTMLInputElement).value);
  }

  onPickUpAddressChange(event: Event): void {
    const v = (event.target as HTMLSelectElement).value;
    this.pickUpAddressId.set(v ? Number(v) : null);
  }

  viewShipment(id: number): void {
    this.router.navigate(['/admin/shipment-details', id]);
  }
}
