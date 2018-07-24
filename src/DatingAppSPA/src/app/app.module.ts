import { HttpModule } from '@angular/http';
import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';

import { AppComponent } from './app.component';
import { NavComponent } from './nav/nav.component';
import { AuthenticationService } from './_services/authentication.service';
import { HttpClientModule } from '../../node_modules/@angular/common/http';
import { HomeComponent } from './home/home.component';
import { RegisterComponent } from './register/register.component';

@NgModule({
   declarations: [
      AppComponent,
      NavComponent,
      HomeComponent,
      RegisterComponent
   ],
   imports: [
      BrowserModule,
      HttpModule,
      FormsModule,
      HttpClientModule
   ],
   providers: [
      AuthenticationService
   ],
   bootstrap: [
      AppComponent
   ]
})
export class AppModule { }
