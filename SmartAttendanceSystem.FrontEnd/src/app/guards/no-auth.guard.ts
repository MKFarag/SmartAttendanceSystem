import { Injectable } from '@angular/core';
import { CanActivate, Router } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class NoAuthGuard implements CanActivate {
  constructor(private router: Router) {}

  canActivate(): boolean {
    // Check if user is already logged in
    const token = localStorage.getItem('token');
    
    if (!token) {
      // No token exists, allow access to auth routes
      return true;
    } else {
      // Token exists, redirect to main app
      this.router.navigate(['/attend']);
      return false;
    }
  }
}