import { Component, signal } from '@angular/core';
import { AdminShipmentService } from '../../../services/admin-shipment.Service';
import { ActivatedRoute, Router } from '@angular/router';
import { ShipmentModel } from '../../../models/admin/admin-shipment/admin-shipment.model';
import { DatePipe, DecimalPipe, NgClass } from '@angular/common';
import { UpdateAdminShipment } from '../update-admin-shipment/update-admin-shipment';

@Component({
  selector: 'app-admin-shipment-details',
  imports: [DatePipe, DecimalPipe, NgClass,UpdateAdminShipment],
  templateUrl: './admin-shipment-details.html',
  styleUrl: './admin-shipment-details.css',
})
export class AdminShipmentDetails {
  shipmentModel = signal<ShipmentModel | null>(null);
  selectedId = signal<number>(0);

  constructor(private shipmentService: AdminShipmentService, private route: ActivatedRoute, private router: Router) {

  }
  ngOnInit(): void {
    const orderid = Number(this.route.snapshot.paramMap.get('id'));
    if (orderid) {
      this.loadShipmentDetails(orderid);
      this.selectedId.set(orderid);
    }
  }

  tableLoading = signal(false);
  loadShipmentDetails(productId: number) {
    this.tableLoading.set(true);
    this.shipmentService.getShipmentDetails(productId).subscribe({
      next: (response: any) => {
        //console.log(response);
        this.shipmentModel.set(response);
        this.tableLoading.set(false);
      },
      error: (error) => {
        //console.error(error);
        this.tableLoading.set(false);
      }
    })
  }
  goBack() {
    this.router.navigate(['/admin/shipments/list']);
  }

  viewOrder(id: number) {
    this.router.navigate(['/admin/order', id]);
  }
  viewVariant(id: number) {
    this.router.navigate(['/admin/product-variant-details', id]);
  }


  showUpdateShipmentPopup = signal(false);
  selectedShipmentIdForUpdate = signal<number | null>(null);

  openUpdatePopup(shipmentId: number) {
    this.selectedShipmentIdForUpdate.set(shipmentId);
    this.showUpdateShipmentPopup.set(true);
  }

  closeUpdateShipmentPopup() {
    this.showUpdateShipmentPopup.set(false);
    this.selectedShipmentIdForUpdate.set(null);
  }

  onShipmentUpdated() {
    this.loadShipmentDetails(this.selectedId());
  }

}
