import { HttpClient } from '@angular/common/http';
import { Component } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { pop_up, PopUp } from '../../utility/popup';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatCardModule } from '@angular/material/card';
import { User } from '../../utility/types';
import { Router, ActivatedRoute } from '@angular/router';
import { CommonModule } from '@angular/common';
import { MatDialog } from '@angular/material/dialog';
import { ProfileEditDialog } from '../../components/profile-edit/profile-edit.component';
import { map } from 'rxjs/operators';

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [
    MatButtonModule,
    MatIconModule,
    MatInputModule,
    MatCardModule,
    CommonModule,
  ],
  templateUrl: './profile.page.html',
  styleUrl: './profile.page.css',
})
export class ProfilePage {
  public profile = <User>{};

  constructor(
    private readonly http: HttpClient,
    public readonly router: Router,
    private readonly route: ActivatedRoute,
    public dialog: MatDialog,
    public snackbar: MatSnackBar
  ) {}

  getProfile() {
    this.route.url
      .pipe(
        map(segments => segments.map(seg => seg.path)),
        map(paths => ({
          first: paths[0] || null,
          second: paths[1] || null
        }))
      )
      .subscribe(({ first, second }) => {
        this.http.get<User>(`api/user/profile/${first == 'user' && second ? second : ''}`).subscribe({
          next: (result) => {
            this.profile = result;
          },
          error: (error) => {
            pop_up(this.snackbar, error.error.message, PopUp.ERROR);
          },
        });
      });
  }

  ngOnInit() {
    this.getProfile();
  }

  editProfile() {
    this.dialog
      .open(ProfileEditDialog, { disableClose: true, data: this.profile })
      .afterClosed()
      .subscribe((updated: boolean) => {
        if (updated) {
          this.getProfile();
        }
      });
  }
}
