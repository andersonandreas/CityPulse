import { Component, OnInit, ViewChild, viewChild } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment.development';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';




import { City } from './city';


@Component({
  selector: 'app-cities',
  templateUrl: './cities.component.html',
  styleUrl: './cities.component.scss'
})

export class CitiesComponent implements OnInit {

  public displayedColumns: string[] = ['id', 'name', 'lat', 'lon'];
  public cities!: MatTableDataSource<City>;

  @ViewChild(MatPaginator) paginnator!: MatPaginator;




  constructor(private http: HttpClient) { }


  ngOnInit(): void {

    this.http.get<City[]>(environment.baseUrl + 'api/Cities')
      .subscribe({
        next: result => {
          this.cities = new MatTableDataSource<City>(result);
          this.cities.paginator = this.paginnator;
        },
        error: error => console.log(error)
      });



  }



}
