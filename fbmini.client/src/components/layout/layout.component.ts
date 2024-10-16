import { HttpClient } from '@angular/common/http';
import { Component } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { RouterOutlet } from '@angular/router';
import { AuthService } from '../../app/auth.component';
import { CommonModule } from '@angular/common';

type User = {
  userName: string;
  email: string;
};

@Component({
  selector: 'app-layout',
  standalone: true,
  imports: [RouterOutlet, MatButtonModule, CommonModule],
  templateUrl: './layout.component.html',
  styleUrl: './layout.component.css',
})
export class LayoutComponent {
  constructor(private readonly http: HttpClient, private readonly authService: AuthService) {}

  public profile: User = {
    userName: '',
    email: '',
  };

  listProfile() {
    this.http.get<User>('api/User').subscribe({
      next: (result) => {
        console.log(result);
        this.profile = result;
      },
      error: (error) => {
        console.error(error);
      },
    });
  }

  ngOnInit() {
    this.listProfile();
  }

  logout(): void {
    this.authService.logout().subscribe({
      error: (error) => {
        console.log(error);
      },
      complete: () => {
        location.reload();
      },
    });
  }
}
