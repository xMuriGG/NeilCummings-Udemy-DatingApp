import { Injectable } from '@angular/core';
import { Resolve, ActivatedRouteSnapshot, RouterStateSnapshot, Router, RouterModule } from '@angular/router';
import { User } from '../_models/user';
import { Observable, of } from 'rxjs';
import { UserService } from '../_services/user.service';
import { AlertifyService } from '../_services/alertify.service';
import { catchError } from 'rxjs/operators';

@Injectable()

// Note: resolving dobijanje podataka prije aktiviranje rute

export class ListResolver implements Resolve<User[]> {
    constructor(private userService: UserService, private router: Router, private alertify: AlertifyService) { }
    pageNumber = 1;
    pageSize = 5;
    likesParams = 'Likers';


    resolve(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<User[]> {

        return this.userService.getUsers(this.pageNumber, this.pageSize, null, this.likesParams).pipe(
            catchError(error => {
                this.alertify.error('Problem retrieving users data');
                this.router.navigate(['/home']);
                return of(null);
            })
        );
    }
}
