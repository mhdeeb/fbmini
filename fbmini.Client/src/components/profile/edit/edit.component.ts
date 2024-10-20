import { HttpClient } from '@angular/common/http';
import { Component, inject } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { pop_up, PopUp } from '../../../utility/popup';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatCardModule } from '@angular/material/card';
import { UserView, User } from '../../../utility/types';
import { FormBuilder, FormGroup, ReactiveFormsModule } from '@angular/forms';
// import { AuthService } from '../../auth/auth.component';
// import { Router } from '@angular/router';
import { MatDialog } from '@angular/material/dialog';
import { BackdropDialogComponent } from '../../backdrop/backdrop.component';

@Component({
  selector: 'app-profile-edit',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    MatButtonModule,
    MatIconModule,
    MatInputModule,
    MatCardModule,
  ],
  templateUrl: './edit.component.html',
  styleUrl: './edit.component.css',
})
export class ProfileEditComponent {
  form: FormGroup;
  snackbar = inject(MatSnackBar);
  selectedPicture: File | null = null;
  selectedCover: File | null = null;

  constructor(
    private readonly http: HttpClient,
    private readonly fb: FormBuilder,
    // private readonly authService: AuthService,
    // private readonly router: Router,
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

  ngOnInit() {
    this.getProfile();
  }

  onPictureSelect(event: any): void {
    this.selectedPicture = event.target.files[0];
  }

  onCoverSelect(event: any): void {
    this.selectedCover = event.target.files[0];
  }

  onSubmit() {
    const dialogRef = this.dialog.open(BackdropDialogComponent, {
      disableClose: true,
    });
    let formData = new FormData();

    for (const value in this.form.value)
      if (this.form.get(value)?.dirty)
        formData.append(value, this.form.get(value)?.value);

    if (this.selectedPicture) formData.append('picture', this.selectedPicture);

    if (this.selectedCover) formData.append('cover', this.selectedCover);

    this.http.post('api/User', formData).subscribe({
      next: (result: any) => {
        dialogRef.close();
        pop_up(this.snackbar, result.message, PopUp.SUCCESS);
      },
      error: (error) => {
        dialogRef.close();
        pop_up(this.snackbar, error.error.message, PopUp.ERROR);
      },
    });
  }
}
