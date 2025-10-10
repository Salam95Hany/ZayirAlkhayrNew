import { Component, CUSTOM_ELEMENTS_SCHEMA, OnInit, TemplateRef } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ZaWebsiteService } from '../../../Services/zainstitution/za-website.service';
import { NgFor, NgIf } from '@angular/common';

@Component({
  selector: 'app-activity-details',
  standalone: true,
  imports: [NgFor,NgIf],
  templateUrl: './activity-details.component.html',
  styleUrls: ['./activity-details.component.css'],
  schemas: [CUSTOM_ELEMENTS_SCHEMA]
})
export class ActivityDetailsComponent implements OnInit {
  ShowInput = false;
  AvtivityData: any;
  ActivityId: any;

  constructor(
    private modalService: NgbModal,
    private route: ActivatedRoute,
    private websiteService: ZaWebsiteService
  ) {}

  ngOnInit(): void {
    this.ActivityId = this.route.snapshot.paramMap.get('id');
    this.GetActivityWithSliderImagesById();
  }

  openDonateModal(modal: TemplateRef<any>) {
    this.modalService.open(modal, {
      size: 'xl',
      scrollable: true,
    });
  }

  GetActivityWithSliderImagesById() {
    this.websiteService
      .GetActivityWithSliderImagesById(this.ActivityId)
      .subscribe((data) => {
        this.AvtivityData = data.results;
      });
  }

}
