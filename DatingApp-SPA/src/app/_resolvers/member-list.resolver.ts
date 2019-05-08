import { Injectable } from '@angular/core';
import { Resolve, ActivatedRouteSnapshot, RouterStateSnapshot, Router, RouterModule } from '@angular/router';
import { User } from '../_models/user';
import { Observable, of } from 'rxjs';
import { UserService } from '../_services/user.service';
import { AlertifyService } from '../_services/alertify.service';
import { catchError } from 'rxjs/operators';
import { userInfo } from 'os';

@Injectable()

// Note: resolving dobijanje podataka prije aktiviranje rute

export class MemberListResolver implements Resolve<User[]> {
    constructor(private userService: UserService, private router: Router, private alertify: AlertifyService) { }
    pageNumber = 1;
    pageSize = 5;
    user: User = JSON.parse(localStorage.getItem('user'));


    resolve(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<User[]> {

        const userParams = { gender: this.user.gender === 'male' ? 'female' : 'male', minAge: '', maxAge: '', orderBy: '' };
        return this.userService.getUsers(this.pageNumber, this.pageSize, userParams).pipe(
            catchError(error => {
                this.alertify.error('Problem retrieving users data');
                this.router.navigate(['/home']);
                return of(null);
            })
        );
    }
}