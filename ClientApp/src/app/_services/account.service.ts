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
  //protected apiServer = AppConfig.settings.apiServer;
  constructor(private router: Router, private http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    //TODO Get current user


    //this.http.get('http://localhost:18185/api/account', { responseType: 'text' }).subscribe(account => {
    //  this.winAccount = account;
    //});

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

//  //get the user's windows account from api
//  async get(): Promise<User>  {
    
//    //let promise = new Promise((resolve, reject) => {
//    //  //TODO
//    //});

//    let myAccount = await this.http.get<User>(`${environment.apiUrl}/api/account`).toPromise();
//     //let myUser = await this.login(myAccount.username, "");

//     let loggedUsername = myAccount.username

//      let myUser = await this.http.post<User>(`${environment.apiUrl}/api/users/authenticate`, { loggedUsername }).toPromise()
//        .then(user => {
//          // store user details and jwt token in local storage to keep user logged in between page refreshes
//          localStorage.setItem('user', JSON.stringify(user));
//          this.userSubject.next(user);
//          return user;
//        });
//     /*let response1 = await this.http.get<User>(`${environment.apiUrl}/api/account`).subscribe(account => { this.login(account.username, "").subscribe(user => { loggedUser = user }) });*/


//    //myUser = await this.http.get<User>(`${environment.apiUrl}/api/account`).toPromise().then(
//    //  account => {
//    //    this.login(account.username, "").toPromise().then(
//    //      user => { myUser= user })
//    //  });

//    //let account = await fetch(`${environment.apiUrl}/api/account`);
//    //let accountjson = await account.json();
//    //let user = await this.login(accountjson.username, "").toPromise();
   
//     // .pipe(map(account => {
//     //  //localStorage.setItem('user', JSON.stringify(user));
//     // //this.userSubject.next(account);
//     //  this.login(account.username, "").subscribe(user => { loggedUser = user });

///*    let resp1 = this.http.get<User>(`${environment.apiUrl}/api/account`).subscribe(account => { this.login(account.username, "") });*/
    
  

//     //}));

    

//         return myUser;

   
//  }

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
