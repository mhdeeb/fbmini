import { HttpClient } from '@angular/common/http';
import { Component, inject } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { pop_up, PopUp } from '../../../utility/popup';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatCardModule } from '@angular/material/card';
import { User } from '../../../utility/types';
import { ActivatedRoute } from '@angular/router';
import { ImageService } from '../../../utility/imageService';
import { DomSanitizer, SafeUrl } from '@angular/platform-browser';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-user',
  standalone: true,
  imports: [
    MatButtonModule,
    MatIconModule,
    MatInputModule,
    MatCardModule,
    CommonModule,
  ],
  templateUrl: './user.component.html',
  styleUrl: './user.component.css',
})
export class UserComponent {
  public profile = <User>{};
  profileUrl: SafeUrl | null = null;
  coverUrl: SafeUrl | null = null;
  username: string | null = null;

  snackbar = inject(MatSnackBar);

  constructor(
    private imageService: ImageService,
    private readonly http: HttpClient,
    private route: ActivatedRoute,
    private sanitizer: DomSanitizer
  ) {}

  getProfile() {
    this.http.get<User>(`api/user/${this.username}`).subscribe({
      next: (result) => {
        this.profile = result;
      },
      error: (error) => {
        pop_up(this.snackbar, error.error.message, PopUp.ERROR);
      },
    });
  }

  loadProfileImage(): void {
    this.imageService.getImage(`api/user/${this.username}/picture`).subscribe({
      next: (blob) => {
        if (blob) {
          const objectURL = URL.createObjectURL(blob);
          this.profileUrl = this.sanitizer.bypassSecurityTrustUrl(objectURL);
        } else {
          console.error('No image data available');
        }
      },
      error: (error) => {
        console.error('Error in component:', error);
      },
    });
  }

  loadCoverImage(): void {
    this.imageService.getImage(`api/user/${this.username}/cover`).subscribe({
      next: (blob) => {
        if (blob) {
          const objectURL = URL.createObjectURL(blob);
          this.coverUrl = this.sanitizer.bypassSecurityTrustUrl(objectURL);
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
    this.username = this.route.snapshot.paramMap.get('username');
    this.getProfile();
    this.loadCoverImage();
    this.loadProfileImage();
  }
}
