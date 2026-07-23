import { Component, signal } from '@angular/core';
import { ProfilePage } from '../../profile-page/profile-page';
import { AddressService } from '../../services/address.Service';
import { AddressModel } from '../../models/address/address-response.model';
import { Router } from '@angular/router';
import { UserOrderService } from '../../services/user-order.Service';

@Component({
  selector: 'app-user-profile-page',
  imports: [ProfilePage],
  templateUrl: './user-profile-page.html',
  styleUrl: './user-profile-page.css',
})
export class UserProfilePage {
  address = signal<AddressModel[] | null>(null);
  constructor(private addressService: AddressService, private router: Router,private orderService : UserOrderService) {

  }
  ngOnInit() {
    this.loadAddress();
    this.loadWalletBalance();
  }
  isLoading = signal(false);
  loadAddress() {
    this.isLoading.set(true);
    this.addressService.getUserAddress().subscribe({
      next: (response: any) => {
        this.address.set(response);
        this.isLoading.set(false);
        //console.log(response);
      },
      error: (error) => {
        //console.error(error);
        this.isLoading.set(false);
      }
    })
  }

  goToAddAddress() {
    this.router.navigate(['/user/add-address']);
  }

  walletBalance = signal<number>(0);

  loadWalletBalance(): void {
    this.orderService.getWalletBalane().subscribe({
      next: (balance: any) => this.walletBalance.set(balance),
      error: () => this.walletBalance.set(0),
    });
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
        //alert("Warehouse deleted");
        this.loadAddress();
      }
    })
  }
}
