import { ComponentFixture, TestBed } from '@angular/core/testing';

import { InactiveAttribute } from './inactive-attribute';

describe('InactiveAttribute', () => {
  let component: InactiveAttribute;
  let fixture: ComponentFixture<InactiveAttribute>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [InactiveAttribute],
    }).compileComponents();

    fixture = TestBed.createComponent(InactiveAttribute);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
