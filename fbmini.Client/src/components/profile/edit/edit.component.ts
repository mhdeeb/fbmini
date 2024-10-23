import { HttpClient } from '@angular/common/http';
import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatSnackBar } from '@angular/material/snack-bar';
import { pop_up, PopUp } from '../../../utility/popup';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatCardModule } from '@angular/material/card';
import { UserView, User } from '../../../utility/types';
import { FormBuilder, FormGroup, ReactiveFormsModule } from '@angular/forms';
import {
  MatDialogActions,
  MatDialogClose,
  MatDialogContent,
  MatDialogRef,
  MatDialogTitle,
  MatDialog,
} from '@angular/material/dialog';
import { MatDividerModule } from '@angular/material/divider';
import { BackdropDialogComponent } from '../../backdrop/backdrop.component';
import { ImageService } from '../../../utility/imageService';
import { DomSanitizer, SafeUrl } from '@angular/platform-browser';

@Component({
  selector: 'app-profile-edit',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    MatButtonModule,
    MatIconModule,
    MatInputModule,
    MatCardModule,
    MatDialogTitle,
    MatDialogContent,
    MatDialogActions,
    MatDialogClose,
    MatDividerModule,
    CommonModule,
  ],
  templateUrl: './edit.component.html',
  styleUrl: './edit.component.css',
})
export class ProfileEditDialog {
  form: FormGroup;
  picturePreview: string | ArrayBuffer | null | SafeUrl = null;
  coverPreview: string | ArrayBuffer | null | SafeUrl = null;

  constructor(
    private readonly http: HttpClient,
    private readonly fb: FormBuilder,
    private readonly imageService: ImageService,
    private readonly sanitizer: DomSanitizer,
    private readonly dialogRef: MatDialogRef<ProfileEditDialog>,
    private readonly snackbar: MatSnackBar,
    public dialog: MatDialog
  ) {
    this.form = this.fb.group(new UserView());
  }

  getProfile() {
    this.http.get<User>('api/User').subscribe({
      next: (result) => {
        this.form.setValue(result);
        this.form.markAsPristine();
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
          this.picturePreview =
            this.sanitizer.bypassSecurityTrustUrl(objectURL);
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
          this.coverPreview = this.sanitizer.bypassSecurityTrustUrl(objectURL);
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

  onPictureSelect(event: any): void {
    const file = event.target.files[0];
    if (file) {
      this.form.patchValue({ Attachment: file });

      const reader = new FileReader();
      reader.onload = () => {
        this.picturePreview = reader.result;
      };
      reader.readAsDataURL(file);
    } else {
      this.form.patchValue({ Attachment: null });
      this.picturePreview = null;
    }
  }

  onCoverSelect(event: any): void {
    const file = event.target.files[0];
    if (file) {
      this.form.patchValue({ Attachment: file });

      const reader = new FileReader();
      reader.onload = () => {
        this.coverPreview = reader.result;
      };
      reader.readAsDataURL(file);
    } else {
      this.form.patchValue({ Attachment: null });
      this.coverPreview = null;
    }
  }

  onSubmit() {
    if (this.form.valid) {
      const dialogRef = this.dialog.open(BackdropDialogComponent, {
        disableClose: true,
      });
      let formData = new FormData();

      for (const value in this.form.value)
        if (this.form.get(value)?.dirty)
          formData.append(value, this.form.get(value)?.value);

      this.http.post('api/User', formData).subscribe({
        next: (result: any) => {
          dialogRef.close();
          this.dialogRef.close(true);
          pop_up(this.snackbar, result.message, PopUp.SUCCESS);
        },
        error: (error) => {
          dialogRef.close();
          pop_up(this.snackbar, error.error.message, PopUp.ERROR);
        },
      });
    }
  }

  onClose() {
    this.dialogRef.close(false);
  }
}
