import { Component, Input } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatDividerModule } from '@angular/material/divider';
import { CommonModule } from '@angular/common';
import { PostView, VoteView } from '../../utility/types';
import { HttpClient } from '@angular/common/http';

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
  @Input()
  post!: PostView;

  constructor(private readonly http: HttpClient) {}

  votePost(value: number) {
    this.http.post(`api/user/vote/${value}/${this.post.id}`, {}).subscribe({
      next: (vote) => {
        this.http.get<VoteView>(`api/user/vote/${this.post.id}`).subscribe({
          next: (vote) => {
            this.post.likes = vote.likes;
            this.post.dislikes = vote.dislikes;
          },
          error: (error) => {
            console.error(error);
          },
        });
      },
      error: (error) => {
        console.error(error);
      },
    });
  }

  sharePost() {
    console.log('Post shared!');
  }

  commentPost() {
    console.log('Comment button clicked!');
  }
}
