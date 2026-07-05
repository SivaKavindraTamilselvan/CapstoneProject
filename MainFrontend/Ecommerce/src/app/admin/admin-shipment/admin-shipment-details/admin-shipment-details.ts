import { Component, signal } from '@angular/core';
import { AdminShipmentService } from '../../../services/admin-shipment.Service';
import { ActivatedRoute } from '@angular/router';
import { ShipmentModel } from '../../../models/admin/admin-shipment/admin-shipment.model';
import { DatePipe, DecimalPipe, NgClass } from '@angular/common';

@Component({
  selector: 'app-admin-shipment-details',
  imports: [DatePipe,DecimalPipe,NgClass],
  templateUrl: './admin-shipment-details.html',
  styleUrl: './admin-shipment-details.css',
})
export class AdminShipmentDetails {
  shipmentModel = signal<ShipmentModel| null>(null);


  constructor(private shipmentService: AdminShipmentService, private route: ActivatedRoute) {

  }
  ngOnInit(): void {
    const orderid = Number(this.route.snapshot.paramMap.get('id'));
    if (orderid) {
      this.loadShipmentDetails(orderid);
    }
  }
  loadShipmentDetails(productId: number) {
    this.shipmentService.getShipmentDetails(productId).subscribe({
      next: (response: any) => {
        console.log(response);
        this.shipmentModel.set(response);
      },
      error: (error) => {
        console.error(error);
      }
    })
  }
}
