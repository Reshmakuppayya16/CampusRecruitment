import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AuthService } from './auth.service';

@Injectable({
  providedIn: 'root',
})
export class StudentService {

  private apiUrl = 'https://localhost:7248/api/Student';

  constructor(private http: HttpClient, private authService: AuthService){}

  //Extract logged in userId
  private getUserId(): number {
    const user = this.authService.getUserData();
    if(!user || !user.userId){
      throw new Error("Could not find userId in localStorage");
    }
    return user.userId;
  }

  //GET: Get personal info of a student
  getProfile(): Observable<any>{
    const userId = this.getUserId();
    return this.http.get(`${this.apiUrl}/get-profile/${userId}`);
  }

  //Post: create new personal information
  savePersonalInfo(data: any): Observable<any>{
    const userId = this.getUserId();
    return this.http.post(`${this.apiUrl}/save-personal/${userId}`, data, {
      responseType: 'text',
    });
  }

  //Save education
  saveEducationInfo(data: any): Observable<any>{
    const userId = this.getUserId();
    return this.http.post(`${this.apiUrl}/save-education/${userId}`,data,{
      responseType: 'text'
    });
  }

  //save address
  saveAddressInfo(data: any): Observable<any>{
    const userId = this.getUserId();
    return this.http.post(`${this.apiUrl}/save-address/${userId}`,data, {
      responseType: 'text'
    });
  }
  
}
