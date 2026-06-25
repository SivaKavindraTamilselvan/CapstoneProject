import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UpdateRejectedProduct } from './update-rejected-product';

describe('UpdateRejectedProduct', () => {
  let component: UpdateRejectedProduct;
  let fixture: ComponentFixture<UpdateRejectedProduct>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [UpdateRejectedProduct],
    }).compileComponents();

    fixture = TestBed.createComponent(UpdateRejectedProduct);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
