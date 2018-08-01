import { AlertifyService } from './../../_services/alertify.service';
import { UserService } from './../../_services/user.service';
import { AuthenticationService } from './../../_services/authentication.service';
import { Component, OnInit, Input } from '@angular/core';
import { User } from '../../_models/user';

@Component({
  selector: 'app-member-card',
  templateUrl: './member-card.component.html',
  styleUrls: ['./member-card.component.css']
})
export class MemberCardComponent implements OnInit {

  @Input() user: User;

  constructor(private authenticationService: AuthenticationService,
     private userService: UserService, private alertifyService: AlertifyService) { }

  ngOnInit() {
  }

  sendLike(id: number) {
    this.userService.sendLike(this.authenticationService.decodedToken.nameid, id).subscribe(data => {
      this.alertifyService.success('You have liked: ' + this.user.knownAs);
    }, error => {
      this.alertifyService.error(error);
    });
  }

}
