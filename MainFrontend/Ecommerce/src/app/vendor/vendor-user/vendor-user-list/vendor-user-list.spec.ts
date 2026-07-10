import { ComponentFixture, TestBed } from '@angular/core/testing';

import { VendorUserList } from './vendor-user-list';

describe('VendorUserList', () => {
  let component: VendorUserList;
  let fixture: ComponentFixture<VendorUserList>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [VendorUserList],
    }).compileComponents();

    fixture = TestBed.createComponent(VendorUserList);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
