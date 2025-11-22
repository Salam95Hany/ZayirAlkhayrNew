import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { ZaWebsiteService } from '../../../../Services/zainstitution/za-website.service';
import { NgFor, NgIf } from '@angular/common';

@Component({
  selector: 'app-project-denied',
  standalone: true,
  imports: [NgFor,NgIf,RouterLink],
  templateUrl: './project-denied.component.html',
  styleUrls: ['./project-denied.component.css']
})
export class ProjectDeniedComponent implements OnInit {
  ProjectsList: any[] = [];
  ProjectId: any;
  ProjectName = '';
  constructor(private route: ActivatedRoute, private websiteService: ZaWebsiteService) {

  }

  ngOnInit(): void {
    this.ProjectId = this.route.snapshot.paramMap.get('id');
    this.GetAllDeniedProjects()
  }

  GetAllDeniedProjects() {
    this.websiteService.GetAllDeniedProjects().subscribe(data => {
      this.ProjectsList = data.results;
      this.ProjectName = this.ProjectsList.find(i => i.id == this.ProjectId)?.name;
      this.ProjectsList = this.ProjectsList.filter(i => i.id != this.ProjectId && i.isVisible);
    });
  }
}
