// src/app/services/fingerprint.service.ts
import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class FingerprintService {
  private baseUrl = 'https://localhost:7120/api'; // Base API URL

  constructor(private http: HttpClient) {}

  private getAuthHeaders(): HttpHeaders {
    const token = localStorage.getItem('token');
    if (!token) {
      throw new Error('No token found');
    }

    return new HttpHeaders({
      'Content-Type': 'application/json',
      Authorization: `Bearer ${token}`,
    });
  }

  // Save fingerprint for a specific student
  saveNewFingerprint(studentId: number): Observable<any> {
    const headers = this.getAuthHeaders();
    return this.http.post(`${this.baseUrl}/Fingerprint/new/${studentId}`, {}, { headers });
  }

  // Get students without fingerprints (two possible endpoints: you can keep both or choose one)
  getStudentsNoFinger(): Observable<any> {
    const headers = this.getAuthHeaders();
    return this.http.get(`${this.baseUrl}/Students/no-finger`, { headers });
  }

  getStudentsWithoutFingerprints(): Observable<any> {
    const headers = this.getAuthHeaders();
    return this.http.get(`${this.baseUrl}/Fingerprint/students-without-fingerprints`, { headers });
  }
}


