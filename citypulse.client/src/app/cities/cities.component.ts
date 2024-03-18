import { Component, OnInit, ViewChild, viewChild } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { environment } from '../../environments/environment.development';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort'

import { City } from './city';


@Component({
  selector: 'app-cities',
  templateUrl: './cities.component.html',
  styleUrl: './cities.component.scss'
})

export class CitiesComponent implements OnInit {

  public displayedColumns: string[] = ['id', 'name', 'lat', 'lon'];
  public cities!: MatTableDataSource<City>;

  defualtPageIndex: number = 0;
  defualtPageSize: number = 10;
  public  defaultSortColumn: string = 'name';
  public defaultSortOrder: 'asc' | 'desc' = 'asc';


  @ViewChild(MatPaginator) paginnator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;


  constructor(private http: HttpClient) { }



  ngOnInit() {
    this.loadData();
  }


  loadData() {
    let pageEvent = new PageEvent();
    pageEvent.pageIndex = this.defualtPageIndex;
    pageEvent.pageSize = this.defualtPageSize;

    this.getData(pageEvent);
  }


  getData(event: PageEvent) {
    let url = environment.baseUrl + 'api/Cities';
    let params = new HttpParams()
      .set('pageIndex', event.pageIndex.toString())
      .set('pageSize', event.pageSize.toString())
      .set('sortColumn', (this.sort) ? this.sort.active : this. defaultSortColumn)
      .set('sortOrder', (this.sort) ? this.sort.direction : this.defaultSortOrder);

    this.http.get<any>(url, { params })
      .subscribe({
        next: (result) => {
          this.paginnator.length = result.totalCount;
          this.paginnator.pageIndex = result.pageIndex;

          this.paginnator.pageSize = result.pageSize;
          this.cities = new MatTableDataSource<City>(result.data);
        },
        error: (error) => console.error('Error when fetching from the db ' + error)
      });

  }

}
