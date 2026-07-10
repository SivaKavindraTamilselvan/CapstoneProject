import { Component, EventEmitter, input, Input, Output } from '@angular/core';

@Component({
  selector: 'app-filter-component',
  imports: [],
  templateUrl: './filter-component.html',
  styleUrl: './filter-component.css',
})
export class FilterComponent {

  errorMessage = input<string | null>(null);
  
  @Input() open = false;

  @Input() title = 'Filters';

  @Output() close = new EventEmitter<void>();

  @Output() toggle = new EventEmitter<void>();

  @Output() apply = new EventEmitter<void>();

  @Output() reset = new EventEmitter<void>();
  onClose() {
    this.close.emit();
  }

  onApply() {
    this.apply.emit();
  }

  onReset() {
    this.reset.emit();
  }
}
