import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ReviewReturnComponent } from './review-return-component';

describe('ReviewReturnComponent', () => {
  let component: ReviewReturnComponent;
  let fixture: ComponentFixture<ReviewReturnComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ReviewReturnComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(ReviewReturnComponent);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
