import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { Router, NavigationEnd, RouterModule } from '@angular/router';
import { filter } from 'rxjs/operators';
import { AccountService } from './account.service';
import { NavComponent } from './components/nav/nav.component';
import { SideBarComponent } from './components/side-bar/side-bar.component';
import { CommonModule } from '@angular/common';
import { LoginComponent } from './components/login/login.component';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css'],
  standalone: true,
  imports: [RouterModule, NavComponent, SideBarComponent, CommonModule, LoginComponent]
})
export class AppComponent implements OnInit {
  isLoggedIn = false;
  userDetails: any = null;
  showModal = false;
  currentUrl: string = '';

  constructor(
    private router: Router,
    private accService: AccountService,
    private cdr: ChangeDetectorRef
  ) {}

  get isSignupRoute(): boolean {
    return this.currentUrl === '/auth/signup';
  }

  get isLoginRoute(): boolean {
    return this.currentUrl === '/auth/login';
  }

  ngOnInit() {
    this.currentUrl = this.router.url;

    const token = localStorage.getItem('token');
    if (token) {
      this.isLoggedIn = true;
      this.fetchUserDetails();
    } else {
      this.isLoggedIn = false;
      this.router.navigate(['/auth/login']);
    }

    this.router.events
      .pipe(filter((event): event is NavigationEnd => event instanceof NavigationEnd))
      .subscribe((event: NavigationEnd) => {
        this.currentUrl = event.urlAfterRedirects;
      });
  }

  onLoginSuccess() {
    const token = localStorage.getItem('token');
    if (token) {
      this.isLoggedIn = true;
      this.fetchUserDetails();
    }
  }

  private fetchUserDetails() {
    this.accService.getUserDetails().subscribe({
      next: (data: any) => {
        this.userDetails = data;
        this.cdr.detectChanges();
      },
      error: () => {
        this.isLoggedIn = false;
        this.router.navigate(['/auth/login']);
      },
    });
  }

  onLogout() {
    this.isLoggedIn = false;
    this.userDetails = null;
    localStorage.removeItem('token');
    this.router.navigate(['/auth/login']);
  }
}
