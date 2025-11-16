import { HttpClient, HttpHeaders, HttpErrorResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, throwError } from 'rxjs';
import { catchError, tap, map } from 'rxjs/operators';
import { Courses } from './models/courses';
import { Student } from './models/student';
import { ApiResponse } from './models/api-response';
import { Department } from './models/department';

@Injectable({
  providedIn: 'root',
})
export class ApiDataService {
  private apiUrl = 'https://localhost:7120/api';

  constructor(private http: HttpClient) {}

  private getAuthToken(): string | null {
    return localStorage.getItem('token');
  }

  private getHeaders(): HttpHeaders {
    const token = this.getAuthToken();
    return new HttpHeaders({
      'Content-Type': 'application/json',
      Authorization: `Bearer ${token}`,
    });
  }

  private handleError(error: HttpErrorResponse) {
    let errorMessage = 'An unknown error occurred!';
    if (error.error instanceof ErrorEvent) {
      errorMessage = `Error: ${error.error.message}`;
    } else {
      errorMessage = `Error Code: ${error.status}\nMessage: ${error.message}`;
      if (error.status === 400 && error.error?.errors) {
        errorMessage = Object.values(error.error.errors).flat().join('\n');
      }
    }
    return throwError(() => errorMessage);
  }

  getAllCourses(): Observable<Courses[]> {
    const headers = this.getHeaders();
    return this.http
      .get<Courses[]>(`${this.apiUrl}/Courses`, { headers, observe: 'response' })
      .pipe(
        tap((response) => console.log('API Response:', response)),
        map((response) => response.body as Courses[]),
        catchError(this.handleError)
      );
  }

  getStudentsByCourseId(courseId: number): Observable<Student[]> {
    const headers = this.getHeaders();
    return this.http
      .get<Student[]>(`${this.apiUrl}/Students/attendance/${courseId}`, {
        headers,
        observe: 'response',
      })
      .pipe(
        tap((response) => console.log('API Response:', response)),
        map((response) => response.body as Student[]),
        catchError(this.handleError)
      );
  }

  getCourseWeekAttendance(courseId: number, weekNum: number): Observable<Student[]> {
    const headers = this.getHeaders();
    return this.http
      .get<Student[]>(`${this.apiUrl}/Students/attendance/${courseId}/${weekNum}`, {
        headers,
        observe: 'response',
      })
      .pipe(
        tap((response) => console.log('API Response:', response)),
        map((response) => response.body as Student[]),
        catchError(this.handleError)
      );
  }

  startAttendance(courseId: number, weekNum: number): Observable<any> {
    const headers = this.getHeaders();
    const url = `${this.apiUrl}/Fingerprint/attendance/Start?courseId=${courseId}&weekNum=${weekNum}`;
    return this.http.get(url, { headers }).pipe(
      catchError(this.handleError)
    );
  }

  endAttendance(courseId: number, weekNum: number): Observable<any> {
    const headers = this.getHeaders();
    return this.http.put(
      `${this.apiUrl}/Fingerprint/attendance/End/${courseId}/${weekNum}`, 
      {}, 
      { headers }
    ).pipe(
      catchError(this.handleError)
    );
  }

  getStudentsWithoutFingerprint(
    pageNumber: number, 
    pageSize: number, 
    searchTerm: string = ''
  ): Observable<ApiResponse> {
    const headers = this.getHeaders();
    let url = `${this.apiUrl}/Students/no-finger?pageNumber=${pageNumber}&pageSize=${pageSize}`;
    
    if (searchTerm) {
      url += `&SearchValue=${encodeURIComponent(searchTerm)}`;
    }
    
    console.log('API URL:', url);
    console.log('Search Term:', searchTerm);

    return this.http.get<ApiResponse>(url, { headers }).pipe(
      catchError(this.handleError)
    );
  }

  saveFingerprint(studentId: number): Observable<any> {
    const headers = this.getHeaders();
    return this.http.post(
      `${this.apiUrl}/Fingerprint/new/${studentId}`,
      {},
      { headers }
    ).pipe(
      catchError(this.handleError)
    );
  }

  createStudent(studentData: any): Observable<any> {
    const headers = this.getHeaders();
    return this.http.post(
      `${this.apiUrl}/Students`,
      studentData,
      { headers }
    ).pipe(
      catchError(this.handleError)
    );
  }

  getAllDepartments(): Observable<Department[]> {
    const headers = this.getHeaders();
    return this.http.get<Department[]>(
      `${this.apiUrl}/Departments`,
      { headers }
    ).pipe(
      catchError(this.handleError)
    );
  }
}