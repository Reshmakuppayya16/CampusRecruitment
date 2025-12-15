import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatSelectModule } from '@angular/material/select';
import { AuthService } from '../../services/auth.service';
import { StudentService } from '../../services/student.service';
import { ChangeDetectorRef } from '@angular/core';


@Component({
  selector: 'app-personalinfo',
  standalone: true,
  imports: [CommonModule, FormsModule, MatFormFieldModule, MatInputModule, MatButtonModule, MatSelectModule],
  templateUrl: './personalinfo.html',
  styleUrl: './personalinfo.css',
})
export class PersonalinfoComponent implements OnInit {

  personal: any ={
    userId: 0,
    fullName: '',
    email: '',
    dateOfBirth: '',
    gender: '',
    phoneNumber: ''
  };

  constructor(private studentService: StudentService, private authService: AuthService, private router: Router, private cdr: ChangeDetectorRef){}

  ngOnInit(): void {
    const user = this.authService.getUserData();

    if(!user)
    {
      this.router.navigate( ['/login']);
      return;
    }

    this.personal.userId = user.userId;
    this.personal.fullName = user.fullName;
    this.personal.email = user.email;

    this.studentService.getProfile().subscribe({
      next: (res: any) => {
        console.log('PROFILE RESPONSE:', res);

        if(!res) return;

        //Update properties not object
        this.personal.dateOfBirth = this.formatDate(res.dateOfBirth);
        this.personal.gender = res.gender ?? '';
        this.personal.phoneNumber = res.phoneNumber ?? '';

        this.cdr.detectChanges();

        console.log('UPDATED PERSONAL:', this.personal);
      },
      error: () => console.log('No existing profile found')
    });
  }
 
  

  private formatDate(dateValue: any): string {
  if (!dateValue) return '';

  // Handles ISO like "2025-12-09T00:00:00"
  if (typeof dateValue === 'string' && dateValue.includes('T')) {
    return dateValue.split('T')[0];
  }

  try {
    const date = new Date(dateValue);
    if (isNaN(date.getTime())) return '';
    return date.toISOString().split('T')[0];
  } catch {
    return '';
  }
}


  saveAndNext(){
    this.studentService.savePersonalInfo(this.personal).subscribe({
      next : () => {
        alert("Personal Info Saved!");
        this.router.navigate(['/dashboard/education']);
      },
      error: () => alert ("Error saving profile")
    });
  }
}
