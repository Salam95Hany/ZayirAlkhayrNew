import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { trigger, state, style, animate, transition } from '@angular/animations';
import { ZaWebsiteService } from '../../../Services/zainstitution/za-website.service';
import { NgFor, NgIf } from '@angular/common';

@Component({
  selector: 'app-projects',
  standalone: true,
  imports: [NgIf,NgFor,RouterLink],
  templateUrl: './projects.component.html',
  styleUrls: ['./projects.component.css'],
  animations: [
    trigger('slideUp', [
      state('void', style({ transform: 'translateY(100%)', opacity: 0 })),
      transition(':enter', [
        animate('0.5s ease-out', style({ transform: 'translateY(0)', opacity: 1 }))
      ])
    ])
  ]
})
export class ProjectsComponent implements OnInit {
  collapsed = true;
  beneFactorCounter: string;
  DonationAmountToDisplay: string;
  DonationAmountToDisplay2: string;
  DonationAmountToDisplay3: string;
  DonationAmountToDisplay4: string;
  DonationAmountDigits: string[] = [];
  DonationAmountDigits2: string[] = [];
  DonationAmountDigits3: string[] = [];
  DonationAmountDigits4: string[] = [];
  currentIndex: number = 0;
  currentIndex2: number = 0;
  currentIndex3: number = 0;
  currentIndex4: number = 0;
  ProjectId: any;
  ProjectData: any;
    swiperBreakpoints = {
    0: { slidesPerView: 1 },
    768: { slidesPerView: 2 },
    1024: { slidesPerView: 3 }
  };

  constructor(private websiteService: ZaWebsiteService, private route: ActivatedRoute) { }

  ngOnInit() {
    this.ProjectId = this.route.snapshot.paramMap.get('id');
    this.GetWebSiteProjectsById();
  }

  GetWebSiteProjectsById() {
    this.websiteService.GetWebSiteProjectsById(this.ProjectId).subscribe(data => {
      this.ProjectData = data.results;
      this.DonationAmountToDisplay = this.ProjectData.totalDonationAmount;
      this.DonationAmountToDisplay2 = this.ProjectData.totalAmount;
      this.DonationAmountToDisplay3 = this.ProjectData.remainingAmount;
      this.DonationAmountToDisplay4 = this.ProjectData.benefactorCount;
      this.animateCounters();
    })
  }

  animateCounters() {
    const interval = setInterval(() => {
      if (this.currentIndex < this.DonationAmountToDisplay.length) {
        this.DonationAmountDigits.unshift(this.DonationAmountToDisplay[this.currentIndex]);
        this.currentIndex++;
      } else {
        clearInterval(interval);
      }
    }, 700);

    const interval2 = setInterval(() => {
      if (this.currentIndex2 < this.DonationAmountToDisplay2.length) {
        this.DonationAmountDigits2.unshift(this.DonationAmountToDisplay2[this.currentIndex2]);
        this.currentIndex2++;
      } else {
        clearInterval(interval2);
      }
    }, 700);

    const interval3 = setInterval(() => {
      if (this.currentIndex3 < this.DonationAmountToDisplay3.length) {
        this.DonationAmountDigits3.unshift(this.DonationAmountToDisplay3[this.currentIndex3]);
        this.currentIndex3++;
      } else {
        clearInterval(interval3);
      }
    }, 700);

    const Interval4 = setInterval(() => {
      if (this.currentIndex4 < this.DonationAmountToDisplay4.length) {
        this.DonationAmountDigits4.unshift(this.DonationAmountToDisplay4[this.currentIndex4]);
        this.currentIndex4++;
      } else {
        clearInterval(Interval4);
      }
    }, 700);
  }


}
