import { HttpClient } from '@angular/common/http';
import { Component, inject, Input } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { CommonModule } from '@angular/common';
import { PostComponent } from '../../components/post/post.component';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { PostView } from '../../utility/types';
import { PopUp, pop_up } from '../../utility/popup';

@Component({
  selector: 'app-feed',
  standalone: true,
  imports: [MatButtonModule, CommonModule, PostComponent],
  templateUrl: './feed.component.html',
  styleUrl: './feed.component.css',
})
export class FeedComponent {
  constructor(private readonly http: HttpClient, public dialog: MatDialog) {}

  public posts: PostView[] = [];
  public isLoaded = false;
  @Input() query: string = '';

  snackbar = inject(MatSnackBar);

  getPosts() {
    this.http.get<PostView[]>(`api/post/${this.query}`).subscribe({
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
