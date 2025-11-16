import { HttpClient, HttpHeaders, HttpErrorResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, throwError } from 'rxjs';
import { catchError, tap, map } from 'rxjs/operators';
import { Courses } from '../models/courses';
import { Student } from '../models/student';
import { ApiResponse } from '../models/api-response';
import { Department } from '../models/department';

@Injectable({
  providedIn: 'root',
})
export class ApiDataService {
  private apiUrl = 'https://localhost:7120/api';

  constructor(private httpClient: HttpClient) {}

  // Method to get the authentication token from localStorage
  private getAuthToken(): string | null {
    return localStorage.getItem('token');
  }

  // Method to create headers with the auth token
  private getHeaders(): HttpHeaders {
    const token = this.getAuthToken();
    return new HttpHeaders({
      'Content-Type': 'application/json',
      Authorization: `Bearer ${token}`,
    });
  }

  // Fetch all courses
  getAllCourses(): Observable<Courses[]> {
    const headers = this.getHeaders();
    return this.httpClient
      .get<Courses[]>(`${this.apiUrl}/Courses`, { headers, observe: 'response' })
      .pipe(
        tap((response) => console.log('API Response:', response)),
        map((response) => response.body as Courses[])
      );
  }

  // Fetch students by course ID
  getStudentsByCourseId(courseId: number): Observable<Student[]> {
    const headers = this.getHeaders();
    return this.httpClient
      .get<Student[]>(`${this.apiUrl}/Students/CourseAttendance/${courseId}`, {
        headers,
        observe: 'response',
      })
      .pipe(
        tap((response) => console.log('API Response:', response)),
        map((response) => response.body as Student[])
      );
  }

  // Fetch attendance data for a specific course and week
  getCourseWeekAttendance(courseId: number, weekNum: number): Observable<Student[]> {
    const headers = this.getHeaders();
    return this.httpClient
      .get<Student[]>(`${this.apiUrl}/Students/CourseWeekAttendance/${courseId}/${weekNum}`, {
        headers,
        observe: 'response',
      })
      .pipe(
        tap((response) => console.log('API Response:', response)),
        map((response) => response.body as Student[])
      );
  }

  // Start attendance recording (GET request with query parameters)
  startAttendance(courseId: number, weekNum: number): Observable<any> {
    const headers = this.getHeaders();
    const url = `${this.apiUrl}/Fingerprint/TakeAttendance/Start?courseId=${courseId}&weekNum=${weekNum}`;
    return this.httpClient.get(url, { headers });
  }

  // End attendance recording
  endAttendance(courseId: number, weekNum: number): Observable<any> {
    const headers = this.getHeaders();
    return this.httpClient.put(`${this.apiUrl}/Fingerprint/TakeAttendance/End/${weekNum}/${courseId}`, {}, { headers });
  }
}