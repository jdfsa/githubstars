import { Injectable } from '@angular/core';
import { CanActivate } from '@angular/router';
import { AuthService } from './auth.service';

@Injectable()
export class AuthCheckService implements CanActivate {

  constructor(private auth: AuthService) { }

  canActivate(): boolean {
    if (!this.auth.isAuthenticated()) {
      this.auth.redirectToLogin();
      return false;
    }
    return true;
  }

}
