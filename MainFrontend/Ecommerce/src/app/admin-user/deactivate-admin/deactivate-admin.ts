import { Component, signal } from '@angular/core';
import { AdminUserService } from '../../services/admin-user.Service';
import { Router } from '@angular/router';
import { PagedResponse } from '../../models/paged-response.model';
import { AdminUserModel } from '../../models/admin-user.model';

@Component({
  selector: 'app-deactivate-admin',
  imports: [],
  templateUrl: './deactivate-admin.html',
  styleUrl: './deactivate-admin.css',
})
export class DeactivateAdmin {
  adminUsers = signal<PagedResponse<AdminUserModel> | null>(null);
  showActivatePopup = signal(false);
  selectedAdminId = signal<number | null>(null);
  constructor(private route : Router,private adimUserService : AdminUserService)
  {

  }
  ngOnInit():void{
    this.loadDeactiveAdminUser();
  }
  loadDeactiveAdminUser(){
    this.adimUserService.getDeactiveAdminUser().subscribe({
      next : (response:any)=>{
        this.adminUsers.set(response);
      },
      error : (error)=>{
        console.log(error);
      }
    })
  }
  activateAdmin()  {
    const id = this.selectedAdminId();
    if(id==null)
    {
      return;
    }
    this.adimUserService.activateAdminUser(id).subscribe({
      next : (response:any)=>{
        this.closePopup();
        this.loadDeactiveAdminUser();
      },
      error : (error)=>{
        console.log(error);
      }
    })
  }
  confirmActivate(id :number){
    this.selectedAdminId.set(id);
    this.showActivatePopup.set(true);
  }
  closePopup(){
    this.showActivatePopup.set(false);
    this.selectedAdminId.set(null);
  }
}
