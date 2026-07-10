import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ApprovalStatusComponenet } from './approval-status-componenet';

describe('ApprovalStatusComponenet', () => {
  let component: ApprovalStatusComponenet;
  let fixture: ComponentFixture<ApprovalStatusComponenet>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ApprovalStatusComponenet],
    }).compileComponents();

    fixture = TestBed.createComponent(ApprovalStatusComponenet);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
