import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  constructor(private readonly http: HttpClient) {}

  login(
    username: string,
    password: string,
    rememberMe: boolean
  ): Observable<any> {
    return this.http.post(
      `/api/account/login?useCookies=true&useSessionCookies=true`,
      { username, password, rememberMe },
      { withCredentials: true }
    );
  }

  register(username: string, password: string) {
    const url = '/api/account/register';
    const body = {
      username,
      password,
    };
    const headers = new HttpHeaders({
      'Content-Type': 'application/json',
    });
    return this.http.post(url, body, { headers });
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
