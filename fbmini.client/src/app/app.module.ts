import { NgModule } from '@angular/core';

import { AppComponent } from './app.component';
import { BrowserModule } from '@angular/platform-browser';
import { RouterModule } from '@angular/router';


import { provideRouter } from '@angular/router';
import { provideAnimations } from '@angular/platform-browser/animations';

import { routes } from './app-routes';
import { provideHttpClient } from '@angular/common/http';

@NgModule({
  declarations: [
    AppComponent
  ],
  imports: [
    BrowserModule, RouterModule
  ],
  providers: [
    provideRouter(routes),
    provideAnimations(),
    provideHttpClient(),
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
