import { BrowserModule } from '@angular/platform-browser';
import { NgModule, ErrorHandler, APP_INITIALIZER, LOCALE_ID } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule, HTTP_INTERCEPTORS} from '@angular/common/http';
import { RouterModule } from '@angular/router';

import { AppComponent } from './app.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { HomeComponent } from './home/home.component';
import { SearchDocumentsComponent } from './search-documents/search-documents.component';
import { AlertCloseComponent } from "./dialogs/alert-close/alert-close.component"; 
import { DetailInvoicesComponent } from "./dialogs/detail-invoices/detail-invoices.component";
import { AlertYesNoComponent } from "./dialogs/alert-yes-no/alert-yes-no.component";
import { ReprintDocumentsComponent } from "./reprint-documents/reprint-documents.component";

import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { MatSliderModule } from '@angular/material/slider';
import { MatTableModule } from '@angular/material/table';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatIconModule } from '@angular/material/icon';
import { MatCardModule } from '@angular/material/card';
import { MatDialogModule } from '@angular/material/dialog';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatButtonModule } from '@angular/material/button';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSelectModule } from '@angular/material/select';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatMenuModule } from '@angular/material/menu';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule, MAT_DATE_FORMATS, MAT_DATE_LOCALE } from '@angular/material/core';

import { FontAwesomeModule, FaIconLibrary } from '@fortawesome/angular-fontawesome';
import { faSquare, faCheckSquare, faFilePdf, faFileExcel, faUser } from '@fortawesome/free-solid-svg-icons';
import { faSquare as farSquare, faCheckSquare as farCheckSquare, faFilePdf as farFilePdf, faFileExcel as farFileExcel, faFileAlt as farFileAlt } from '@fortawesome/free-regular-svg-icons';

import { WinAuthInterceptor } from './common/winauth-interceptor';
// used to create backend
import { JwtInterceptor, ErrorInterceptor } from './_helpers';
import { AlertComponent } from './_components';
const accountModule = () => import('./account/account.module').then(x => x.AccountModule);
const usersModule = () => import('./users/users.module').then(x => x.UsersModule);

import { AuthGuard } from './_helpers';
import { AppConfig } from './app.config';

@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    HomeComponent,
    SearchDocumentsComponent,
    AlertCloseComponent,
    DetailInvoicesComponent,
    AlertYesNoComponent,
    ReprintDocumentsComponent,
    AlertComponent
  ],
  imports: [
    MatNativeDateModule,
    MatDatepickerModule,
    MatMenuModule,
    MatSidenavModule,
    MatSnackBarModule,
    MatSlideToggleModule,
    MatSelectModule,
    MatProgressSpinnerModule,
    FontAwesomeModule,
    MatCheckboxModule,
    MatButtonModule,
    MatDialogModule,
    MatCardModule,
    MatIconModule,
    MatPaginatorModule,
    MatFormFieldModule,
    MatInputModule,
    MatTableModule,
    ReactiveFormsModule,
    MatSliderModule,
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    FormsModule,
    RouterModule.forRoot([
      { path: '', component: SearchDocumentsComponent, pathMatch: 'full', canActivate: [AuthGuard] },
      { path: 'search-documents', component: SearchDocumentsComponent, canActivate: [AuthGuard]  },
      { path: 'reprint-documents', component: ReprintDocumentsComponent, canActivate: [AuthGuard] },
      { path: 'account', loadChildren: accountModule },
      { path: 'users', loadChildren: usersModule, canActivate: [AuthGuard] },
      // otherwise redirect to home
      { path: '**', redirectTo: '' }
    
], { relativeLinkResolution: 'legacy' }),
    BrowserAnimationsModule,
    MatToolbarModule
 
  ],
  providers: [
    MatDatepickerModule,
    MatNativeDateModule,
    { provide: MAT_DATE_LOCALE, useValue: 'it-IT' },// default localize for datapicker
    AppConfig,
    { provide: APP_INITIALIZER, useFactory: (config: AppConfig) => () => config.load(), deps: [AppConfig], multi: true },
    { provide: HTTP_INTERCEPTORS, useClass: JwtInterceptor, multi: true },
    { provide: HTTP_INTERCEPTORS, useClass: ErrorInterceptor, multi: true }

  ],
  bootstrap: [
    AppComponent
  ],
  exports: [
    MatPaginatorModule,
    MatToolbarModule,
    MatSidenavModule
  ]
  
})
export class AppModule {
  constructor(private library: FaIconLibrary) {
    library.addIcons(faSquare, faCheckSquare, faFilePdf, faFileExcel, farFilePdf, farFileExcel, farFileAlt, faUser);
  }
}
