import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GetAddress } from './get-address';

describe('GetAddress', () => {
  let component: GetAddress;
  let fixture: ComponentFixture<GetAddress>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [GetAddress],
    }).compileComponents();

    fixture = TestBed.createComponent(GetAddress);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
