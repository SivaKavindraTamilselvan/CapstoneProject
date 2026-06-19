import { Component, signal } from '@angular/core';
import { Route, Router } from '@angular/router';
import { PagedResponse } from '../../models/paged-response.model';
import { AdminUserService } from '../../services/admin-user.Service';
import { AdminUserModel } from '../../models/admin-user.model';

@Component({
  selector: 'app-admin-list',
  imports: [],
  templateUrl: './admin-list.html',
  styleUrl: './admin-list.css',
})
export class AdminList {
  adminUsers = signal<PagedResponse<AdminUserModel> | null>(null);
  constructor(private route : Router,private adminUserService : AdminUserService){

  }
  ngOnInit():void{
    this.loadAdminUser();
  }
  loadAdminUser(){
    this.adminUserService.getAdminUser().subscribe({
      next: (response:any)=>{
        this.adminUsers.set(response);
      },
      error : (error) =>{
        console.log(error);
      }
    })
  }
}
