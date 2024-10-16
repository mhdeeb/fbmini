import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  constructor(private http: HttpClient) {}

  login(email: string, password: string): Observable<any> {
    return this.http.post(
      `/api/account/login?useCookies=true&useSessionCookies=true`,
      { email, password },
      { withCredentials: true }
    );
  }

  logout(): Observable<any> {
    return this.http.post(`/api/account/logout`, {}, { withCredentials: true });
  }

  isAuthenticated() {
    return this.http.get<boolean>(`/api/account/check`, {
      withCredentials: true,
    });
  }
}
