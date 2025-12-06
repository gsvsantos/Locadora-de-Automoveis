export interface EmployeeDto {
  fullName: string;
  admissionDate: Date;
  salary: number;
}

export interface CreateEmployeeDto extends EmployeeDto {
  userName: string;
  email: string;
  phoneNumber: string;
  password: string;
}

export interface Employee extends EmployeeDto {
  id: string;
}

export interface ListEmployeesDto {
  quantity: number;
  employees: Employee[];
}

export interface EmployeeDetailsApiDto {
  employee: {
    id: string;
    fullName: string;
    admissionDate: string;
    salary: number;
  };
}

export type EmployeeDataPayload = ListEmployeesDto;
