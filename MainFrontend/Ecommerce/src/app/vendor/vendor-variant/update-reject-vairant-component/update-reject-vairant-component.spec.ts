import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UpdateRejectVairantComponent } from './update-reject-vairant-component';

describe('UpdateRejectVairantComponent', () => {
  let component: UpdateRejectVairantComponent;
  let fixture: ComponentFixture<UpdateRejectVairantComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [UpdateRejectVairantComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(UpdateRejectVairantComponent);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
