import { AuthenticationService } from '../_services/authentication.service';
import { Injectable } from '@angular/core';
import { CanActivate, Router } from '@angular/router';
import { AlertifyService } from '../_services/alertify.service';

@Injectable({
  providedIn: 'root'
})
export class AuthenticationGuard implements CanActivate {

  constructor(private authenticationService: AuthenticationService,
              private router: Router,
              private alertifyService: AlertifyService) { }

  canActivate(): boolean {
    if (this.authenticationService.loggedIn()) {
      return true;
    }
    this.alertifyService.error('You shall not pass!');
    this.router.navigate(['/home']);
    return false;
  }
}
