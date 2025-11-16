import { CommonModule } from '@angular/common';
import { Component, OnInit, OnDestroy, HostListener } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ApiDataService } from '../../api-data.service';
import { Subject, takeUntil } from 'rxjs';
import { HttpClientModule } from '@angular/common/http';
import { Student } from '../../models/student';

interface Departments {
  id: number;
  name: string;
}

interface StudentForm {
  name: string;
  email: string;
  level: number | null;
  department: Departments | null;
}

interface ApiStudent {
  id: number;
  name: string;
  level: number;
  department: string;
  email?: string;
}

interface ApiResponse {
  items: Student[];
  pageNumber: number;
  totalPages: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
}

@Component({
  selector: 'app-fingerprint-main',
  standalone: true,
  imports: [FormsModule, CommonModule, HttpClientModule],
  templateUrl: './fingerprint-main.component.html',
  styleUrls: ['./fingerprint-main.component.css']
})
export class FingerprintMainComponent implements OnInit, OnDestroy {
  showModal = false;
  searchText = '';
  currentPage = 1;
  pageSize = 5;
  isLoading = false;
  isSavingFingerprint = new Map<number, boolean>();
  isAddingStudent = false;
  error: string | null = null;
  fingerprintError: string | null = null;
  showSuccess = false;
  successMessage = '';
  totalPages = 1;

  newStudent: StudentForm = {
    name: '',
    email: '',
    level: null,
    department: null
  };

  levels = [1, 2, 3, 4];
  departments: Departments[] = [];
  students: ApiStudent[] = [];
  private destroy$ = new Subject<void>();

  constructor(private apiDataService: ApiDataService) {}

  ngOnInit() {
    this.updatePageSizeBasedOnScreenWidth();
    this.fetchInitialData();
  }

  ngOnDestroy() {
    this.destroy$.next();
    this.destroy$.complete();
  }
  
  @HostListener('window:resize')
  onWindowResize() {
    this.updatePageSizeBasedOnScreenWidth();
    this.fetchStudentsWithoutFingerprint();
  }
  
  private updatePageSizeBasedOnScreenWidth() {
    const screenWidth = window.innerWidth;
    
    // Adjust these breakpoints as needed
    if (screenWidth < 768) {
      this.pageSize = 5; // Small screens
    } else if (screenWidth < 1200) {
      this.pageSize = 8; // Medium screens
    } else {
      this.pageSize = 10; // Large screens
    }
    
    console.log('Screen width:', screenWidth, 'Page size:', this.pageSize);
  }

  private fetchInitialData() {
    this.isLoading = true;
    this.apiDataService.getAllDepartments()
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (departments) => {
          this.departments = departments;
          this.fetchStudentsWithoutFingerprint();
        },
        error: (error) => {
          console.error('Error fetching departments:', error);
          this.error = 'Failed to load departments. Please try again later.';
          this.isLoading = false;
        }
      });
  }

  fetchStudentsWithoutFingerprint() {
    this.apiDataService.getStudentsWithoutFingerprint(this.currentPage, this.pageSize, this.searchText)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (response: ApiResponse) => {
          this.students = response.items.map(student => ({
            id: student.id,
            name: student.name,
            level: student.level,
            department: typeof student.department === 'string' ? student.department : student.department.name,
            email: student.email
          }));
          this.totalPages = response.totalPages;
          this.currentPage = response.pageNumber;
          this.isLoading = false;
        },
        error: (error) => {
          console.error('Error fetching students:', error);
          this.error = 'Failed to fetch students. Please try again later.';
          this.isLoading = false;
        }
      });
  }

  onSearch() {
    this.currentPage = 1;
    console.log('Searching for:', this.searchText);
    this.fetchStudentsWithoutFingerprint();
  }

  onSearchTextChange() {
    if (!this.searchText || this.searchText.trim() === '') {
      this.currentPage = 1;
      this.fetchStudentsWithoutFingerprint();
    }
  }

  onPageChange(page: number) {
    if (page >= 1 && page <= this.totalPages) {
      this.currentPage = page;
      this.fetchStudentsWithoutFingerprint();
    }
  }

  openAddStudentModal() {
    this.showModal = true;
    this.newStudent = {
      name: '',
      email: '',
      level: null,
      department: null
    };
    this.error = null;
  }

  closeModal() {
    this.showModal = false;
  }

  saveStudent() {
    if (!this.validateStudentForm()) {
      return;
    }

    const studentData = {
      Name: this.newStudent.name!.trim(),
      Email: this.newStudent.email!.trim(),
      Level: this.newStudent.level!,
      DepartmentId: this.newStudent.department!.id
    };

    this.isAddingStudent = true;
    this.error = null;

    this.apiDataService.createStudent(studentData)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: () => this.handleStudentCreationSuccess(),
        error: (error) => this.handleStudentCreationError(error)
      });
  }

  private validateStudentForm(): boolean {
    if (!this.newStudent.name || !this.newStudent.email || 
        this.newStudent.level === null || this.newStudent.department === null) {
      this.error = 'Please fill all required fields';
      return false;
    }
    return true;
  }

  private handleStudentCreationSuccess() {
    this.closeModal();
    this.fetchStudentsWithoutFingerprint();
    this.showSuccess = true;
    this.successMessage = 'Student added successfully!';
    setTimeout(() => this.showSuccess = false, 3000);
    this.isAddingStudent = false;
  }

  private handleStudentCreationError(error: any) {
    console.error('Error creating student:', error);
    if (error.status === 400 && error.error?.errors) {
      const validationErrors = error.error.errors;
      this.error = Object.values(validationErrors).flat().join('\n');
    } else {
      this.error = 'Failed to add student. Please try again.';
    }
    this.isAddingStudent = false;
  }

  saveFingerprint(studentId: number) {
    this.isSavingFingerprint.set(studentId, true);
    this.fingerprintError = null;

    this.apiDataService.saveFingerprint(studentId)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: () => this.handleFingerprintSaveSuccess(studentId),
        error: (error) => this.handleFingerprintSaveError(error, studentId)
      });
  }

  private handleFingerprintSaveSuccess(studentId: number) {
    this.showSuccess = true;
    this.successMessage = 'Fingerprint saved successfully!';
    this.fingerprintError = null;
    setTimeout(() => this.showSuccess = false, 3000);
    this.fetchStudentsWithoutFingerprint();
    this.isSavingFingerprint.set(studentId, false);
  }

  private handleFingerprintSaveError(error: any, studentId: number) {
    console.error('Error saving fingerprint:', error);
    // this.fingerprintError = 'Failed to save fingerprint. Please try again.';
    this.isSavingFingerprint.set(studentId, false);
    
    setTimeout(() => {
      this.fingerprintError = null;
    }, 5000);
  }

  isStudentSaving(studentId: number): boolean {
    return this.isSavingFingerprint.get(studentId) || false;
  }

  @HostListener('window:scroll', ['$event'])
  onWindowScroll() {
    const container = document.querySelector('.container-table');
    if (container) {
      const isScrolled = container.scrollTop > 50;
      container.classList.toggle('scrolled', isScrolled);
    }
  }
}
