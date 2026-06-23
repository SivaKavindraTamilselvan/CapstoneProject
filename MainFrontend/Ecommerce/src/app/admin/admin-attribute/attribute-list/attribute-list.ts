import { Component, computed, signal } from '@angular/core';
import { Router } from '@angular/router';
import { AdminProductCategoryService } from '../../../services/admin-category.Service';
import { AdminAttributeModel } from '../../../models/admin/admin-product-category/response/admin-attribute.model';
import { AttributeFilter } from '../../../models/admin/admin-product-category/filter-models/attribute.filter';
import { PagedResponse } from '../../../models/paged-response.model';
import { AddAttributeModel } from '../../../models/admin/admin-product-category/add-models/add-attribute.model';
import { DatePipe } from '@angular/common';
import { FormField,form, required } from '@angular/forms/signals';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

@Component({
  selector: 'app-attribute-list',
  imports: [DatePipe,FormField,ReactiveFormsModule,FormsModule],
  templateUrl: './attribute-list.html',
  styleUrl: './attribute-list.css',
})
export class AttributeList { 
  attribute = signal<PagedResponse<AdminAttributeModel> | null>(null);
  attributeName = signal<string>('');
  status = signal<boolean | null>(null);
  addedByAdminId = signal<number | null>(null);

  pageNumber = signal<number>(1);
  pageSize = signal<number>(10);
  totalPages = computed(() => this.attribute()?.totalPages ?? 1);
  filterPanelOpen = signal<boolean>(false);

  showActivatePopup = signal(false);
  addAttributeModel = signal(new AddAttributeModel());

  constructor(private router:Router,private adminCategoryService : AdminProductCategoryService){

  }
  ngOnInit(){
    this.loadAttribute();
  }
  loadAttribute(){
    this.adminCategoryService.getAttribute(this.buildFilter()).subscribe({
      next : (response:any)=>{
        this.attribute.set(response);
        console.log(response);
      },
      error : (error)=>{
        console.error(error);
        if (error.status == 404) {
          this.attribute.set({
            items: [],
            totalCount: 0,
            pageNumber: this.pageNumber(),
            pageSize: this.pageSize(),
            totalPages: 1
          });
        }
      }
    })
  }
  private buildFilter() : AttributeFilter{
    return{
      attributeName : this.attributeName(),
      status : this.status(),
      addedByAdminId : this.addedByAdminId()
    }
  }
  toggleFilterPanel(): void {
    this.filterPanelOpen.update((open) => !open);
  }
  closeFilterPanel(): void {
    this.filterPanelOpen.set(false);
  }
  applyFilters(): void {
    this.pageNumber.set(1);
    this.loadAttribute();
    this.closeFilterPanel();
  }
  resetFilters(): void {
    this.pageNumber.set(1);
    this.addedByAdminId.set(null);
    this.status.set(null);
    this.attributeName.set('');
    this.loadAttribute();
    this.closeFilterPanel();
  }
  goToPage(pageNumber: number): void {
    if (pageNumber < 1 || pageNumber > this.totalPages()) {
      return;
    }
    this.pageNumber.set(pageNumber);
    this.loadAttribute();
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
    this.loadAttribute();
  }
  onAdminIdInput(event: Event): void {
    const v = (event.target as HTMLInputElement).value;
    this.addedByAdminId.set(v ? Number(v) : null);
  }
  onAttributeNameInput(event: Event): void {
    const v = (event.target as HTMLInputElement).value;
    this.attributeName.set(v);
  }
  onStatusChange(event: Event): void {
    const value = (event.target as HTMLSelectElement).value;
    if (value === '') {
      this.status.set(null);
    }
    else {
      this.status.set(value === 'true');
    }
  }
  openPopup():void{
    this.showActivatePopup.set(true);
  }
  closePopup():void{
    this.showActivatePopup.set(false);
  }
  addForm = form(this.addAttributeModel,(path)=>{
    required(path.attributeName, {message: "Enter The Attribute Name"});
  });
  addAttribute(){
    if(this.addForm().invalid()){
      alert("Category Is Invalid");
      return;
    }
    this.adminCategoryService.addAttribute(this.addAttributeModel()).subscribe({
      next :(response:any)=>{
        alert("Category added successfully");
        console.log(response);
        this.loadAttribute();
        this.closePopup();
      },
      error : (error)=>{
        console.error(error);
      }
    })
  }
}
