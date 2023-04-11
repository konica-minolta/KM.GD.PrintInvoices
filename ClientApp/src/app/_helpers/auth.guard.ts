//this is used to prevent unauthenticated users from accessing
import { Injectable } from '@angular/core';
import { Router, CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { first } from 'rxjs/operators';

import { AccountService, AlertService} from 'src/app/_services';
import { User } from '../_models';

@Injectable({ providedIn: 'root' })
export class AuthGuard implements CanActivate {
 
  constructor(
    private router: Router,
    private accountService: AccountService,
    private alertService: AlertService
  ) { }

  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot) {
    const user = this.accountService.userValue;
  
    if (user) {
      // authorised so return true
      return true;
    }
    else {
      const myUser = this.accountService.getUserAccount();

      if (myUser) {
        return true;
      }
      else {
        this.router.navigate(['/account/login'], { queryParams: { returnUrl: state.url } });
        return false;
      }
    }

  }
}
