import { Component, CUSTOM_ELEMENTS_SCHEMA, OnInit } from '@angular/core';
import { ZaWebsiteService } from '../../../Services/zainstitution/za-website.service';
import { register } from 'swiper/element/bundle';
import { NgFor, NgIf } from '@angular/common';

@Component({
  selector: 'app-event',
  standalone: true,
  imports:[NgFor,NgIf],
  templateUrl: './event.component.html',
  styleUrls: ['./event.component.css'],
  schemas: [CUSTOM_ELEMENTS_SCHEMA]
})
export class EventComponent implements OnInit {
  EventsData: any[] = [];

  constructor(private websiteService: ZaWebsiteService) {
    register();
  }

  ngOnInit(): void {
    this.GetAllWebSiteEvents();
  }

  GetAllWebSiteEvents() {
    this.websiteService.GetAllWebSiteEvents().subscribe(data => {
      this.EventsData = data.results;
    })
  }

}
