import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../environments/environment';
import { IAppConfig } from './_models/app-config.model';

@Injectable()
export class AppConfig {
   settings: IAppConfig;
  constructor(private http: HttpClient) { }
  load() {
    //const jsonFile = `../../appsettings.${environment.name}.json`;
    //const jsonFile ="C:\\source\\repos\\KM.GD.PrintInvoices\appsettings.Development.json";

    const jsonFile ="api/Account/ClientAppSettings";
    return new Promise<void>((resolve, reject) => {
      this.http.get(jsonFile).toPromise().then((response: IAppConfig) => {
        this.settings = <IAppConfig>response;
        resolve();
      }).catch((response: any) => {
        reject(`Could not load file '${jsonFile}': ${JSON.stringify(response)}`);
      });
    });
  }
}
