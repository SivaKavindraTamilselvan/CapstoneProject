import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UpdateAdminShipment } from './update-admin-shipment';

describe('UpdateAdminShipment', () => {
  let component: UpdateAdminShipment;
  let fixture: ComponentFixture<UpdateAdminShipment>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [UpdateAdminShipment],
    }).compileComponents();

    fixture = TestBed.createComponent(UpdateAdminShipment);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
