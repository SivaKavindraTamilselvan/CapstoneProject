import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AdminDetailProduct } from './admin-detail-product';

describe('AdminDetailProduct', () => {
  let component: AdminDetailProduct;
  let fixture: ComponentFixture<AdminDetailProduct>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AdminDetailProduct],
    }).compileComponents();

    fixture = TestBed.createComponent(AdminDetailProduct);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
