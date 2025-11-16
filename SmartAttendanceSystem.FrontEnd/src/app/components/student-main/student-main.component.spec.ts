import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ApiDataService } from '../../api-data.service'; 
import { Courses } from '../../models/courses'; 

@Component({
  selector: 'app-student-main',
  standalone: true,
  imports: [FormsModule, CommonModule],
  templateUrl: './student-main.component.html',
  styleUrl: './student-main.component.css'
})
export class StudentMainComponent implements OnInit {
  students = [
    { name: 'Ashrqat Ali Fawzy', level: 'Level 4', attendance: 'Attend', selected: false },
    { name: 'Ahmed Abdel Fatah', level: 'Level 4', attendance: 'Absent', selected: false },
    { name: 'Ashrqat Ali Fawzy', level: 'Level 4', attendance: 'Attend', selected: false },
    { name: 'Ahmed Abdel Fatah', level: 'Level 4', attendance: 'Absent', selected: false },
    { name: 'Ashrqat Ali Fawzy', level: 'Level 4', attendance: 'Attend', selected: false },
    { name: 'Ahmed Abdel Fatah', level: 'Level 4', attendance: 'Absent', selected: false },
    { name: 'Ahmed Abdel Fatah', level: 'Level 4', attendance: 'Attend', selected: false },
    { name: 'Ahmed Abdel Fatah', level: 'Level 4', attendance: 'Absent', selected: false },
    { name: 'Ahmed Abdel Fatah', level: 'Level 4', attendance: 'Attend', selected: false },
    { name: 'Ahmed Abdel Fatah', level: 'Level 4', attendance: 'Absent', selected: false },
    { name: 'Ahmed Abdel Fatah', level: 'Level 4', attendance: 'Attend', selected: false },
    { name: 'Ahmed Abdel Fatah', level: 'Level 4', attendance: 'Absent', selected: false },
    { name: 'Ahmed Abdel Fatah', level: 'Level 4', attendance: 'Attend', selected: false },
    { name: 'Ahmed Abdel Fatah', level: 'Level 4', attendance: 'Absent', selected: false },
    { name: 'Ahmed Abdel Fatah', level: 'Level 4', attendance: 'Attend', selected: false },
    { name: 'Ahmed Abdel Fatah', level: 'Level 4', attendance: 'Absent', selected: false },
  ];

  courses: Courses[] = []; 
  selectAll = false;

  constructor(private apiDataService: ApiDataService) { }

  ngOnInit(): void {
    this.fetchAllCourses();
  }

  // Fetch all courses
  fetchAllCourses(): void {
    this.apiDataService.getAllCourses().subscribe({
      next: (data) => {
        console.log('Fetched Courses:', data); 
        this.courses = data; 
      },
      error: (error) => {
        console.error('Error fetching courses:', error);
        alert('Failed to fetch courses. Please try again later.'); 
      }
    });
  }

  toggleSelectAll() {
    this.students.forEach(student => {
      student.selected = this.selectAll;
    });
  }
}