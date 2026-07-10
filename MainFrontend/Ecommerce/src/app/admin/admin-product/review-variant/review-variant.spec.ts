import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ReviewVariant } from './review-variant';

describe('ReviewVariant', () => {
  let component: ReviewVariant;
  let fixture: ComponentFixture<ReviewVariant>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ReviewVariant],
    }).compileComponents();

    fixture = TestBed.createComponent(ReviewVariant);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
