import { Component, signal } from '@angular/core';
import { ProfilePage } from '../../profile-page/profile-page';
import { AddressService } from '../../services/address.Service';
import { AddressModel } from '../../models/address/address-response.model';
import { Router } from '@angular/router';

@Component({
  selector: 'app-user-profile-page',
  imports: [ProfilePage],
  templateUrl: './user-profile-page.html',
  styleUrl: './user-profile-page.css',
})
export class UserProfilePage {
  address = signal<AddressModel[] | null>(null);
  constructor(private addressService: AddressService,private router :Router) {

  }
  ngOnInit() {
    this.loadAddress();
  }
  loadAddress() {
    this.addressService.getUserAddress().subscribe({
      next: (response: any) => {
        this.address.set(response);
        console.log(response);
      },
      error: (error) => {
        console.error(error);
      }
    })
  }

  goToAddAddress() {
    this.router.navigate(['/user/add-address']);
  }


  makeDefaultAddress(addressId: number) {
    this.addressService.setDefaultAddress(addressId).subscribe({
      next: () => {
        this.loadAddress();
      },
      error: (error) => {
        console.error(error);
      },
    });
  }

  deleteAddress(addressId: number) {
    this.addressService.deleteUserAddress(addressId).subscribe({
      next: (response: any) => {
        alert("Warehouse deleted");
        this.loadAddress();
      }
    })
  }
}
