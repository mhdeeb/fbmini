import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Component, Inject, inject } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import {
  MAT_DIALOG_DATA,
  MatDialog,
  MatDialogRef,
} from '@angular/material/dialog';
import { MatDividerModule } from '@angular/material/divider';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatSnackBar } from '@angular/material/snack-bar';
import { PostView } from '../../utility/types';
import { PopUp, pop_up } from '../../utility/popup';
import { PostComponent } from '../../components/post/post.component';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

@Component({
  selector: 'app-comment-dialog',
  templateUrl: './comment-dialog.component.html',
  styleUrl: './comment-dialog.component.css',
  standalone: true,
  imports: [
    MatButtonModule,
    MatInputModule,
    MatCardModule,
    MatDividerModule,
    CommonModule,
    PostComponent,
    MatProgressSpinnerModule,
    MatIconModule,
  ],
})
export class CommentDialogComponent {
  constructor(
    private readonly http: HttpClient,
    public dialog: MatDialog,
    @Inject(MAT_DIALOG_DATA) public data: number,
    private readonly dialogRef: MatDialogRef<CommentDialogComponent>
  ) {}

  public posts: PostView[] = [];
  public isLoaded = false;

  snackbar = inject(MatSnackBar);

  getPosts() {
    this.http.get<PostView[]>(`api/post/list/${this.data}`).subscribe({
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

  onClose() {
    this.dialogRef.close(false);
  }
}
