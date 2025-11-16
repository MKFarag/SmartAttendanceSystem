import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule, Router } from '@angular/router';
import { Component, Input, Output, EventEmitter, OnChanges, SimpleChanges } from '@angular/core';

@Component({
  selector: 'app-side-bar',
  standalone: true,
  imports: [FormsModule, CommonModule, RouterModule],
  templateUrl: './side-bar.component.html',
  styleUrl: './side-bar.component.css',
})
export class SideBarComponent implements OnChanges {
  @Input() userDetails: any; 
  @Output() logout = new EventEmitter<void>();

  isSidebarVisible = false;

  constructor(private router: Router) {}

  ngOnChanges(changes: SimpleChanges) {
    if (changes['userDetails'] && changes['userDetails'].currentValue) {
      this.isSidebarVisible = true;
    }
  }

  toggleSidebar(): void {
    this.isSidebarVisible = !this.isSidebarVisible;
  }

  onLogout() {
    this.logout.emit();
    this.router.navigate(['/log']);
  }
}