import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { User } from '../_models/user';

// ovo nam više nije potrebno jer smo u app.module.ts konfigurisali JwtModule.forRoot(...) da automatski šalje autorizacijski token
// u get bi slali dodatne hedere return this.http.get<User>(this.baseUrl + id, httpOptions);
// const httpOptions = {
//   headers: new HttpHeaders({
//     Authorization: 'Bearer ' + localStorage.getItem('token')
//   })
// };

@Injectable({
  providedIn: 'root'
})
export class UserService {
  baseUrl = environment.apiUrl + 'user/';
  constructor(private http: HttpClient) { }

  getUser(id: number): Observable<User> {
    return this.http.get<User>(this.baseUrl + id);
  }
  getUsers(): Observable<User[]> {
    return this.http.get<User[]>(this.baseUrl);
  }
  updateUser(id: number, user: User) {
    return this.http.put(this.baseUrl + id, user);
  }
}
