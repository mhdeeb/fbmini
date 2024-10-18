import { HttpClient } from '@angular/common/http';
import { Component, inject } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { pop_up, PopUp } from '../../utility/popup';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatCardModule } from '@angular/material/card';

type User = {
  userName: string;
  email: string;
};

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [MatButtonModule, MatIconModule, MatInputModule, MatCardModule],
  templateUrl: './profile.component.html',
  styleUrl: './profile.component.css',
})
export class ProfileComponent {
  public profile: User = {
    userName: '',
    email: '',
  };

  snackbar = inject(MatSnackBar);

  constructor(private readonly http: HttpClient) {}

  getProfile() {
    this.http.get<User>('api/User').subscribe({
      next: (result) => {
        this.profile = result;
      },
      error: (error) => {
        pop_up(this.snackbar, error.error.message, PopUp.ERROR);
      },
    });
  }

  ngOnInit() {
    this.getProfile();
  }
}
