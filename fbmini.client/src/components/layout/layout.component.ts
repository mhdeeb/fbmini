import { HttpClient } from '@angular/common/http';
import { Component } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { RouterOutlet } from '@angular/router';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-layout',
  standalone: true,
  imports: [RouterOutlet, MatButtonModule],
  templateUrl: './layout.component.html',
  styleUrl: './layout.component.css',
})
export class LayoutComponent {
  constructor(private http: HttpClient) {}
  logout(): void {
    this.http
      .post(`/api/account/logout`, {}, { withCredentials: true })
      .subscribe({
        next: (event) => {
          console.log(event);
          localStorage.setItem('loggedIn', '0');
        },
        error: (error) => {
          console.log(error);
        },
        complete: () => {
          location.reload();
        },
      });
  }
}
