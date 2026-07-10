import { ComponentFixture, TestBed } from '@angular/core/testing';

import { InactiveMappedAttribute } from './inactive-mapped-attribute';

describe('InactiveMappedAttribute', () => {
  let component: InactiveMappedAttribute;
  let fixture: ComponentFixture<InactiveMappedAttribute>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [InactiveMappedAttribute],
    }).compileComponents();

    fixture = TestBed.createComponent(InactiveMappedAttribute);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
