import { MatSnackBar } from '@angular/material/snack-bar';

export enum PopUp {
  ERROR = 'snack-bar--error',
  SUCCESS = 'snack-bar--success',
  INFO = 'snack-bar--info',
}

export function pop_up(
  snackbar: MatSnackBar,
  message: string,
  popup_style: PopUp
) {
  snackbar.open(message, '', {
    horizontalPosition: 'center',
    verticalPosition: 'top',
    duration: 2000,
    panelClass: popup_style,
  });
}
