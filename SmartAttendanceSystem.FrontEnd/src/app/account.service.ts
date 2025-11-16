import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class AccountService {
  private apiUrl = 'https://localhost:7120'; 

  constructor(private http: HttpClient) {}

  // Login API call
  onLogin(loginObj: { email: string; password: string }): Observable<any> {
    const headers = new HttpHeaders({
      'Content-Type': 'application/json',
    });

    return this.http.post(`${this.apiUrl}/Auth`, loginObj, { headers });
  }

  // Sign Up API call
  onSignUp(signUpObj: { 
    name: string; 
    email: string; 
    password: string; 
    department: string; 
    level: number; 
  }): Observable<any> {
    const headers = new HttpHeaders({
      'Content-Type': 'application/json',
    });

    return this.http.post(`${this.apiUrl}/Auth/register`, signUpObj, { headers });
  }

  // Fetch user details using the /me endpoint
  getUserDetails(): Observable<any> {
    const token = localStorage.getItem('token'); 
    if (!token) {
      throw new Error('No token found'); 
    }

    const headers = new HttpHeaders({
      Authorization: `Bearer ${token}`,
    });

    return this.http.get(`${this.apiUrl}/me`, { headers });
  }
}