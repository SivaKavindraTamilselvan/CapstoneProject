import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ReviewPopupComponent } from './review-popup-component';

describe('ReviewPopupComponent', () => {
  let component: ReviewPopupComponent;
  let fixture: ComponentFixture<ReviewPopupComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ReviewPopupComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(ReviewPopupComponent);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
