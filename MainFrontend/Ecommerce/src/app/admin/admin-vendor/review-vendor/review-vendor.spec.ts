import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ReviewVendor } from './review-vendor';

describe('ReviewVendor', () => {
  let component: ReviewVendor;
  let fixture: ComponentFixture<ReviewVendor>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ReviewVendor],
    }).compileComponents();

    fixture = TestBed.createComponent(ReviewVendor);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
