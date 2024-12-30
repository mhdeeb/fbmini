import { Component, EventEmitter, Input, Output, inject } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatDividerModule } from '@angular/material/divider';
import { CommonModule } from '@angular/common';
import { PostView, VoteView } from '../../utility/types';
import { HttpClient } from '@angular/common/http';
import { MatSnackBar } from '@angular/material/snack-bar';
import { pop_up, PopUp } from '../../utility/popup';
import { MatDialog } from '@angular/material/dialog';
import { ConfirmDialogComponent } from '../confirm-dialog/confirm-dialog.component';
import { CommentDialogComponent } from '../comment-dialog/comment-dialog.component';

@Component({
  selector: 'app-post',
  templateUrl: './post.component.html',
  styleUrl: './post.component.css',
  standalone: true,
  imports: [
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatDividerModule,
    CommonModule,
  ],
})
export class PostComponent {
  @Input() post!: PostView;

  imageUrl!: string;

  @Output() remove = new EventEmitter<void>();

  readonly dialog = inject(MatDialog);

  constructor(
    private readonly http: HttpClient,
    private readonly snackbar: MatSnackBar
  ) {}

  ngOnInit() {
    this.imageUrl = this.post.poster.pictureUrl ?? 'blank-profile-picture.webp';

    const date = new Date(this.post.date + 'Z');

    const options: Intl.DateTimeFormatOptions = {
      year: 'numeric',
      month: 'long',
      day: 'numeric',
      hour: '2-digit',
      minute: '2-digit',
      second: '2-digit',
      hour12: true,
    };

    this.post.date = date.toLocaleString(undefined, options);
  }

  votePost(value: number) {
    this.http
      .patch<VoteView>(`api/post/vote/${this.post.id}/${value}`, {})
      .subscribe({
        next: (vote) => {
          this.post.likes = vote.likes;
          this.post.dislikes = vote.dislikes;
          this.post.vote = vote.vote;
        },
        error: (error) => {
          pop_up(this.snackbar, error.error.message, PopUp.ERROR);
        },
      });
  }

  commentPost() {
    // this.dialog.open(CommentDialogComponent, { data: this.post.id });
    pop_up(this.snackbar, 'Comments are not implemented yet', PopUp.WARNING);
  }

  confirmDeletePost() {
    this.dialog
      .open(ConfirmDialogComponent, {})
      .afterClosed()
      .subscribe((close) => {
        if (close)
          this.http.delete(`api/post/delete/${this.post.id}`).subscribe({
            next: () => {
              pop_up(this.snackbar, 'Post Deleted', PopUp.INFO);
              this.remove.emit();
            },
            error: (error) => {
              pop_up(this.snackbar, error.error.message, PopUp.ERROR);
            },
          });
      });
  }
}
