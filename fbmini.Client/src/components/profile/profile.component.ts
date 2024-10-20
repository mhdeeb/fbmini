import { HttpClient } from '@angular/common/http';
import { Component, inject } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { pop_up, PopUp } from '../../utility/popup';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatCardModule } from '@angular/material/card';
import { User } from '../../utility/types';
import { Router } from '@angular/router';
import { ImageService } from '../../utility/imageService';
import { DomSanitizer, SafeUrl } from '@angular/platform-browser';
import { CommonModule } from '@angular/common';

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
  profileUrl: SafeUrl | null = null;
  coverUrl: SafeUrl | null = null;

  snackbar = inject(MatSnackBar);

  constructor(
    private imageService: ImageService,
    private readonly http: HttpClient,
    public readonly router: Router,
    private sanitizer: DomSanitizer
  ) {}

  getProfile() {
    this.http.get<User>('api/User').subscribe({
      next: (result) => {
        this.profile = result;
      },
      error: (error) => {
        pop_up(this.snackbar, error.error.message, PopUp.ERROR);
      },
    });
  }

  loadProfileImage(): void {
    this.imageService.getImage('api/user/picture').subscribe({
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
    this.imageService.getImage('api/user/cover').subscribe({
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
    this.getProfile();
    this.loadCoverImage();
    this.loadProfileImage();
  }
}
