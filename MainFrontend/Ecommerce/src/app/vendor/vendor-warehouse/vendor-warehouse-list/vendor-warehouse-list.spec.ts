import { ComponentFixture, TestBed } from '@angular/core/testing';

import { VendorWarehouseList } from './vendor-warehouse-list';

describe('VendorWarehouseList', () => {
  let component: VendorWarehouseList;
  let fixture: ComponentFixture<VendorWarehouseList>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [VendorWarehouseList],
    }).compileComponents();

    fixture = TestBed.createComponent(VendorWarehouseList);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
