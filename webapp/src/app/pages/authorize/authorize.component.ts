import { Component, OnInit } from '@angular/core';
import { AuthService } from '../../auth/auth.service';
import { ActivatedRoute, Router } from '@angular/router';
import { Observable } from 'rxjs/Observable';

@Component({
  selector: 'app-authorize',
  template: ''
})
export class AuthorizeComponent implements OnInit {

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private auth: AuthService) { }

  ngOnInit() {
    this.route.queryParams.subscribe(params => {
      if (params['code'] && params['state']) {
        this.auth.getTokenByCode(params['code'], params['state']).subscribe(data => {
          if (data.error) {
            this.auth.redirectToLogin();
            return;
          }
          this.auth.storeToken(data.token_type + ' ' + data.access_token);
          this.router.navigateByUrl('/');
        });
      }
    });
  }

}
