import { ComponentFixture, TestBed } from '@angular/core/testing';

import { VendorInventoryDetails } from './vendor-inventory-details';

describe('VendorInventoryDetails', () => {
  let component: VendorInventoryDetails;
  let fixture: ComponentFixture<VendorInventoryDetails>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [VendorInventoryDetails],
    }).compileComponents();

    fixture = TestBed.createComponent(VendorInventoryDetails);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
