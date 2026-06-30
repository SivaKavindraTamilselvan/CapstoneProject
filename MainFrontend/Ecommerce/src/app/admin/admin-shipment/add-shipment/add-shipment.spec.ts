import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AddShipment } from './add-shipment';

describe('AddShipment', () => {
  let component: AddShipment;
  let fixture: ComponentFixture<AddShipment>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AddShipment],
    }).compileComponents();

    fixture = TestBed.createComponent(AddShipment);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
