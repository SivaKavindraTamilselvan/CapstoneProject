import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DeactivateAdmin } from './deactivate-admin';

describe('DeactivateAdmin', () => {
  let component: DeactivateAdmin;
  let fixture: ComponentFixture<DeactivateAdmin>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DeactivateAdmin],
    }).compileComponents();

    fixture = TestBed.createComponent(DeactivateAdmin);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
