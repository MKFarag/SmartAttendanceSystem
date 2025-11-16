import { Component, EventEmitter, Output } from '@angular/core';
import { Router, RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { AccountService } from '../../account.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [FormsModule, CommonModule, RouterModule],
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css'],
})
export class LoginComponent {
  loginObj: any = {
    email: '',
    password: '',
  };

  errorMessage: string = '';
  isLoading: boolean = false;

  @Output() loginSuccess = new EventEmitter<void>();

  constructor(private router: Router, private accService: AccountService) {}

  navigateToSignup() {
    this.router.navigate(['/signup']);
  }

  onLogin() {
    this.errorMessage = ''; 
    this.isLoading = true; 

    const loginObj = {
      email: this.loginObj.email,
      password: this.loginObj.password,
    };

    this.accService.onLogin(loginObj).subscribe({
      next: (res: any) => {
        console.log('Login successful', res);

        localStorage.setItem('token', res.token);
        localStorage.setItem('refreshToken', res.refreshToken);
        localStorage.setItem('expiresIn', res.expiresIn);
        localStorage.setItem('refreshTokenExpiration', res.refreshTokenExpiration);

        this.accService.getUserDetails().subscribe({
          next: (userDetails: any) => {
            console.log('User details:', userDetails);
            this.isLoading = false;
            this.loginSuccess.emit();
            this.router.navigate(['/attend']);
          },
          error: (error) => {
            console.error('Failed to fetch user details', error);
            this.errorMessage = 'Invalid User Data, Please try again.';
            this.isLoading = false;
          },
        });
      },
      error: (error) => {
        console.error('Login failed', error);
        this.isLoading = false;

        if (error.status === 401) {
          this.errorMessage = 'Invalid email or password. Please try again.';
        } else if (error.status === 0) {
          this.errorMessage = 'Unable to connect to the server. Please check your internet connection.';
        } else if (error.status === 500) {
          this.errorMessage = 'Something went wrong on the server. Please try again later.';
        } else if (error.status === 403) {
          this.errorMessage = 'Access denied.';
        } else {
          this.errorMessage = 'An unexpected error occurred. Please try again later.';
        }
      },
    });
  }
}