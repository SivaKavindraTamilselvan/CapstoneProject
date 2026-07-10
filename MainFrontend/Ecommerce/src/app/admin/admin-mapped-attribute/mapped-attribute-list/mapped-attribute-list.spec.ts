import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MappedAttributeList } from './mapped-attribute-list';

describe('MappedAttributeList', () => {
  let component: MappedAttributeList;
  let fixture: ComponentFixture<MappedAttributeList>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MappedAttributeList],
    }).compileComponents();

    fixture = TestBed.createComponent(MappedAttributeList);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
