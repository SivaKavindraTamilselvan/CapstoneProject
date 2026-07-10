import { Component, EventEmitter, input, Input, Output } from '@angular/core';

@Component({
  selector: 'app-header-component',
  imports: [],
  templateUrl: './header-component.html',
  styleUrl: './header-component.css',
})
export class HeaderComponent {
  filterApplied = input(false);
title = input<string | null>('Header');
  @Output() toggle = new EventEmitter<void>();
  @Output() reset = new EventEmitter<void>();

  onToggle() {
    this.toggle.emit();
  }

  onReset() {
    this.reset.emit();
  }
}
