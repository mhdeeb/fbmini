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
import { ImageService } from '../../utility/imageService';
import { DomSanitizer, SafeUrl } from '@angular/platform-browser';
import { CommonModule } from '@angular/common';
import { MatDialog } from '@angular/material/dialog';
import { ProfileEditDialog } from './edit/edit.component';

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
  templateUrl: './profile.component.html',
  styleUrl: './profile.component.css',
})
export class ProfileComponent {
  public profile = <User>{};
  isOwner: boolean = false;
  userName: string | null = '';
  Urls: any = {};

  constructor(
    private readonly imageService: ImageService,
    private readonly http: HttpClient,
    public readonly router: Router,
    private readonly route: ActivatedRoute,
    private readonly sanitizer: DomSanitizer,
    public dialog: MatDialog,
    public snackbar: MatSnackBar
  ) {}

  getProfile() {
    let api: string;
    if (this.isOwner) api = `api/user`;
    else api = `api/user/${this.userName}`;
    this.http.get<User>(api).subscribe({
      next: (result) => {
        this.profile = result;
      },
      error: (error) => {
        pop_up(this.snackbar, error.error.message, PopUp.ERROR);
      },
    });
  }

  loadImage(name: string): void {
    let api: string;
    if (this.isOwner) api = `api/user/${name}`;
    else api = `api/user/${this.userName}/${name}`;
    this.imageService.getImage(api).subscribe({
      next: (blob) => {
        if (blob) {
          this.Urls[name] = this.sanitizer.bypassSecurityTrustUrl(
            URL.createObjectURL(blob)
          );
        } else {
          console.error('No image data available');
        }
      },
      error: (error) => {
        console.error('Error in component:', error);
      },
    });
  }

  ngOnInit() {
    this.route.data.subscribe((data) => {
      this.isOwner = data['owner'];
    });

    this.route.params.subscribe((params) => {
      if (params['username']) {
        this.userName = params['username'];
      }
    });

    if (!this.isOwner)
      this.route.paramMap.subscribe((params) => {
        this.userName = params.get('username');
        this.getProfile();
        this.loadImage('cover');
        this.loadImage('picture');
        window.scrollTo({ top: 0, behavior: 'smooth' });
      });

    this.getProfile();
    this.loadImage('picture');
    this.loadImage('cover');
  }

  editProfile() {
    this.dialog
      .open(ProfileEditDialog, { disableClose: true })
      .afterClosed()
      .subscribe((updated: boolean) => {
        if (updated) {
          this.getProfile();
          this.loadImage('picture');
          this.loadImage('cover');
        }
      });
  }
}
