import { Component, input, output, signal } from '@angular/core';
import { Router } from '@angular/router';
import { AdminProductCategoryService } from '../../../services/admin-category.Service';
import { PopupBase } from '../../../shared-class/popup-base-class';
import { AddProductCategoryModel } from '../../../models/admin/admin-product-category/add-models/add-category.model';
import { form, FormField, required } from '@angular/forms/signals';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

@Component({
  selector: 'app-create-category-component',
  imports: [FormField, ReactiveFormsModule, FormsModule],
  templateUrl: './create-category-component.html',
  styleUrl: './create-category-component.css',
})
export class CreateCategoryComponent extends PopupBase {
  constructor(private route: Router, private adminCategoryService: AdminProductCategoryService) {
    super();
  }
  show = input<boolean>(false);
  close = output<void>();
  added = output<void>();

  loading = signal(false);
  successMessage = signal('');
  errorMessage = signal('');

  addCategoryModel = signal(new AddProductCategoryModel());

  addForm = form(this.addCategoryModel, (path) => {
    required(path.productCategoryName, { message: "Enter The Product Category Name" });
  });

  addCategory() {
    this.errorMessage.set('');
    this.successMessage.set('');
    if (this.addForm().invalid()) {
      this.errorMessage.set("Enter Category Name");
      return;
    }
    this.loading.set(true);
    this.adminCategoryService.addCategory(this.addCategoryModel()).subscribe({
      next: (response: any) => {
        this.successMessage.set("Category added successfully");
        setTimeout(() => {
          this.added.emit();
          this.close.emit();
          this.loading.set(false);
        }, 3000);
      },
      error: (error) => {
        console.error(error);
        this.errorMessage.set(error?.error?.message ?? "Failed to add category");
        this.loading.set(false);
      }
    })
  }
  cancel() {
    this.errorMessage.set('');
    this.successMessage.set('');
    this.addCategoryModel.set(new AddProductCategoryModel());
    this.addForm().reset();
    this.close.emit();
  }


}
