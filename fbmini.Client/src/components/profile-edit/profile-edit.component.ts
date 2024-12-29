import { HttpClient } from '@angular/common/http';
import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatSnackBar } from '@angular/material/snack-bar';
import { pop_up, PopUp } from '../../utility/popup';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatCardModule } from '@angular/material/card';
import { UserView, User } from '../../utility/types';
import {
  FormBuilder,
  FormControl,
  FormGroup,
  ReactiveFormsModule,
} from '@angular/forms';
import {
  MatDialogActions,
  MatDialogClose,
  MatDialogContent,
  MatDialogRef,
  MatDialogTitle,
  MatDialog,
} from '@angular/material/dialog';
import { MatDividerModule } from '@angular/material/divider';
import { BackdropDialogComponent } from '../backdrop/backdrop.component';

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
  templateUrl: './profile-edit.component.html',
  styleUrls: ['./profile-edit.component.css'],
})
export class ProfileEditDialog {
  public form: FormGroup;
  public previews: {
    picture: string | ArrayBuffer | null;
    cover: string | ArrayBuffer | null;
  } = {
      picture: null,
      cover: null
    };

  constructor(
    private readonly http: HttpClient,
    private readonly fb: FormBuilder,
    private readonly dialogRef: MatDialogRef<ProfileEditDialog>,
    private readonly snackbar: MatSnackBar,
    public dialog: MatDialog
  ) {
    this.form = this.fb.group(new UserView());
  }

  getProfile() {
    this.http.get<User>('api/user/profile').subscribe({
      next: (result) => {
        this.form.patchValue({
          email: result.email,
          bio: result.bio,
          phoneNumber: result.phoneNumber,
        });

        this.previews.picture = result.pictureUrl ?? null;
        this.previews.cover = result.coverUrl ?? null;

        this.form.markAsPristine();
      },
      error: (error) => {
        pop_up(this.snackbar, error.error.message, PopUp.ERROR);
      },
    });
  }

  ngOnInit() {
    this.getProfile();
  }

  onFileSelected(event: any, id: string): void {
    const file = event.target.files[0];
    if (file) {
      const reader = new FileReader();
      reader.onload = () => {
        if (id === 'picture') {
          this.previews.picture = reader.result;
          this.form.patchValue({ picture: file });
          this.form.controls['picture'].markAsDirty();
        } else if (id === 'cover') {
          this.previews.cover = reader.result;
          this.form.patchValue({ cover: file });
          this.form.controls['cover'].markAsDirty();
        }
      };
      reader.readAsDataURL(file);
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

      console.log(formData);

      this.http.post('api/user/profile', formData).subscribe({
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
