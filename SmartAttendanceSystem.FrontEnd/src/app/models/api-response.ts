import { Student } from './student';

export interface ApiResponse {
  items: Student[];
  pageNumber: number;
  totalPages: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
} 