import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';

@Injectable()
export class UserService {

  constructor(private http: HttpClient) { }

  public getUserData() {
    return this.http.get<any>(environment.endpoints.user);
  }
}
