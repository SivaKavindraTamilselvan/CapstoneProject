import { Component, EventEmitter, Input, Output } from '@angular/core';
import { Column } from '../data-table-component/column.model';
import { TableAction } from '../data-table-component/table-actions.model';
import { NgClass } from '@angular/common';

@Component({
  selector: 'app-mobile-card-component',
  imports: [NgClass],
  templateUrl: './mobile-card-component.html',
  styleUrl: './mobile-card-component.css',
})
export class MobileCardComponent {
  @Input() columns: Column[] = [];
  @Input() rows: any[] = [];
  @Input() actions: TableAction[] = [];
  @Input() titleKey = '';
  @Output() action = new EventEmitter<{ type: string; row: any; }>();

  getCellValue(row: any, column: Column): any {
    const value = row[column.key];
    return column.formatter ? column.formatter(value, row) : value;
  }
  onAction(action: TableAction, row: any) {
    this.action.emit({
      type: action.action,
      row: row
    });
  }
}
