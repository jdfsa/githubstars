import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { AppDataModel } from '../model/app-data.model';

@Injectable()
export class AuthService {

  constructor(private http: HttpClient) { }

  get TOKEN_STORAGE_NAME(): string {
    return 'githubstars';
  }

  private getAppData(): AppDataModel {
    const item = localStorage.getItem(this.TOKEN_STORAGE_NAME);
    if (!item) {
      return new AppDataModel('', '');
    }

    const obj = JSON.parse(item);
    return new AppDataModel(obj.token || '', obj.userId || '');
  }

  private storeAppData(data: AppDataModel) {
    localStorage.setItem(this.TOKEN_STORAGE_NAME, JSON.stringify(data));
  }

  public getToken(): string {
    return this.getAppData().token || '';
  }

  public storeToken(token: string) {
    const data = this.getAppData();
    data.token = token;
    this.storeAppData(data);
  }

  public getUserId(): string {
    return this.getAppData().userId || '';
  }

  public storeUserId(userId: string) {
    const data = this.getAppData();
    data.userId = userId;
    this.storeAppData(data);
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
