import { HttpClient } from '@angular/common/http';
import { Component, inject } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { RouterOutlet } from '@angular/router';
import { AuthService } from '../auth/auth.component';
import { CommonModule } from '@angular/common';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';

import { User } from '../../utility/types';
import { PostComponent } from '../post/post.component';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [RouterOutlet, MatButtonModule, CommonModule, PostComponent],
  templateUrl: './home.component.html',
  styleUrl: './home.component.css',
})
export class HomeComponent {
  constructor(
    private readonly http: HttpClient,
    private readonly authService: AuthService,
    public dialog: MatDialog
  ) {}

  // public posts;

  snackbar = inject(MatSnackBar);

  getPosts() {
    this.http.get<User>('api/user/post').subscribe({
      next: (posts) => {
        // this.posts = posts;
      },
      error: (error) => {
        console.error(error);
      },
    });
  }

  getRange(start: number, end: number): number[] {
    return Array.from({ length: end - start + 1 }, (_, i) => start + i);
  }

  ngOnInit() {
    this.getPosts();
  }
}
