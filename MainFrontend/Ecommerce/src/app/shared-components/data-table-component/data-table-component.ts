import { Component, EventEmitter, Input, Output } from '@angular/core';
import { Column } from './column.model';
import { NgClass } from '@angular/common';
import { TableAction } from './table-actions.model';

@Component({
  selector: 'app-data-table-component',
  imports: [NgClass],
  templateUrl: './data-table-component.html',
  styleUrl: './data-table-component.css',
})
export class DataTableComponent {
  @Input() columns: Column[] = [];
  @Input() rows: any[] = [];
  @Input() actions: TableAction[] = [];
  @Output() action = new EventEmitter<{type: string;row: any;}>();

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
