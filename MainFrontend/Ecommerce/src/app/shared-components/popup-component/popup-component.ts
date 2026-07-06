import { NgClass } from '@angular/common';
import { Component, input, output } from '@angular/core';

@Component({
  selector: 'app-popup-component',
  imports: [NgClass],
  templateUrl: './popup-component.html',
  styleUrl: './popup-component.css',
})
export class PopupComponent {
  show = input(false);
  title = input('Confirmation');
  titleClass = input('text-red-700');
  message = input('Are you sure you want to continue?');
  confirmText = input('Confirm');
  cancelText = input('Cancel');
  confirmButtonClass = input('bg-red-700 hover:bg-red-900');
  confirm = output<void>();
  cancel = output<void>();
  onConfirm() {
    this.confirm.emit();
  }
  onCancel() {
    this.cancel.emit();
  }
}
