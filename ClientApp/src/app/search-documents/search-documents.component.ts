import { Component, Inject, ViewChild, VERSION, AfterViewInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { MatPaginator } from '@angular/material/paginator';
import { MatTableDataSource } from '@angular/material/table';
import { animate, state, style, transition, trigger } from '@angular/animations';
import { MatDialog, MatDialogConfig  } from '@angular/material/dialog';
import { AlertCloseComponent } from '../dialogs/alert-close/alert-close.component';
import { DetailInvoicesComponent } from "../dialogs/detail-invoices/detail-invoices.component";
import { AlertYesNoComponent } from "../dialogs/alert-yes-no/alert-yes-no.component";
import { MatProgressSpinner } from '@angular/material/progress-spinner';
import { MatFormField } from '@angular/material/form-field';

import { MatCheckbox } from '@angular/material/checkbox';
import { MatButton } from '@angular/material/button';
import { MatSnackBar, MatSnackBarHorizontalPosition, MatSnackBarVerticalPosition, } from '@angular/material/snack-bar';
import { PrintInvoiceFiles } from '../_models/PrintInvoiceFiles';
import { SelectionModel } from '@angular/cdk/collections';
import { each } from 'jquery';
import { PrintInvoiceFilesList } from '../_models/PrintInvoiceFilesList';

@Component({
    selector: 'app-search-documents',
    templateUrl: './search-documents.component.html',
  styleUrls: ['./search-documents.component.scss'],
  animations: [
    trigger('detailExpand', [
      state('collapsed', style({ height: '0px', minHeight: '0' })),
      state('expanded', style({ height: '*' })),
      transition('expanded <=> collapsed', animate('225ms cubic-bezier(0.4, 0.0, 0.2, 1)')),
    ]),
  ],

})
export class SearchDocumentsComponent implements AfterViewInit {
  versionAng = 'Angular: v' + VERSION.full;

  @ViewChild(MatPaginator) paginator: MatPaginator;

  files: PrintInvoiceFiles[];


  ngAfterViewInit() {
    this.dataSource.paginator = this.paginator;
  }

  displayedFileColumns: string[] = ['select', 'fileName', 'numDocs','parentFolder'];
  pageSizeOptions: number[] = [5, 10, 25, 100];

  clickedRows = new Set<PrintInvoiceFiles>();
  dataSource = new MatTableDataSource<any>();
  selection = new SelectionModel<PrintInvoiceFiles>(true, []);


  IsWait: boolean = false;

  txtSearchInvoice: string = '';
  txtLastSearch: string = '';
  lastSearch: string = '';
  private url;
  expandedElement: PrintInvoiceFiles | null;

  //SnackBar options
  durationInSeconds = 5;
  horizontalPosition: MatSnackBarHorizontalPosition = 'center';
  verticalPosition: MatSnackBarVerticalPosition = 'top';
 
  constructor(private http: HttpClient, @Inject('BASE_URL') baseUrl: string, public dialog: MatDialog, private _snackBar: MatSnackBar) {
    this.url = baseUrl;
  }
  ngOnInit(): void {

  }

  onSubmit(value: string): void {
    alert('onSubmit value: ' + value);
  }


  onKeydown(event) {
    if (event.key === "Enter") {
      this.searchDocument();
    }
  }
  public searchDocument() {
    // search no empty string
    if (this.txtSearchInvoice.trim() != "") {
      this.IsWait = true;
      this.lastSearch = this.txtSearchInvoice;
      this.txtLastSearch = "Fattura/e ricercata/e: '" + this.lastSearch + "'";
      this.http.get<PrintInvoiceFiles[]>(this.url + 'PrintFiles/searchInAllPath', { params: { fileNameList: this.txtSearchInvoice } }).subscribe(result => {
        //no results
        if (result.length == 0) {
          this.IsWait = false;
          const dialogConfig = new MatDialogConfig();
          dialogConfig.data = { title: "Attenzione", content: "Non sono stati trovati risultati per la ricerca '" + this.lastSearch + "'" };
          dialogConfig.disableClose = true;
          let dialogRef = this.dialog.open(AlertCloseComponent, dialogConfig);

          dialogRef.afterClosed().subscribe(value => {
            console.log(`Dialog sent: ${value}`);
          });
          this.files = result;
       
        }
        else {
          this.IsWait = false;
          this.files = result;
          this.dataSource = new MatTableDataSource<PrintInvoiceFiles>(this.files);
          this.dataSource.paginator = this.paginator;
        }
      }, error => {
        console.error(error);
        this.openSnackBar("Si è verificato un errore: " + error.error.message, "X", "red-snackbar");
        this.IsWait = false;
      });
    }
    else {
      const dialogConfig = new MatDialogConfig();
      dialogConfig.data = { title: "Attenzione", content: "Occorre inserire un numero fattura da ricercare"};
      let dialogRef = this.dialog.open(AlertCloseComponent, dialogConfig);

      dialogRef.afterClosed().subscribe(value => {
        console.log(`Dialog sent: ${value}`);
      });

    }
    this.txtSearchInvoice = '';
   
  }

  public clickedOnRow(row) {
    // Selected rows

    if (this.clickedRows.has(row)) {
      this.clickedRows.delete(row);
    } else {
      this.clickedRows.add(row);
    }    
    this.selection.toggle(row);
  }

  public openSelected() {
    // open details
    const dialogConfig = new MatDialogConfig();
    var fileSelected = new PrintInvoiceFilesList();
    fileSelected.printInvoiceFiles = [...this.clickedRows];
    dialogConfig.data = fileSelected;
    dialogConfig.disableClose = true;
    let dialogRef = this.dialog.open(DetailInvoicesComponent, dialogConfig);

    dialogRef.afterClosed().subscribe(value => {
      console.log(`Dialog sent: ${value}`);
      if (value !== undefined) {
        if (value.code === "0") {
          this.openSnackBar("Documento Stampato", "X", "green-snackbar");
        }
        else {
          this.openSnackBar("Si è verificato un errore: " + value.message, "X", "red-snackbar");
        }
      } 
        this.selection.clear();
        this.clickedRows.clear();
     
    });
  }

  buildMultiInvoice() {
    const result = new PrintInvoiceFiles();
    this.clickedRows.forEach(row => {

    });
    return result;
  }

  /** Whether the number of selected elements matches the total number of rows. */
  isAllSelected() {
    const numSelected = this.selection.selected.length;
    const numRows = this.dataSource.data.length;
    return numSelected === numRows;
  }

  isOneSelected() {
    return this.selection.selected.length > 0 && this.files.length > 0;
  }

  /** Selects all rows if they are not all selected; otherwise clear selection. */
  masterToggle() {
    if (this.isAllSelected()) {
      this.selection.clear();
      this.clickedRows.clear();
    }
    else {
      this.dataSource.data.forEach(row => {
        this.selection.select(row);
        if (!this.clickedRows.has(row)) {
          this.clickedRows.add(row);
        }
      });
    }
      
  }

  /** The label for the checkbox on the passed row */
  checkboxLabel(row?: PrintInvoiceFiles): string {
    if (!row) {
      return `${this.isAllSelected() ? 'select' : 'deselect'} all`;
    }
    return `${this.selection.isSelected(row) ? 'deselect' : 'select'} row ${row.fileName}`;
  }

  openSnackBar(message:string, label:string, className:string) {
    this._snackBar.open(message, label, {
      duration: this.durationInSeconds * 1000,
      horizontalPosition: this.horizontalPosition,
      verticalPosition: this.verticalPosition,
      panelClass: [className],
    });
  }
}


interface Response {
  code: string;
  message: string;
}
