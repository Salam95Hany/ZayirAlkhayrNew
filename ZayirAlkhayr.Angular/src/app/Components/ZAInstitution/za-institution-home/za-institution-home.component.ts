import { Component, OnInit } from '@angular/core';
import { MenuSidebarItem } from '../../../Models/shared/MenueSidebarItem';
import { NgbDropdownModule } from '@ng-bootstrap/ng-bootstrap';
import { CommonModule, DatePipe } from '@angular/common';
import { ActivatedRoute, RouterModule } from '@angular/router';
import { MenueService, MenuType } from '../../../Services/shared/menue.service';
import { OverviewCardComponent } from "../../../Shared/overview-card/overview-card.component";
import { ArabicDayDatePipe } from '../../../Pipes/arabic-day-date.pipe';

@Component({
  selector: 'app-za-institution-home',
  standalone: true,
  imports: [CommonModule, RouterModule, NgbDropdownModule, OverviewCardComponent,ArabicDayDatePipe],
  providers: [DatePipe],
  templateUrl: './za-institution-home.component.html',
  styleUrl: './za-institution-home.component.css'
})
export class ZaInstitutionHomeComponent implements OnInit {
  selectedTabName: string;
  menuItem: MenuSidebarItem;
  today = new Date();
  statisticsCardList: any[] = [
    {
      title: 'عدد المتبرعين',
      number: 56,
      statusIcon: 'fa-arrow-circle-up fas',
      statusBgClass: 'bg-success-gradient',
    },
    {
      title: 'إجمالي الايرادات',
      number: 150000,
      statusIcon: 'fa-arrow-circle-up fas',
      statusBgClass: 'bg-primary-gradient',
    },
    {
      title: 'إجمالي الصادرات',
      number: 25000,
      statusIcon: 'fa-arrow-circle-up fas',
      statusBgClass: 'bg-secondary-gradient',
    }
  ];
  isToggle = false;

  onToggleContent() {
    this.isToggle = !this.isToggle;
    const htmlElement = document.querySelector('html');
    if (this.isToggle) {
      htmlElement.style.cssText = `overflow: hidden`;
    } else {
      htmlElement.style.cssText = `overflow: auto`;
    }
  }

  onOverlayClicked() {
    this.isToggle = false;
    const htmlElement = document.querySelector('html');
    htmlElement.style.cssText = `overflow: auto`;
  }


  constructor(private route: ActivatedRoute, private datePipe: DatePipe, private menuService: MenueService) { }

  ngOnInit(): void {

    this.route.params.subscribe(params => {
      this.menuItem = null;
      if (params['tabName']) {
        this.selectedTabName = params['tabName'];
        this.menuItem = this.menuService.getMenuById(MenuType.ZAInstitution, this.selectedTabName);
      }
    });
    this.getGeneralAccountsStatistics();
  }

  getGeneralAccountsStatistics() {
    // this.generalAccountService.GetGeneralAccounts_Statistics().subscribe(data => {
    //   this.statisticsCardList = data;
    // },
    //   (error) => { console.log("error", error); }, () => { });
  }
}
