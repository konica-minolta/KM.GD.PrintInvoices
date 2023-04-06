import { Component, OnInit} from '@angular/core';
import { MatToolbar } from '@angular/material/toolbar';
import { MatIconModule } from '@angular/material/icon'
import { AppComponent } from '../app.component';
import { User } from '../_models';
import { AppConfig } from '../app.config';



@Component({
  selector: 'app-nav-menu',
  templateUrl: './nav-menu.component.html',
  styleUrls: ['./nav-menu.component.css']
})
export class NavMenuComponent implements OnInit {
  isExpanded = false;
  winAccount: User;
  isAdmin: boolean;
  
  constructor(parent: AppComponent, appConfig: AppConfig) {
    //parent.user.then(u => { this.winAccount = u.username });
    this.winAccount = parent.user;
    this.isAdmin = false;
  
    appConfig.settings.AdminUsers.forEach(admUsr => {
      if (this.winAccount.username.replace(/\\\\/g, '\\') == admUsr.replace(/\\\\/g, '\\')) {
        this.isAdmin = true;
    
      } });

  }

  ngOnInit() {
  }

  collapse() {
    this.isExpanded = false;
  }

  toggle() {
    this.isExpanded = !this.isExpanded;
  }
}
