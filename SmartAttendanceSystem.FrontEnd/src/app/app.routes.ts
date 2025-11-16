import { LoginComponent } from './components/login/login.component';
import { SignUpComponent } from './components/sign-up/sign-up.component';
import { AttendMainComponent } from './components/attend-main/attend-main.component';
import { StudentMainComponent } from './components/student-main/student-main.component';
import { FingerprintMainComponent } from './components/fingerprint-main/fingerprint-main.component';
import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AuthGuard } from './guards/auth.guard';
import { NoAuthGuard } from './guards/no-auth.guard';
import { CanDeactivateAttendanceGuard } from './can-deactivate-attendance.guard';

export const routes: Routes = [
  { 
    path: 'auth',
    children: [
      { 
        path: 'login', 
        component: LoginComponent,
        canActivate: [NoAuthGuard]
      },
      { 
        path: 'signup', 
        component: SignUpComponent,
        canActivate: [NoAuthGuard]
      },
      { path: '', redirectTo: 'login', pathMatch: 'full' }
    ]
  },
  {
    path: 'attend',
    component: AttendMainComponent,
    canActivate: [AuthGuard],
    canDeactivate: [CanDeactivateAttendanceGuard],
  },
  { 
    path: 'students', 
    component: StudentMainComponent, 
    canActivate: [AuthGuard] 
  },
  { 
    path: 'fingerprint', 
    component: FingerprintMainComponent, 
    canActivate: [AuthGuard] 
  },
  { path: '', redirectTo: 'auth/login', pathMatch: 'full' },
  { path: '**', redirectTo: 'auth/login' },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
})
export class AppRoutingModule {}