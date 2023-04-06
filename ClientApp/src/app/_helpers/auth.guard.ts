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
    //else {
    //  this.accountService.get().pipe(first())
    //    .subscribe({
    //      next: () => {
    //        // get return url from query parameters or default to home page
    //        //const returnUrl = this.route.snapshot.queryParams['returnUrl'] || '/';
    //        //this.router.navigateByUrl(returnUrl);
    //      },
    //      error: error => {
    //        this.alertService.error(error);
    //        /*this.loading = false;*/
    //        // not logged in so redirect to login page with the return url
    //        this.router.navigate(['/account/login'], { queryParams: { returnUrl: state.url } });
    //        return false;
    //      }
    //    });
    //}
  
    //this.router.navigate(['/account/login'], { queryParams: { returnUrl: state.url } });
    //return false;
  }
}
