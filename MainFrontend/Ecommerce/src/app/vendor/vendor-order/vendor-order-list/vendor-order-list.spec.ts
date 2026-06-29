import { ComponentFixture, TestBed } from '@angular/core/testing';

import { VendorOrderList } from './vendor-order-list';

describe('VendorOrderList', () => {
  let component: VendorOrderList;
  let fixture: ComponentFixture<VendorOrderList>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [VendorOrderList],
    }).compileComponents();

    fixture = TestBed.createComponent(VendorOrderList);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
