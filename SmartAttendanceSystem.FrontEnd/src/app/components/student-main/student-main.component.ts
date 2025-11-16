import { CommonModule } from '@angular/common';
import { Component, OnInit, OnDestroy } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Subject, takeUntil } from 'rxjs';
import { ApiDataService } from '../../api-data.service';
import { Courses } from '../../models/courses';
import { Student } from '../../models/student';

@Component({
  selector: 'app-student-main',
  standalone: true,
  imports: [FormsModule, CommonModule],
  templateUrl: './student-main.component.html',
  styleUrl: './student-main.component.css',
})
export class StudentMainComponent implements OnInit, OnDestroy {
  // Public properties
  public students: Student[] = [];
  public filteredStudents: Student[] = [];
  public courses: Courses[] = [];
  public selectAll = false;
  public selectedCourseId: number | null = null;
  public selectedWeek: number | null = null;
  public isLoading = false;
  public error: string | null = null;

  // Private properties
  private destroy$ = new Subject<void>();

  constructor(private apiDataService: ApiDataService) {
    // Initialize arrays with type assertions
    this.students = [] as Student[];
    this.filteredStudents = [] as Student[];
    this.courses = [] as Courses[];
  }

  ngOnInit(): void {
    this.fetchAllCourses();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  // Public methods
  public fetchAllCourses(): void {
    this.isLoading = true;
    this.error = null;
    this.apiDataService.getAllCourses()
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (data) => {
          this.courses = data;
          this.isLoading = false;
        },
        error: (error) => {
          console.error('Error fetching courses:', error);
          this.error = 'Failed to fetch courses. Please try again later.';
          this.isLoading = false;
        },
      });
  }

  public onCourseChange(event: Event): void {
    const selectElement = event.target as HTMLSelectElement;
    this.selectedCourseId = selectElement.value ? +selectElement.value : null;
    this.fetchData();
  }

  public onWeekChange(event: Event): void {
    const selectElement = event.target as HTMLSelectElement;
    this.selectedWeek = selectElement.value ? +selectElement.value : 0;
    this.fetchData();
  }

  public fetchData(): void {
    if (!this.selectedCourseId) {
      this.students = [];
      this.filteredStudents = [];
      return;
    }

    this.isLoading = true;
    this.error = null;

    if (this.selectedWeek !== 0 && this.selectedWeek !== null) {
      this.fetchCourseWeekAttendance(this.selectedCourseId, this.selectedWeek);
    } else {
      this.fetchStudentsByCourseId(this.selectedCourseId);
    }
  }

  public fetchStudentsByCourseId(courseId: number): void {
    this.apiDataService.getStudentsByCourseId(courseId)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (data) => {
          this.students = data;
          this.filteredStudents = data;
          this.isLoading = false;
        },
        error: (error) => {
          console.error('Error fetching students by course ID:', error);
          // this.error = 'Failed to fetch students. Please try again later.';
          this.isLoading = false;
        },
      });
  }

  public fetchCourseWeekAttendance(courseId: number, weekNum: number): void {
    this.apiDataService.getCourseWeekAttendance(courseId, weekNum)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (data) => {
          this.students = data.map((student: any) => ({
            id: student.id,
            name: student.name,
            level: student.level,
            department: { id: 0, name: student.departmentName },
            courses: [],
            attendance: [{ week: weekNum, status: student.attend }],
            total: '0/0',
          }));
          this.filteredStudents = this.students;
          this.isLoading = false;
        },
        error: (error) => {
          console.error('Error fetching attendance data:', error);
          this.error = 'Failed to fetch attendance data. Please try again later.';
          this.isLoading = false;
        },
      });
  }

  public getAttendanceStatus(student: Student, week: number | null): string {
    if (!week || !student.attendance) return 'Not Attend';

    const attendance = student.attendance.find((a) => a.week === week);
    if (!attendance) return 'Not Attend';

    if (typeof attendance.status === 'boolean') {
      return attendance.status ? 'Attend' : 'Absent';
    }
    return attendance.status || 'Absent';
  }

  public toggleSelectAll(): void {
    this.filteredStudents.forEach((student) => {
      student.selected = this.selectAll;
    });
  }

  public clearError(): void {
    this.error = null;
  }
}