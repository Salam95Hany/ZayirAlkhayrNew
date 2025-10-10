import { Component, OnInit, ViewChild } from '@angular/core';
import { Slide } from "../media/media.interface";
import { AnimationType } from "../media/media.animations";
import { Router, RouterLink, RouterLinkActive } from '@angular/router';
import { PagingFilterModel } from '../../../Models/shared/PagingFilterModel ';
import { ZaWebsiteService } from '../../../Services/zainstitution/za-website.service';
import { CommonModule } from '@angular/common';
import { CarouselComponent } from '../media/media.component';

@Component({
  selector: 'app-header',
  standalone: true,
  imports:[CommonModule,CarouselComponent,RouterLink,RouterLinkActive],
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.css']
})
export class HeaderComponent implements OnInit {
  @ViewChild("navbar") navbarEl: HTMLElement;
  PagingFilter: PagingFilterModel = {
    filterList: [],
    currentPage: 1,
    pageSize: 25,
  }
  collapsed = true;
  slides: Slide[] = [];
  animationType = AnimationType.Scale;

  constructor(private router: Router, private websiteService: ZaWebsiteService) {

  }

  ngOnInit(): void {
    this.GetHomeSliderImages();

  }

  GetHomeSliderImages() {
    this.websiteService.GetHomeSliderImages(this.PagingFilter).subscribe(data => {
      this.slides = data.results.filter(i => i.isVisible).map<Slide>(i => { return { headline: i.title, src: i.image } });
    });
  }

  GoToAdmin() {
    this.router.navigateByUrl('/login');
  }
}
