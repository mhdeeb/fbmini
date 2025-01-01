import { Component } from '@angular/core';
import { FeedComponent } from '../../components/feed/feed.component';
import { ActivatedRoute, Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { pop_up, PopUp } from '../../utility/popup';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatDialog } from '@angular/material/dialog';
import { PostEditDialog } from '../../components/post-edit/post-edit.component';

@Component({
  standalone: true,
  imports: [FeedComponent, MatIconModule, MatButtonModule],
  templateUrl: './feed.page.html',
  styleUrl: './feed.page.css',
})
export class FeedPage {
  query!: string;
  parentPostId?: number;

  fetchParentId(query: string) {
    this.http
      .get<{ parentPostId: number }>(`api/post/parent/${query}`)
      .subscribe({
        next: (result) => {
          this.parentPostId = result.parentPostId;
        },
        error: (error) => {
          pop_up(this.snackbar, error.error.message, PopUp.ERROR);
        },
      });
  }

  constructor(
    private readonly route: ActivatedRoute,
    private readonly http: HttpClient,
    private readonly snackbar: MatSnackBar,
    private readonly router: Router,
    public dialog: MatDialog
  ) {
    route.params.subscribe((val) => {
      this.query = val['query'];
      this.fetchParentId(this.query);
    });
  }

  goToParent() {
    if (this.parentPostId) {
      this.router.navigate(['/feed', this.parentPostId]);
    } else {
      this.router.navigate(['/home']);
    }
  }

  comment() {
    this.dialog.open(PostEditDialog, {
      disableClose: true,
      data: {
        postId: this.query,
      },
    });
  }
}
