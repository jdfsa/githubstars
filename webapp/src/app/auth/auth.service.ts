import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';

@Injectable()
export class AuthService {

  constructor(private http: HttpClient) { }

  get TOKEN_STORAGE_NAME(): string {
    return 'githubstars';
  }

  public getToken(): string {
    return localStorage.getItem(this.TOKEN_STORAGE_NAME) || '';
  }

  public storeToken(token: string) {
    localStorage.setItem(this.TOKEN_STORAGE_NAME, token);
  }

  public isAuthenticated(): boolean {
    return !!this.getToken();
  }

  public redirectToLogin() {
    return this.http.get<string>(environment.endpoints.authorize + `?urlBack=${environment.appData.redirectUri}`)
      .subscribe(data => {
          window.location.href = data;
        });
    }

  public getTokenByCode(code: string, state: string) {
    return this.http.get<any>(environment.endpoints.token + `?githubCode=${code}&state=${state}`);
  }

}
