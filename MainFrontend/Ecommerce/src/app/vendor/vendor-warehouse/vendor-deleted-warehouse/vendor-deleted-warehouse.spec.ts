import { ComponentFixture, TestBed } from '@angular/core/testing';

import { VendorDeletedWarehouse } from './vendor-deleted-warehouse';

describe('VendorDeletedWarehouse', () => {
  let component: VendorDeletedWarehouse;
  let fixture: ComponentFixture<VendorDeletedWarehouse>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [VendorDeletedWarehouse],
    }).compileComponents();

    fixture = TestBed.createComponent(VendorDeletedWarehouse);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
