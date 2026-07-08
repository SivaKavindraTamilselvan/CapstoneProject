import { Component, computed, signal } from '@angular/core';
import { PagedResponse } from '../../../models/paged-response.model';
import { ShipmentModel } from '../../../models/admin/admin-shipment/admin-shipment.model';
import { Router } from '@angular/router';
import { AdminShipmentService } from '../../../services/admin-shipment.Service';
import { ShipmentFilter } from '../../../models/admin/admin-shipment/shipment.filter';
import { DatePipe } from '@angular/common';
import { ShipmentUpdateModel } from '../../../models/admin/admin-shipment/update-shipment.model';
import { form, FormField, required } from '@angular/forms/signals';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

@Component({
  selector: 'app-update-shipment',
  imports: [DatePipe, FormField, ReactiveFormsModule, FormsModule],
  templateUrl: './update-shipment.html',
  styleUrl: './update-shipment.css',
})
export class UpdateShipment {
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

  showActivatePopup = signal(false);
  successMessage = signal<string | null>(null);
  errorMessage = signal<string | null>(null);
  updateShipmentModel = signal(new ShipmentUpdateModel());
  selectedShipmentId = signal<number | null>(null);

  constructor(
    private router: Router,
    private shipmentService: AdminShipmentService
  ) { }

  ngOnInit(): void {
    this.loadShipments();
  }


  loadShipments(): void {
    this.shipmentService.getShipment(new ShipmentFilter()).subscribe({
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

  updateForm = form(this.updateShipmentModel, (path) => {
    required(path.remarks, { message: "Enter The Remarks" });
    required(path.location, { message: "Enter The Location" });
    required(path.shipmentStatusId, { message: "Enter The Shipment Status" });
  })

  updateShipment() {
    if (this.updateForm().invalid()) return;

    this.errorMessage.set(null);
    this.successMessage.set(null);

    if (this.updateForm().invalid()) {
      this.errorMessage.set("Enter proper details");
      return;
    }

    const request = {
      shipmentStatusId:  Number(this.updateShipmentModel().shipmentStatusId),
      remarks: this.updateShipmentModel().remarks,
      shipmentId: this.updateShipmentModel().shipmentId,
      location: this.updateShipmentModel().location,
    };

    this.shipmentService.updateShipment(request).subscribe({
      next: () => {
        this.successMessage.set("Shipment updated successfully. Closing in 3 seconds...");
        setTimeout(() => {
          this.closePopup();
          this.successMessage.set(null);
          this.loadShipments();
        }, 3000);
      },
      error: (error) => {
        this.successMessage.set(null);

        if (error.status === 400 && error.error?.errors) {
          const messages = Object.values(error.error.errors)
            .flat()
            .join(", ");

          this.errorMessage.set(messages);
        }
        else {
          this.errorMessage.set(
            error.error?.message ?? "Something went wrong. Please try again."
          );
        }
      }
    });
  }

  openDeletePopup(shipmentId: number) {
    this.selectedShipmentId.set(shipmentId);

    this.updateShipmentModel.update(model => ({
      ...model,
      shipmentId: shipmentId,
      remark: '',
      location: '',
      shipmentStatusId : ''
    }));

    this.showActivatePopup.set(true);
  }

  closePopup() {
    this.showActivatePopup.set(false);
    this.selectedShipmentId.set(null);
    this.updateShipmentModel.set(new ShipmentUpdateModel());
    this.errorMessage.set(null);
  }
}

