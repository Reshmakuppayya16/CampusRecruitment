import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';

@Injectable({
providedIn: 'root',
})
export class AuthService {

  private apiUrl = 'https://localhost:7248/api/auth';

  constructor(private http: HttpClient) {}

  register(data: {
    FullName: string;
    Email: string;
    PasswordHash: string;
    Role: string;
  }): Observable<any> {
    return this.http.post(`${this.apiUrl}/register`, data);
  }

  login(credentials: { Email: string; PasswordHash: string }): Observable<any> {
    return this.http.post(`${this.apiUrl}/login`, credentials).pipe(
    tap((res: any) => {
    if (res && res.user) {
      this.saveUserData(res.user);  // ✅ Save user fullname, email, role
    }
  })
  );
}

saveUserData(user: any) {
  localStorage.setItem('user', JSON.stringify(user));
}

getUserData() {
  const user = localStorage.getItem('user');
  return user ? JSON.parse(user) : null;
}

logout() {
  localStorage.removeItem('user');
  }
}
