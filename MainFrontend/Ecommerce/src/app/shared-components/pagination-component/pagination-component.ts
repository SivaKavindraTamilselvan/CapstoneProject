import { Component, EventEmitter, Input, Output } from '@angular/core';

@Component({
  selector: 'app-pagination-component',
  templateUrl: './pagination-component.html',
  styleUrl: './pagination-component.css'
})
export class PaginationComponent {

  @Input() pageNumber = 1;

  @Input() totalPages = 1;

  @Input() totalCount = 0;

  @Input() pageSize = 10;

  @Output() previous = new EventEmitter<void>();

  @Output() next = new EventEmitter<void>();

  @Output() pageSizeChange = new EventEmitter<number>();

  onPageSizeChange(event: Event) {
    this.pageSizeChange.emit(
      Number((event.target as HTMLSelectElement).value)
    );
  }

}