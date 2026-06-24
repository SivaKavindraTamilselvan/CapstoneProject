import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ReviewProduct } from './review-product';

describe('ReviewProduct', () => {
  let component: ReviewProduct;
  let fixture: ComponentFixture<ReviewProduct>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ReviewProduct],
    }).compileComponents();

    fixture = TestBed.createComponent(ReviewProduct);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
