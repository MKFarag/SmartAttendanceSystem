import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AttendMainComponent } from './attend-main.component';

describe('AttendMainComponent', () => {
  let component: AttendMainComponent;
  let fixture: ComponentFixture<AttendMainComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AttendMainComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(AttendMainComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
