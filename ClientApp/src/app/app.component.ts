import { Component, OnInit} from '@angular/core';
import { AccountService, AlertService } from './_services';
import { User } from './_models';
import { first } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { AppConfig } from './app.config';


@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit {
  user:  User;
  title = 'X-Invoice';
  versionApp = environment.appVersion;
  constructor(private accountService: AccountService, private alertService: AlertService, appConfig: AppConfig) {

  }

  async ngOnInit() {
    await this.accountService.getUserAccount().then(res => { this.user = res; })
      .catch(error => {
        this.alertService.error(error);
        this.logout();
    });
  }

  logout() {
    this.accountService.logout();
  }
}
