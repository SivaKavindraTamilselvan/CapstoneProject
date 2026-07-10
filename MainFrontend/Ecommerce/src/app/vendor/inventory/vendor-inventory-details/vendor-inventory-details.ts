import { Component, signal } from '@angular/core';
import { VendorInventoryService } from '../../../services/vendor-inventory.Service';
import { ActivatedRoute, Router } from '@angular/router';
import { VendorInventoryModel } from '../../../models/inventory/inventory.model';
import { NgClass } from '@angular/common';

@Component({
  selector: 'app-vendor-inventory-details',
  imports: [NgClass],
  templateUrl: './vendor-inventory-details.html',
  styleUrl: './vendor-inventory-details.css',
})
export class VendorInventoryDetails {
  inventory = signal<VendorInventoryModel |null >(null);
  constructor(private route : ActivatedRoute,private vendorInventoryService : VendorInventoryService){

  }
  ngOnInit(): void {
    const inventoryId = Number(this.route.snapshot.paramMap.get('id'));

    if (inventoryId) {
      this.loadInventory(inventoryId);
    }
  }
  loadInventory(inventoryId : number){
    this.vendorInventoryService.getInventoryDetails(inventoryId).subscribe({
      next : (response:any)=>{
        this.inventory.set(response);
      },
      error:(error)=>{
        console.error(error);
      }
    })
  }
}
