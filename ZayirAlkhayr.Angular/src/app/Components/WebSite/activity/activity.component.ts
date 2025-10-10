import { Component, OnInit } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { PagingFilterModel } from '../../../Models/shared/PagingFilterModel ';
import { ZaWebsiteService } from '../../../Services/zainstitution/za-website.service';
import { NgFor, NgIf } from '@angular/common';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-activity',
  standalone: true,
  imports: [NgFor,RouterLink],
  templateUrl: './activity.component.html',
  styleUrls: ['./activity.component.css']
})
export class ActivityComponent implements OnInit {
  ActivitiesData: any[] = [];
  PagingFilter: PagingFilterModel = {
    filterList: [],
    currentPage: 1,
    pageSize: 100
  }
  constructor(private websiteService: ZaWebsiteService,private modalService: NgbModal) {

  }

  ngOnInit(): void {
    this.GetAllActivities();
  }

  GetAllActivities() {
    this.websiteService.GetAllActivities(this.PagingFilter).subscribe(data => {
      this.ActivitiesData = data.results;
      this.ActivitiesData = this.ActivitiesData.filter(i => i.isVisible);
    });
  }

  OpenVodaFoneCashModal(content: any) {
    this.modalService.open(content, {
      size: 'lg',
      scrollable: true,
      centered: true
    });
  }

  OpenCashModal(content: any) {
    this.modalService.open(content, {
      size: 'lg',
      scrollable: true,
      centered: true
    });
  }

  OpenAhlyBankModal(content: any) {
    this.modalService.open(content, {
      size: 'lg',
      scrollable: true,
      centered: true
    });
  }

}
