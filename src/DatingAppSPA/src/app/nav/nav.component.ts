import { Component, OnInit } from '@angular/core';
import { AuthenticationService } from '../_services/authentication.service';
import { AlertifyService } from '../_services/alertify.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {

  model: any = {};
  photoUrl: string;

  constructor(public authenticationService: AuthenticationService, private alertifyService: AlertifyService, private router: Router) { }

  ngOnInit() {
    this.authenticationService.currentPhotoUrl.subscribe(photoUrl => this.photoUrl = photoUrl);
  }

  login() {
    this.authenticationService.login(this.model).subscribe(next => {
      this.alertifyService.success('Logged in successfully');
    }, error => {
      this.alertifyService.error(error);
    }, () => {
      this.router.navigate(['/members']);
    });
  }

  loggedIn() {
    return this.authenticationService.loggedIn();
  }

  logout() {
    localStorage.removeItem('token');
    localStorage.removeItem('user');
    this.authenticationService.decodedToken = null;
    this.authenticationService.currentUser = null;
    this.alertifyService.message('Logged out');
    this.router.navigate(['/home']);
  }

}
