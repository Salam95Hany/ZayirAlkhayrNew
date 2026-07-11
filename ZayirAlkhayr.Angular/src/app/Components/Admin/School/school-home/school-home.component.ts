import { CommonModule, DatePipe } from '@angular/common';
import { Component } from '@angular/core';
import { ActivatedRoute, RouterModule } from '@angular/router';
import { NgbDropdownModule } from '@ng-bootstrap/ng-bootstrap';
import { OverviewCardComponent } from '../../../../Shared/overview-card/overview-card.component';
import { ArabicDayDatePipe } from '../../../../Pipes/arabic-day-date.pipe';
import { MenueService, MenuType } from '../../../../Services/shared/menue.service';
import { MenuSidebarItem } from '../../../../Models/shared/MenueSidebarItem';

@Component({
  selector: 'app-school-home',
  standalone: true,
  imports: [CommonModule, RouterModule, NgbDropdownModule, OverviewCardComponent, ArabicDayDatePipe],
  providers: [DatePipe],
  templateUrl: './school-home.component.html',
  styleUrl: './school-home.component.css'
})
export class SchoolHomeComponent {
selectedTabName: string;
  menuItem: MenuSidebarItem;
  today = new Date();
  statisticsCardList: any[] = [
    {
      title: 'إجمالي الزائرين',
      number: 682,
      statusIcon: 'fa-arrow-circle-up fas',
      statusBgClass: 'bg-success-gradient',
    },
    {
      title: 'إجمالي المتبرعين',
      number: 84,
      statusIcon: 'fa-arrow-circle-up fas',
      statusBgClass: 'bg-primary-gradient',
    },
    {
      title: 'إجمالي المستخدمين النشطين',
      number: 7,
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
        let menue = JSON.parse(JSON.stringify(this.menuService.getMenuById(MenuType.School, this.selectedTabName)))
        this.menuItem = this.menuService.filterMenusByUserPermissions(menue);
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
