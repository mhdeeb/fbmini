import { HttpClient } from '@angular/common/http';
import { Component, inject } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { RouterOutlet } from '@angular/router';
import { CommonModule } from '@angular/common';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';

import { PostView } from '../../utility/types';
import { PostComponent } from '../post/post.component';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [
    RouterOutlet,
    MatButtonModule,
    CommonModule,
    PostComponent,
    MatProgressSpinnerModule,
  ],
  templateUrl: './home.component.html',
  styleUrl: './home.component.css',
})
export class HomeComponent {
  constructor(private readonly http: HttpClient, public dialog: MatDialog) {}

  public posts: PostView[] = [];
  public isLoaded = false;

  snackbar = inject(MatSnackBar);

  getPosts() {
    this.http.get<PostView[]>('api/user/list').subscribe({
      next: (posts) => {
        this.posts = posts;
        this.isLoaded = true;
      },
      error: (error) => {
        console.error(error);
      },
    });
  }

  ngOnInit() {
    this.getPosts();
  }
}
