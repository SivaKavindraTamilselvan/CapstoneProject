import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DetailedCardComponenet } from './detailed-card-componenet';

describe('DetailedCardComponenet', () => {
  let component: DetailedCardComponenet;
  let fixture: ComponentFixture<DetailedCardComponenet>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DetailedCardComponenet],
    }).compileComponents();

    fixture = TestBed.createComponent(DetailedCardComponenet);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
