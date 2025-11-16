import { TestBed } from '@angular/core/testing';
import { CanActivateFn } from '@angular/router';

import { canDeactivateAttendanceGuard } from './can-deactivate-attendance.guard';

describe('canDeactivateAttendanceGuard', () => {
  const executeGuard: CanActivateFn = (...guardParameters) => 
      TestBed.runInInjectionContext(() => canDeactivateAttendanceGuard(...guardParameters));

  beforeEach(() => {
    TestBed.configureTestingModule({});
  });

  it('should be created', () => {
    expect(executeGuard).toBeTruthy();
  });
});
