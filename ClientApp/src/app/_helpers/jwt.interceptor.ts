//This class intercepts http requests from the application to add a JWT auth token to the Authorization header if the user is logged in
import { Injectable } from '@angular/core';
import { HttpRequest, HttpHandler, HttpEvent, HttpInterceptor } from '@angular/common/http';
import { Observable } from 'rxjs';

import { environment } from 'src/environments/environment';
import { AccountService } from 'src/app/_services';

@Injectable()
export class JwtInterceptor implements HttpInterceptor {
  constructor(private accountService: AccountService) { }

  intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    //// if first request try to get windows logged user data
   if (request.url.endsWith('/users/authenticate') || request.url.endsWith('/users/register') || request.url.endsWith('/users') || request.url.match(/\/users\/\d+$/)) {
      // add auth header with jwt if user is logged in and request is to the api url
      const user = this.accountService.userValue;
      //const isLoggedIn = user && user.token;
      //const isApiUrl = request.url.startsWith(environment.apiUrl);
      //if (isLoggedIn && isApiUrl) {
      //  request = request.clone({
      //    setHeaders: {
      //      Authorization: `Bearer ${user.token}`
      //    }
      //  });
      //}

      return next.handle(request);
    }
    else {
      return next.handle(request);
    }

  }
}
