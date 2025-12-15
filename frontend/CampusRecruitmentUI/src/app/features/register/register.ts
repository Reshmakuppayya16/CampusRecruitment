import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';

// Angular Material Modules
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatSelectModule } from '@angular/material/select';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatSelectModule
  ],
  templateUrl: './register.html',
  styleUrls: ['./register.css']
})
export class RegisterComponent {
  FullName = "";
  Email = "";
  PasswordHash = "";
  ConfirmPassword = "";
  Role = "";

  constructor(private auth: AuthService, private router: Router) {}

  register() {
    if (!this.FullName || !this.Email || !this.PasswordHash || !this.ConfirmPassword || !this.Role) {
      alert("⚠️ Please fill in all fields");
      return;
    }

    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    if (!emailRegex.test(this.Email)) {
      alert("⚠️ Enter a valid email address");
      return;
    }

    if (this.PasswordHash !== this.ConfirmPassword) {
      alert("⚠️ Password & Confirm Password do not match!");
      return;
    }

    const body = {
      FullName: this.FullName,
      Email: this.Email,
      PasswordHash: this.PasswordHash,
      Role: this.Role
    };

    this.auth.register(body).subscribe({
      next: (res) => {
        alert("🎉 Registration Successful!");
        this.router.navigate(['/login']);
      },
      error: (err) => {
        alert(err.error?.message || "Registration failed. Try again.");
      }
    });
  }
}
