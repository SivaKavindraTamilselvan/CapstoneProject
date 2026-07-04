import { Component, signal } from '@angular/core';
import { AddAddress } from '../../../address/add-address/add-address';

@Component({
  selector: 'app-add-warehouse',
  imports: [AddAddress],
  templateUrl: './add-warehouse.html',
  styleUrl: './add-warehouse.css',
})
export class AddWarehouse {}
