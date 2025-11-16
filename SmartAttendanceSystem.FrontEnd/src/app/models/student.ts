export interface Student {
  id: number;
  name: string;
  level: number;
  department: {
    id: number;
    name: string;
  };
  courses: {
    id: number;
    name: string;
    code: string;
  }[];
  selected?: boolean;
  attendance?: { week: number; status: string | boolean }[];
  total?: string;
  isSaving?: boolean;
  email?: string;
}