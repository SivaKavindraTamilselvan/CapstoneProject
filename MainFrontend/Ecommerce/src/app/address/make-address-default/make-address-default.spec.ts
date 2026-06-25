import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MakeAddressDefault } from './make-address-default';

describe('MakeAddressDefault', () => {
  let component: MakeAddressDefault;
  let fixture: ComponentFixture<MakeAddressDefault>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MakeAddressDefault],
    }).compileComponents();

    fixture = TestBed.createComponent(MakeAddressDefault);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
