import { HttpClient } from '@angular/common/http';
import { Component, inject } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { CommonModule } from '@angular/common';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';

import { PostView } from '../../utility/types';
import { PostComponent } from '../../components/post/post.component';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { PopUp, pop_up } from '../../utility/popup';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [
    MatButtonModule,
    CommonModule,
    PostComponent,
    MatProgressSpinnerModule,
  ],
  templateUrl: './home.page.html',
  styleUrl: './home.page.css',
})
export class HomePage {
  constructor(private readonly http: HttpClient, public dialog: MatDialog) {}

  public posts: PostView[] = [];
  public isLoaded = false;

  snackbar = inject(MatSnackBar);

  getPosts() {
    this.http.get<PostView[]>('api/post/list').subscribe({
      next: (posts) => {
        this.posts = posts;
        this.isLoaded = true;
      },
      error: (error) => {
        pop_up(this.snackbar, error, PopUp.ERROR);
      },
    });
  }

  ngOnInit() {
    this.getPosts();
  }

  removePost(post: PostView) {
    this.posts = this.posts.filter((p) => p !== post);
  }
}
