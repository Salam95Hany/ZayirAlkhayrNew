import { Component, OnInit } from '@angular/core';
import { PagingFilterModel } from '../../../Models/shared/PagingFilterModel ';
import { ZaWebsiteService } from '../../../Services/zainstitution/za-website.service';
import { NgFor, NgIf } from '@angular/common';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-photos',
  standalone: true,
  imports: [NgFor,RouterLink],
  templateUrl: './photos.component.html',
  styleUrls: ['./photos.component.css']
})
export class PhotosComponent implements OnInit {
  PhotosData: any[] = [];
  PagingFilter: PagingFilterModel = {
    filterList: [],
    currentPage: 1,
    pageSize: 100
  }

  constructor(private websiteService: ZaWebsiteService) { }

  ngOnInit(): void {
    this.GetAllPhotos();

  }

  GetAllPhotos() {
    this.websiteService.GetAllPhotos(this.PagingFilter).subscribe(data => {
      this.PhotosData = data.results.filter(i => i.isVisible);
    });
  }

}
