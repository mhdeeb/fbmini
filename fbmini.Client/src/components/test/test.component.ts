import { Component, inject } from '@angular/core';
import { pop_up, PopUp } from '../../utility/popup';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatButtonModule } from '@angular/material/button';

@Component({
  templateUrl: './test.component.html',
  styleUrl: './test.component.css',
  imports: [MatButtonModule],
  standalone: true,
})
export class TestComponent {
  constructor() {}
  snackbar = inject(MatSnackBar);

  success() {
    pop_up(this.snackbar, 'Long Success Message!', PopUp.SUCCESS);
  }
  info() {
    pop_up(this.snackbar, 'Long Information Message', PopUp.INFO);
  }
  error() {
    pop_up(this.snackbar, 'Long Error Message!', PopUp.ERROR);
  }
}
