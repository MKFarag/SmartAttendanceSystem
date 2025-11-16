import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { ApiDataService } from '../../api-data.service';
import { Courses } from '../../models/courses';
import { NgIf, NgFor } from '@angular/common'; 

@Component({
  selector: 'app-attend-main',
  standalone: true,
  imports: [NgIf, NgFor], 
  templateUrl: './attend-main.component.html',
  styleUrl: './attend-main.component.css',
})
export class AttendMainComponent implements OnInit {
  courses: Courses[] = [];
  weeks: string[] = [];
  selectedCourseId: number | null = null;
  selectedWeek: string | null = null;
  showContent: boolean = true; 
  loading: boolean = false; 

  constructor(
    private apiDataService: ApiDataService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.fetchAllCourses();
    this.initializeWeeks();

    // Prevent page refresh and call endAttendance if needed
    window.onbeforeunload = () => {
      if (this.loading) {
        this.endAttendanceOnUnload();
        return false; 
      }
      return true;
    };
  }

  fetchAllCourses(): void {
    this.apiDataService.getAllCourses().subscribe({
      next: (data) => {
        console.log('Fetched Courses:', data);
        this.courses = data;
      },
      error: (error) => {
        console.error('Error fetching courses:', error);
      },
    });
  }

  initializeWeeks(): void {
    for (let i = 1; i <= 10; i++) {
      this.weeks.push(`Week ${i}`);
    }
  }

  onCourseChange(event: Event): void {
    const selectElement = event.target as HTMLSelectElement;
    this.selectedCourseId = selectElement.value ? +selectElement.value : null;
    this.selectedWeek = null; 
    console.log('Selected Course ID:', this.selectedCourseId);
  }

  onWeekChange(event: Event): void {
    const selectElement = event.target as HTMLSelectElement;
    this.selectedWeek = selectElement.value ? selectElement.value : null;
    console.log('Selected Week:', this.selectedWeek);
  }

  onStart(): void {
    if (!this.selectedCourseId || !this.selectedWeek) {
      alert('Please select a course and a week before starting.');
      return;
    }

    console.log('Before updating UI state:', {
      showContent: this.showContent,
      loading: this.loading,
    });

    // Update UI state
    this.showContent = false; // Hide left and right sections
    this.loading = true; // Show loading animation

    console.log('After updating UI state:', {
      showContent: this.showContent,
      loading: this.loading,
    });

    // Force change detection
    this.cdr.detectChanges();

    // Extract week number from selectedWeek string
    const weekNum = this.selectedWeek ? +this.selectedWeek.split(' ')[1] : null;

    if (weekNum === null) {
      alert('Invalid week number.');
      return;
    }

    // Call the start attendance endpoint with courseId and weekNum as query parameters
    this.apiDataService.startAttendance(this.selectedCourseId, weekNum).subscribe({
      next: (response) => {
        console.log('Attendance started successfully:', response);
        // No need to reset UI state here since the loading animation should remain visible
      },
      error: (error) => {
        console.error('Error starting attendance:', error);
        // Reset UI state on error
        // this.showContent = true;
        // this.loading = false;
        this.cdr.detectChanges();
      },
    });
  }

  onEnd(): void {
    if (!this.selectedCourseId || !this.selectedWeek) {
      alert('Please select a course and a week before ending.');
      return;
    }

    console.log('Attendance recording ended for:', {
      courseId: this.selectedCourseId,
      week: this.selectedWeek,
    });

    // Extract week number from selectedWeek string
    const weekNum = this.selectedWeek ? +this.selectedWeek.split(' ')[1] : null;

    if (weekNum === null) {
      alert('Invalid week number.');
      return;
    }

    // Show loading animation while ending attendance
    this.showContent = true; // Hide left and right sections
    this.loading = false;
    this.cdr.detectChanges();

    // Call the end attendance endpoint with courseId and weekNum
    this.apiDataService.endAttendance(this.selectedCourseId, weekNum).subscribe({
      next: (response) => {
        console.log('Attendance ended successfully:', response);
        this.resetForm(); // Reset the form after ending attendance
      },
      error: (error) => {
        console.error('Error ending attendance:', error);
        this.cdr.detectChanges();
      },
    });
  }

  resetForm(): void {
    this.selectedCourseId = null;
    this.selectedWeek = null;
    // this.showContent = true;
    // this.loading = false;
    this.cdr.detectChanges();
  }

  // Implement the canDeactivate method
  canDeactivate(): boolean {
    if (this.loading) {
      const confirmLeave = confirm('Attendance recording is in progress. Are you sure you want to leave?');
      if (confirmLeave) {
        this.endAttendanceOnUnload();
      }
      return confirmLeave;
    }
    return true;
  }

  // Method to call endAttendance on page unload
  endAttendanceOnUnload(): void {
    if (this.selectedCourseId && this.selectedWeek) {
      const weekNum = this.selectedWeek ? +this.selectedWeek.split(' ')[1] : null;
      if (weekNum !== null) {
        this.apiDataService.endAttendance(this.selectedCourseId, weekNum).subscribe({
          next: (response) => {
            console.log('Attendance ended successfully on page unload:', response);
          },
          error: (error) => {
            console.error('Error ending attendance on page unload:', error);
          },
        });
      }
    }
  }
}