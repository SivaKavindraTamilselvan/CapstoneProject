import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AdminShipmentDetails } from './admin-shipment-details';

describe('AdminShipmentDetails', () => {
  let component: AdminShipmentDetails;
  let fixture: ComponentFixture<AdminShipmentDetails>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AdminShipmentDetails],
    }).compileComponents();

    fixture = TestBed.createComponent(AdminShipmentDetails);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
