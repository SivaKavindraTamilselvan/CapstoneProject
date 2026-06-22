import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ActiveMappedAttribute } from './active-mapped-attribute';

describe('ActiveMappedAttribute', () => {
  let component: ActiveMappedAttribute;
  let fixture: ComponentFixture<ActiveMappedAttribute>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ActiveMappedAttribute],
    }).compileComponents();

    fixture = TestBed.createComponent(ActiveMappedAttribute);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
