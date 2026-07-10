import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AdminProfilePage } from './admin-profile-page';

describe('AdminProfilePage', () => {
  let component: AdminProfilePage;
  let fixture: ComponentFixture<AdminProfilePage>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AdminProfilePage],
    }).compileComponents();

    fixture = TestBed.createComponent(AdminProfilePage);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
