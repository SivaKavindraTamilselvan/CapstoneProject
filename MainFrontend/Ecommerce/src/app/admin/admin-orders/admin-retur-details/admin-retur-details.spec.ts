import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AdminReturDetails } from './admin-retur-details';

describe('AdminReturDetails', () => {
  let component: AdminReturDetails;
  let fixture: ComponentFixture<AdminReturDetails>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AdminReturDetails],
    }).compileComponents();

    fixture = TestBed.createComponent(AdminReturDetails);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
