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
  ContentImageUrl?: string;
  posterImageUrl?: string;

  constructor(private readonly http: HttpClient) {}

  b64ToURL(b: string, contentType: string) {
    const byteCharacters = atob(b);
    const byteNumbers = new Array(byteCharacters.length);
    for (let i = 0; i < byteCharacters.length; i++)
      byteNumbers[i] = byteCharacters.charCodeAt(i);

    const byteArray = new Uint8Array(byteNumbers);

    const blob = new Blob([byteArray], {
      type: contentType,
    });
    return URL.createObjectURL(blob);
  }

  ngOnInit() {
    if (this.post.attachment) {
      this.ContentImageUrl = this.b64ToURL(
        this.post.attachment.fileData,
        this.post.attachment.contentType
      );
    }
    if (this.post.poster.picture) {
      this.posterImageUrl = this.b64ToURL(
        this.post.poster.picture.fileData,
        this.post.poster.picture.contentType
      );
    } else {
      this.posterImageUrl = 'blank-profile-picture.webp';
    }

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
      .post<VoteView>(`api/user/vote/${value}/${this.post.id}`, {})
      .subscribe({
        next: (vote) => {
          this.post.likes = vote.likes;
          this.post.dislikes = vote.dislikes;
          this.post.vote = vote.vote;
        },
        error: (error) => {
          console.error(error);
        },
      });
  }

  commentPost() {
    console.log('Comment button clicked!');
  }
}
