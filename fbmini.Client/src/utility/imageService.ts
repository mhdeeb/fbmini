import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';

@Injectable({
  providedIn: 'root',
})
export class ImageService {
  constructor(private readonly http: HttpClient) {}

  getImage(url: string): Observable<Blob | null> {
    return this.http.get(url, { responseType: 'blob' }).pipe(
      catchError((error) => {
        if (error.status === 404) {
          console.error('Image not found, loading fallback image');
          return this.getFallbackImage();
        } else {
          console.error('Error fetching image:', error);
          return of(null);
        }
      })
    );
  }

  private getFallbackImage(): Observable<Blob> {
    const fallbackUrl = 'blank-profile-picture.webp';
    return this.http.get(fallbackUrl, { responseType: 'blob' });
  }
}
