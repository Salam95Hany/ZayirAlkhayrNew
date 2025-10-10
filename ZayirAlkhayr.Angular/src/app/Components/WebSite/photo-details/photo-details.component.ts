import { Component, OnInit } from '@angular/core';
import { Slide } from '../media/media.interface';
import { ActivatedRoute } from '@angular/router';
import { CarouselComponent } from '../media/media.component';
import { ZaWebsiteService } from '../../../Services/zainstitution/za-website.service';
import { NgFor, NgIf } from '@angular/common';

@Component({
  selector: 'app-photo-details',
  standalone: true,
  imports: [CarouselComponent,NgFor,NgIf],
  templateUrl: './photo-details.component.html',
  styleUrls: ['./photo-details.component.css']
})
export class PhotoDetailsComponent implements OnInit {
  isGallery = false;
  currentGallery: number = 0;
  PhotoDetails: any;
  PhotoId: any;
  imageUrls: Slide[] = [];


  constructor(private websiteService: ZaWebsiteService, private route: ActivatedRoute) { }

  ngOnInit(): void {
    this.PhotoId = this.route.snapshot.paramMap.get('id');
    this.GetPhotoWithDetailsById();
  }

  GetPhotoWithDetailsById() {
    this.websiteService.GetPhotoWithDetailsById(this.PhotoId).subscribe(data => {
      this.PhotoDetails = data.results;
      this.imageUrls = data.results.detailImages.map(i => { return { src: i } });
    })
  }
}
