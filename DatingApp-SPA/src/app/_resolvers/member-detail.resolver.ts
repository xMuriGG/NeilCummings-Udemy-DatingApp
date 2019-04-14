import { Injectable } from '@angular/core';
import { Resolve, ActivatedRouteSnapshot, RouterStateSnapshot, Router, RouterModule } from '@angular/router';
import { User } from '../_models/user';
import { Observable, of } from 'rxjs';
import { UserService } from '../_services/user.service';
import { AlertifyService } from '../_services/alertify.service';
import { catchError } from 'rxjs/operators';

@Injectable()

// Note: resolving dobijanje podataka prije aktiviranje rute

export class MemberDetailResolver implements Resolve<User>{
    constructor(private userService: UserService, private router: Router, private alertify: AlertifyService) { }

    resolve(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): User | Observable<User> | Promise<User> {
        return this.userService.getUser(route.params.id).pipe(
            catchError(error => {
                this.alertify.error('Problem retrieving user data');
                this.router.navigate(['/members']);
                return of(null);
            })
            );
    }
}