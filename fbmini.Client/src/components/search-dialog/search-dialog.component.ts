import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { FormControl, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { map, Observable, startWith } from 'rxjs';

import { MatAutocompleteModule } from '@angular/material/autocomplete';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { MatDialogRef } from '@angular/material/dialog';

@Component({
  selector: 'app-search-dialog',
  templateUrl: './search-dialog.component.html',
  styleUrl: './search-dialog.component.css',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    CommonModule,
    MatFormFieldModule,
    MatInputModule,
    MatIconModule,
    FormsModule,
    MatButtonModule,
    MatAutocompleteModule,
  ],
})
export class SearchDialog {
  searchQuery = new FormControl('');
  options: string[] = [''];
  filteredOptions!: Observable<string[]>;

  constructor(
    private readonly http: HttpClient,
    private readonly router: Router,
    private readonly dialogRef: MatDialogRef<SearchDialog>
  ) {}

  clearSearch(): void {
    this.searchQuery.reset();
  }

  ngOnInit() {
    this.http.get<string[]>('api/user/list').subscribe({
      next: (response) => {
        this.options = response;
        this.filteredOptions = this.searchQuery.valueChanges.pipe(
          startWith(''),
          map((value) => this._filter(value || ''))
        );
      },
      error: () => {},
    });
  }

  private _filter(value: string): string[] {
    const filterValue = value.toLowerCase();
    return this.options.filter((option) =>
      option.toLowerCase().includes(filterValue)
    );
  }

  onSubmit() {
    this.dialogRef.close();
    if (this.searchQuery.value) {
      this.router.navigate([`user/${this.searchQuery.value}`]);
      this.searchQuery.reset();
    }
  }
}
