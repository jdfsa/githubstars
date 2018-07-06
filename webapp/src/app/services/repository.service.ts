import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { HttpClient } from '@angular/common/http';

@Injectable()
export class RepositoryService {

  constructor(private http: HttpClient) { }

  public getRepositories(search: string) {
    return this.http.get<any>(environment.endpoints.repository + `?search=${search}`);
  }

}