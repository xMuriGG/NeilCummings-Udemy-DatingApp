import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { User } from '../_models/user';
import { PaginatedResult } from '../_models/pagination';
import { map } from 'rxjs/operators';

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

  getUsers(pageNumber?, pageSize?, userParams?): Observable<PaginatedResult<User[]>> {
    const paginatedResult: PaginatedResult<User[]> = new PaginatedResult<User[]>();

    let httpParams = new HttpParams();


    if (pageNumber != null && pageSize != null) {
      httpParams = httpParams.append('pageNumber', pageNumber);
      httpParams = httpParams.append('pageSize', pageSize);
    }
    if (userParams != null) {
      httpParams = httpParams.append('minAge', userParams.minAge);
      httpParams = httpParams.append('maxAge', userParams.maxAge);
      httpParams = httpParams.append('gender', userParams.gender);
      httpParams = httpParams.append('orderBy', userParams.orderBy);
    }

    return this.http.get<User[]>(this.baseUrl, { observe: 'response', params: httpParams }).pipe(
      map(response => {
        paginatedResult.result = response.body;
        const header = response.headers.get('Pagination');
        if (header != null) {
          paginatedResult.pagiantion = JSON.parse(header);
        }
        return paginatedResult;
      })
    );
  }
  updateUser(id: number, user: User) {
    return this.http.put(this.baseUrl + id, user);
  }
  setMainPhoto(userId: number, id: number) {
    return this.http.post(environment.apiUrl + 'users/' + userId + '/photos/' + id + '/setmain', {});
  }
  deletePhoto(userId: number, id: number) {
    return this.http.delete(environment.apiUrl + 'users/' + userId + '/photos/' + id);
  }
}
