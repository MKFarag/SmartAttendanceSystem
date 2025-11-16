import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { AccountService } from '../../account.service';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-sign-up',
  standalone: true,
  imports: [FormsModule, CommonModule, RouterModule],
  templateUrl: './sign-up.component.html',
  styleUrls: ['./sign-up.component.css'],
})
export class SignUpComponent {
  signObj: any = {
    name: '',
    email: '',
    password: '',
    adminPassword: ''
  };

  errorMessage: string = '';
  isLoading: boolean = false;

  constructor(private router: Router, private accService: AccountService) {}

  onSign() {
    this.errorMessage = '';
    this.isLoading = true;

    // Validate Name
    if (!this.signObj.name) {
      this.errorMessage = 'Name is required';
      this.isLoading = false;
      return;
    }
    if (this.signObj.name.length < 3 || this.signObj.name.length > 200) {
      this.errorMessage = 'Name must be between 3 and 200 characters';
      this.isLoading = false;
      return;
    }

    // Validate Email
    if (!this.signObj.email) {
      this.errorMessage = 'Email is required';
      this.isLoading = false;
      return;
    }
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    if (!emailRegex.test(this.signObj.email)) {
      this.errorMessage = 'Please enter a valid email address';
      this.isLoading = false;
      return;
    }

    // Validate Password
    if (!this.signObj.password) {
      this.errorMessage = 'Password is required';
      this.isLoading = false;
      return;
    }
    const passwordRegex = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$/;
    if (!passwordRegex.test(this.signObj.password)) {
  
      this.isLoading = false;
      return;
    }

    // Validate Instructor Password
    if (!this.signObj.adminPassword) {
      this.errorMessage = 'Instructor password is required';
      this.isLoading = false;
      return;
    }

    // Prepare API request
    const requestBody = {
      Name: this.signObj.name.trim(),
      Email: this.signObj.email.trim(),
      Password: this.signObj.password,
      InstructorPassword: this.signObj.adminPassword
    };

    // Make API call
    fetch('https://localhost:7120/Auth/register', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        'Accept': 'application/json'
      },
      body: JSON.stringify(requestBody)
    })
    .then(async response => {
      const data = await response.json();
      
      if (!response.ok) {
        // Handle API validation errors
        if (data.errors) {
          const errorMessages = [];
          for (const key in data.errors) {
            errorMessages.push(...data.errors[key]);
          }
          this.errorMessage = errorMessages.join('\n');
        } else {
          this.errorMessage = data.title || 'Registration failed';
        }
        throw new Error(this.errorMessage);
      }
      return data;
    })
    .then(data => {
      console.log('Registration successful', data);
      this.goToLogin();
    })
    .catch(error => {
      console.error('Registration error:', error);
      if (!this.errorMessage) {
        this.errorMessage = 'An error occurred during registration. Please try again.';
      }
    })
    .finally(() => {
      this.isLoading = false;
    });
  }

  goToLogin() {
    this.router.navigate(['/auth/login']).then(() => {
      console.log('Navigation to login complete');
    }).catch(error => {
      console.error('Navigation error:', error);
    });
  }
}