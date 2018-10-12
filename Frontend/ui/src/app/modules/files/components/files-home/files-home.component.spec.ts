import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { FilesHomeComponent } from './files-home.component';

describe('FilesHomeComponent', () => {
  let component: FilesHomeComponent;
  let fixture: ComponentFixture<FilesHomeComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ FilesHomeComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(FilesHomeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
