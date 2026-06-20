import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ActivateAdmin } from './activate-admin';

describe('ActivateAdmin', () => {
  let component: ActivateAdmin;
  let fixture: ComponentFixture<ActivateAdmin>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ActivateAdmin],
    }).compileComponents();

    fixture = TestBed.createComponent(ActivateAdmin);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
