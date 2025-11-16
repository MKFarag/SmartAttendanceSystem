import { Injectable } from '@angular/core';
import { CanDeactivate } from '@angular/router';
import { AttendMainComponent } from './components/attend-main/attend-main.component';

@Injectable({
  providedIn: 'root',
})
export class CanDeactivateAttendanceGuard implements CanDeactivate<AttendMainComponent> {
  canDeactivate(component: AttendMainComponent): boolean {
    return component.canDeactivate();
  }
}