import { Component, Inject, ViewChild, VERSION, AfterViewInit } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { MatTableDataSource } from '@angular/material/table';
import { MatButton } from '@angular/material/button';
import { MatSnackBar, MatSnackBarHorizontalPosition, MatSnackBarVerticalPosition, } from '@angular/material/snack-bar';
import { param } from 'jquery';
import { FormControl, FormGroup, Validators} from '@angular/forms';
import { BehaviorSubject, combineLatest, Observable, of } from "rxjs";
import { map } from "rxjs/operators";
import { MatDatepickerInputEvent } from '@angular/material/datepicker';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatPaginator, PageEvent } from '@angular/material/paginator';

import { TableUtil } from "../utils/tableUtils";
import * as XLSX from "xlsx";
import { AppConfig } from '../app.config';



@Component({
  selector: 'reprint-documents',
  templateUrl: './reprint-documents.component.html',
  styleUrls: ['./reprint-documents.component.scss']
})
export class ReprintDocumentsComponent {
  constructor(private http: HttpClient, @Inject('BASE_URL') baseUrl: string, private _snackBar: MatSnackBar,  appConfig: AppConfig) {
    this.url = baseUrl;
    this.confPgSizeOption = appConfig.settings.PageSizeOptions;
    this.printedStatus = appConfig.settings.WfStatePrinted;
    this.deliveredStatus = appConfig.settings.WfStateDelivered;
 
  }
  private url;
  private confPgSizeOption;
  deliveredStatus;
  printedStatus;
  printedFiles: PrintedOrder[];
  dataSource = new MatTableDataSource<any>();
  IsWait: boolean = true;
  clickedRows = new Set<PrintedOrder>();
  displayedFileColumns: string[] = ['invoicE_FILE', 'numDocs', 'pages', 'data', 'state', 'button'];
  // Paginator
  pageSizeOptions: number[] = [5, 10, 20, 50, 100, 500];
 

  @ViewChild(MatPaginator) paginator: MatPaginator;
  pageIndex: number;  //current page
  pageSize: number;   // the number of items displayed on each page 
  length: number;     //The length of the total number of items that are being paginated. Defaulted to 0.
  pageEvent: PageEvent; //Event page change
  pageParameterChanged: FilterOnPrintedOrder;
  //SnackBar
  durationInSeconds = 5;
  horizontalPosition: MatSnackBarHorizontalPosition = 'center';
  verticalPosition: MatSnackBarVerticalPosition = 'top';

  //Date
  range = new FormGroup({
    start: new FormControl(),
    end: new FormControl(),
  });

  events: string[] = [];
  addStartDateEvent(type: string, event: MatDatepickerInputEvent<Date>) {

    this.filteredValues.dateFrom = event.value;
    this.pageParameterChanged.dateFrom = new Date(Date.UTC(event.value.getFullYear(), event.value.getMonth(), event.value.getDate()));
  }
  addEndDateEvent(type: string, event: MatDatepickerInputEvent<Date>) {
    this.filteredValues.dateTo = event.value;
    this.pageParameterChanged.dateTo = new Date(Date.UTC(event.value.getFullYear(), event.value.getMonth(), event.value.getDate()));
  }

  //Filter column
  numColumnFilter = new FormControl();
  private filterValues = { id: '', name: '' }
  filteredValues = {
    invoicE_FILE: '',
    dateFrom: null,
    dateTo:null 
  };

  getPrintedDocs(): Observable<PagedResultBase> {
    return this.http.post<PagedResultBase>(this.url + 'PrintFiles/printedfilelist', this.pageParameterChanged);
  }

  ngOnInit(): void {
    this.pageSizeOptions=this.confPgSizeOption;
    this.pageParameterChanged = new FilterOnPrintedOrder();
    this.pageParameterChanged.invoiceName = '';
    this.pageParameterChanged.pageNumber = 0;
    this.pageParameterChanged.pageSize = this.pageSizeOptions[0] ;
    
    this.getPrintedDocs().subscribe(result => {
      if (result.results.length == 0) {
      }
      else {
        this.printedFiles = result.results;
        this.length = result.rowCount;

        this.pageIndex = 0;
        this.pageSize = this.pageSizeOptions[0];

        this.dataSource = new MatTableDataSource<PrintedOrder>(this.printedFiles);
        this.dataSource.paginator = this.paginator;
      }
      this.IsWait = false;
    }, error => {
      console.error(error);
      this.IsWait = false;
    });

    //Filter on invoice number event
    this.numColumnFilter.valueChanges.subscribe((numColumnFilterValue) => {
      this.filteredValues.invoicE_FILE = numColumnFilterValue;
      this.pageParameterChanged.invoiceName = numColumnFilterValue; 
    });
  
  }
  ngAfterViewInit(): void {
    this.dataSource.paginator = this.paginator;
  }

  public clickedOnRow(row) {
    // Selected rows
    this.clickedRows.clear();
    this.clickedRows.add(row);
  }

  public reprintDocument(row) {
    var id = row.id;
    var printedOrderId = row.ordeR_ID;
    var printedOrderItemId = row.pritedOrderItems[0].ordeR_ITEM_ID;
    this.http.get<Response>(this.url + 'PrintFiles/reprintorder', { params: { orderId: id } }).subscribe(result => {
      if (result.code =='0') {
        this.openSnackBar("Documento Stampato", "X", "green-snackbar");
      }
      else {
        this.openSnackBar("Si è verificato un errore: " + result.message, "X", "red-snackbar");
      }
      this.IsWait = false;
     
    }, error => {
      console.error(error);
      this.IsWait = false;
      this.openSnackBar("Si è verificato un errore", "X", "red-snackbar");
    });
  }

