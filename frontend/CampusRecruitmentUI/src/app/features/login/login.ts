import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { tap } from 'rxjs';

@Component({
  selector: 'app-login',
  standalone:true,
  imports: [CommonModule, FormsModule, MatFormFieldModule, MatButtonModule, MatInputModule, RouterLink],
  templateUrl: './login.html',
  styleUrl: './login.css',
})
export class LoginComponent {
  Email = "";
  PasswordHash = "";

  constructor(private auth: AuthService, private router: Router) {}

  onLogin(){
    if(!this.Email || !this.PasswordHash){
      alert("⚠️ Please fill all fields");
      return;
    }
    const body = {
      Email: this.Email,
      PasswordHash: this.PasswordHash
    };

    this.auth.login(body).pipe(
      tap((res: any) => {
        if (res && res.user) {
          // Save the user immediately after login
          this.auth.saveUserData(res.user);
       }
      })
    ).subscribe({
      next: (res) => {
        alert("🎉 Login Successful!");
        this.router.navigate(['/dashboard']); 
      },
      error: () => {
        alert("❌ Invalid username or password");
      }
    });
  }
}
