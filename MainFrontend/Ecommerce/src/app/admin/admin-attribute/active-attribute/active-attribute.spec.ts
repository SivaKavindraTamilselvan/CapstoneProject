import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ActiveAttribute } from './active-attribute';

describe('ActiveAttribute', () => {
  let component: ActiveAttribute;
  let fixture: ComponentFixture<ActiveAttribute>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ActiveAttribute],
    }).compileComponents();

    fixture = TestBed.createComponent(ActiveAttribute);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
