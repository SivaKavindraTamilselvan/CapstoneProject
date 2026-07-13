import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GetWarehouseAddress } from './get-warehouse-address';

describe('GetWarehouseAddress', () => {
  let component: GetWarehouseAddress;
  let fixture: ComponentFixture<GetWarehouseAddress>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [GetWarehouseAddress],
    }).compileComponents();

    fixture = TestBed.createComponent(GetWarehouseAddress);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