  public showDocument(row) {
    var itemId = row.pritedOrderItems[0].ordeR_ITEM_ID;

    this.http.get(this.url + "PrintFiles/downloadfile", {
      params: { orderId: itemId }
      , responseType:'arraybuffer'
    }).subscribe(response => this.downloadFile(response, "application/pdf"));
  }
  /**
   * Method is use to download file.
   * @param data - Array Buffer data
   * @param type - type of the document.
   */
  downloadFile(data: any, type: string) {
    let blob = new Blob([data], { type: type });
    let url = window.URL.createObjectURL(blob);
    let pwa = window.open(url);
    if (!pwa || pwa.closed || typeof pwa.closed == 'undefined') {
      alert('Please disable your Pop-up blocker and try again.');
    }
  }

  openSnackBar(message: string, label: string, className: string) {
    this._snackBar.open(message, label, {
      duration: this.durationInSeconds * 1000,
      horizontalPosition: this.horizontalPosition,
      verticalPosition: this.verticalPosition,
      panelClass: [className],
    });
  }

  createFilter() {
    let filterFunction = function (data: any, filter: string): boolean {
      let searchTerms = JSON.parse(filter);
      console.log(searchTerms);
      let inoiceNuberSearch = data.invoicE_FILE.trim().toLowerCase().indexOf(searchTerms.invoicE_FILE.trim().toLowerCase()) != -1;
      let dateSearched = data.date >= searchTerms.dateFrom && data.date <= searchTerms.dateTo;
      return inoiceNuberSearch || dateSearched;

    }
    return filterFunction;

  }

  clearFilter() {
    this.numColumnFilter.setValue('');
    this.range.reset();

    this.filteredValues.dateFrom = '';
    this.pageParameterChanged.dateFrom = undefined;

    this.filteredValues.dateTo = '';
    this.pageParameterChanged.dateTo = undefined;
    this.pageParameterChanged.invoiceName = '';

    this.searchFilter();
  }
  searchFilter() {
    this.IsWait = true;
    this.pageParameterChanged.pageNumber = 0;
    this.pageIndex = 0;
    this.getPrintedDocs().pipe(map(data => { this.length = data.rowCount; return data; })).subscribe(result => {
      if (result.results.length == 0) {
      }
      else {

      }

      this.length = result.rowCount;
      this.pageIndex = result.currentPage;
      this.pageSize = result.pageSize;

      this.printedFiles = result.results
      this.dataSource = new MatTableDataSource<PrintedOrder>(this.printedFiles);

      this.IsWait = false;
    }, error => {
      console.error(error);
      this.IsWait = false;
    });
  }

  public getServerData(event?: PageEvent) {
    this.IsWait = true;
    this.pageParameterChanged.pageNumber = event.pageIndex ;
    this.pageParameterChanged.pageSize = event.pageSize;
    this.pageParameterChanged.length = this.length;

    this.getPrintedDocs().pipe(map(data => { this.length = data.rowCount; return data; })).subscribe(result => {
      if (result.results.length == 0) {
      }
      else {
        this.length = result.rowCount;
        this.pageIndex = event.pageIndex;
        this.pageSize = event.pageSize;    
        this.printedFiles = result.results
        this.dataSource = new MatTableDataSource<PrintedOrder>(this.printedFiles);
      }
      this.IsWait = false;
    }, error => {
      console.error(error);
      this.IsWait = false;
    });
  }

  getNextData(currentSize, offset, limit) {
    var nextFilterPage = new FilterOnPrintedOrder();
    nextFilterPage.invoiceName = '';
    nextFilterPage.length = limit;
    nextFilterPage.pageSize = offset;
    nextFilterPage.pageNumber = currentSize;
    this.http.post<PagedResultBase>(this.url + 'PrintFiles/printedfilelist', nextFilterPage).subscribe(result => {

      result.results.length = result.rowCount;
      this.printedFiles.push(...result.results);
      this.dataSource = new MatTableDataSource<PrintedOrder>(result.results);
      this.dataSource.paginator = this.paginator;
    }, error => {
      console.error(error);
     
    });
  }

  exportTable() {
    TableUtil.exportTableToExcel("printedFilesTable");
  }

  exportArray() {
    const pritedOrdereArr: Partial<PrintedOrder>[] = this.printedFiles.map(x => ({
      invoicE_FILE: x.invoicE_FILE,
      n_DOCS: x.n_DOCS,
      totaL_PAGES: x.totaL_PAGES,
      date: x.date,
      state: x.state
    }));
    TableUtil.exportArrayToExcel(pritedOrdereArr, "ExampleArray");
  }
}

export interface PrintedOrder {
  id: number;
  ordeR_ID: string;
  invoicE_FILE: string;
  date: string;
  n_DOCS: number;
  state: string;
  totaL_PAGES: number;
  pritedOrderItems: PrintedOrderItem[];

}
interface PrintedOrderItem {
  id: number;
  ordeR_ITEM_ID: string;
  state: string;
}

interface ResponsePrintingOrderItems {
  ok: boolean;
  message: string;
  details: Details;
}
interface Details {
  orderItemId: string;
  printer: string;
  paper: string;
  workstep: string;
}




export class FilterOnPrintedOrder {
  pageNumber: number;
  pageSize: number;
  length: number;
  invoiceName: string;
  dateFrom: Date;
  dateTo: Date;
}

interface PagedResultBase {
  currentPage: number;  //current page
  pageCount: number;
  pageSize: number;     //total size
  rowCount: number;
  results: PrintedOrder[];
}

interface Response {
  code: string;
  message: string;
}
