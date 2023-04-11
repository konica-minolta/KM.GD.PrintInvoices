// handles communication between the Angular app and the backend api for everything related to accounts.
import { Injectable, Inject } from '@angular/core';
import { Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable } from 'rxjs';
import { map } from 'rxjs/operators';

import { environment } from 'src/environments/environment';
import { User } from 'src/app/_models';
import { AppConfig } from '../app.config';

@Injectable({ providedIn: 'root' })
export class AccountService {
  private userSubject: BehaviorSubject<User>;
  public user: Observable<User>;
  private url;
  constructor(private router: Router, private http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    this.url = baseUrl;
    this.userSubject = new BehaviorSubject<User>(JSON.parse(localStorage.getItem('user')));
    this.user = this.userSubject.asObservable();
  }

  public get userValue(): User {
    return this.userSubject.value;
  }

  login(username, password) {
    return this.http.post<User>(this.url +`api/users/authenticate`, { username, password })
      .pipe(map(user => {
        // store user details and jwt token in local storage to keep user logged in between page refreshes
        localStorage.setItem('user', JSON.stringify(user));
        this.userSubject.next(user);
        return user;
      }));
  }

  logout() {
    // remove user from local storage and set current user to null
    localStorage.removeItem('user');
    this.userSubject.next(null);
    this.router.navigate(['/account/login']);
  }

  register(user: User) {
    return this.http.post(this.url +`api/users/register`, user);
  }

  getAll() {
    return this.http.get<User[]>(this.url +`api/users`);
  }

  getById(id: string) {
    return this.http.get<User>(this.url + `api/users/${id}`);
  }

  update(id, params) {
    return this.http.put(this.url +`api/users/${id}`, params)
      .pipe(map(x => {
        // update stored user if the logged in user updated their own record
        if (id == this.userValue.userId) {
          // update local storage
          const user = { ...this.userValue, ...params };
          localStorage.setItem('user', JSON.stringify(user));

          // publish updated user to subscribers
          this.userSubject.next(user);
        }
        return x;
      }));
  }

  delete(id: string) {
    return this.http.delete(this.url +`api/users/${id}`)
      .pipe(map(x => {
        // auto logout if the logged in user deleted their own record
        if (id == this.userValue.userId) {
          this.logout();
        }
        return x;
      }));
  }
  async getAccountSync() {
    return await this.http.get<User>(this.url+`api/account`).toPromise();
  }

  async getUserSync(username, password) {
    return await this.http.post<User>(this.url+`api/users/authenticate`, { username, password }).toPromise()
      .then(user => {
        // store user details and jwt token in local storage to keep user logged in between page refreshes
        localStorage.setItem('user', JSON.stringify(user));
        this.userSubject.next(user);
        return user;
      });
     
  }

  async getUserAccount(): Promise<User> {
    let account = <User>await this.getAccountSync();

    let user = <User>await this.getUserSync(account.username, "").catch(
      error => console.log(error));
    if (user)
      this.router.navigate(['/search-documents']);
    return user;
  }

}
