import { Injectable } from '@angular/core';
import { Router, CanDeactivate, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { AuthService } from '../_services/auth.service';
import { AlertifyService } from '../_services/alertify.service';
import { MemberEditComponent } from '../members/member-edit/member-edit.component';

@Injectable({
  providedIn: 'root'
})
export class UnsavedChangesGuard implements CanDeactivate<MemberEditComponent> {
  // tslint:disable-next-line: max-line-length
  canDeactivate(component: MemberEditComponent) {
    if (component.editForm.dirty) {
      return confirm('Unsaved changes detected.');
    }
    return true;
  }

  constructor(private authService: AuthService, private router: Router, private aleftify: AlertifyService) { }


}
