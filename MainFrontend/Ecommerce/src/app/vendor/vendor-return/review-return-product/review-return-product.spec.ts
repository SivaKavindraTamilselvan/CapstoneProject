import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ReviewReturnProduct } from './review-return-product';

describe('ReviewReturnProduct', () => {
  let component: ReviewReturnProduct;
  let fixture: ComponentFixture<ReviewReturnProduct>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ReviewReturnProduct],
    }).compileComponents();

    fixture = TestBed.createComponent(ReviewReturnProduct);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
