import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ActiveVendor } from './active-vendor';

describe('ActiveVendor', () => {
  let component: ActiveVendor;
  let fixture: ComponentFixture<ActiveVendor>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ActiveVendor],
    }).compileComponents();

    fixture = TestBed.createComponent(ActiveVendor);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
