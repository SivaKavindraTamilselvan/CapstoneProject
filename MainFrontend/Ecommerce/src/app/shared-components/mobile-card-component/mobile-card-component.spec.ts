import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MobileCardComponent } from './mobile-card-component';

describe('MobileCardComponent', () => {
  let component: MobileCardComponent;
  let fixture: ComponentFixture<MobileCardComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MobileCardComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(MobileCardComponent);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
