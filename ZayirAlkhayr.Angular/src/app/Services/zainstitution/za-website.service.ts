import { Injectable } from '@angular/core';
import { PagingFilterModel } from '../../Models/shared/PagingFilterModel ';
import { FilterModel } from '../../Models/shared/FilterModel';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { FileSortingModel } from '../../Models/shared/FileModel';
import { PagedResponseModel } from '../../Models/shared/PagedResponseModel';

@Injectable({
  providedIn: 'root'
})
export class ZaWebsiteService {
  apiURL = environment.apiUrl;
  constructor(private http: HttpClient) { }

  // ============================= SliderImage ==============================

  GetHomeSliderImages(PagingFilter: PagingFilterModel) {
    return this.http.post<PagedResponseModel<any[]>>(this.apiURL + 'WebsiteHome/GetHomeSliderImages', PagingFilter);
  }

  GetAllWebPagesFilters(PageName: string) {
    return this.http.get<FilterModel[]>(this.apiURL + 'WebsiteHome/GetAllWebPagesFilters?PageName=' + PageName);
  }

  GetPagesAutoSearch(SearchText: string) {
    return this.http.get<any[]>(this.apiURL + 'WebsiteHome/GetPagesAutoSearch?SearchText=' + SearchText);
  }

  AddNewSliderImage(Model: any) {
    return this.http.post<any>(this.apiURL + 'WebsiteHome/AddNewSliderImage', Model);
  }

  UpdateSliderImage(Model: any) {
    return this.http.post<any>(this.apiURL + 'WebsiteHome/UpdateSliderImage', Model);
  }

  DeleteSliderImage(SliderImageId: number) {
    return this.http.get<any>(this.apiURL + 'WebsiteHome/DeleteSliderImage?SliderImageId=' + SliderImageId);
  }

  // ============================= Activity ==============================

  GetAllActivities(PagingFilter: PagingFilterModel) {
    return this.http.post<PagedResponseModel<any[]>>(this.apiURL + 'Activity/GetAllActivities', PagingFilter);
  }

  GetActivitySliderImagesById(ActivityId: number) {
    return this.http.get<any[]>(this.apiURL + 'Activity/GetActivitySliderImagesById?ActivityId=' + ActivityId);
  }

  GetActivityWithSliderImagesById(ActivityId: number) {
    return this.http.get<any>(this.apiURL + 'Activity/GetActivityWithSliderImagesById?ActivityId=' + ActivityId);
  }

  AddNewActivity(Model: any) {
    return this.http.post<any>(this.apiURL + 'Activity/AddNewActivity', Model);
  }

  AddActivitySliderImage(Model: any) {
    return this.http.post<any>(this.apiURL + 'Activity/AddActivitySliderImage', Model);
  }

  UpdateActivity(Model: any) {
    return this.http.post<any>(this.apiURL + 'Activity/UpdateActivity', Model);
  }

  DeleteActivity(ActivityId: number) {
    return this.http.get<any>(this.apiURL + 'Activity/DeleteActivity?ActivityId=' + ActivityId);
  }

  ApplyFilesSorting(Model: FileSortingModel[], ActivityId: number) {
    return this.http.post<any>(this.apiURL + 'Activity/ApplyFilesSorting?ActivityId=' + ActivityId, Model);
  }

  // ============================= Photos ==============================

  GetAllPhotos(PagingFilter: PagingFilterModel) {
    return this.http.post<PagedResponseModel<any[]>>(this.apiURL + 'Photo/GetAllPhotos', PagingFilter);
  }

  GetPhotoDetails(PhotoId: number) {
    return this.http.get<any[]>(this.apiURL + 'Photo/GetPhotoDetails?PhotoId=' + PhotoId);
  }

  GetPhotoWithDetailsById(PhotoId: number) {
    return this.http.get<any>(this.apiURL + 'Photo/GetPhotoWithDetailsById?PhotoId=' + PhotoId);
  }

  AddNewPhoto(Model: any) {
    return this.http.post<any>(this.apiURL + 'Photo/AddNewPhoto', Model);
  }

  AddPhotoDetailsImage(Model: any) {
    return this.http.post<any>(this.apiURL + 'Photo/AddPhotoDetailsImage', Model);
  }

  UpdatePhoto(Model: any) {
    return this.http.post<any>(this.apiURL + 'Photo/UpdatePhoto', Model);
  }

  DeletePhoto(PhotoId: number) {
    return this.http.get<any>(this.apiURL + 'Photo/DeletePhoto?PhotoId=' + PhotoId);
  }

  ApplyPhotoFilesSorting(Model: FileSortingModel[], PhotoId: number) {
    return this.http.post<any>(this.apiURL + 'Photo/ApplyPhotoFilesSorting?PhotoId=' + PhotoId, Model);
  }

  // ============================= Events ==============================

  GetAllEvents(PagingFilter: PagingFilterModel) {
    return this.http.post<PagedResponseModel<any[]>>(this.apiURL + 'Event/GetAllEvents', PagingFilter);
  }

  GetAllWebSiteEvents() {
    return this.http.get<any[]>(this.apiURL + 'Event/GetAllWebSiteEvents');
  }

  GetEventSliderImagesById(EventId: number) {
    return this.http.get<any>(this.apiURL + 'Event/GetEventSliderImagesById?EventId=' + EventId);
  }

  AddNewEvent(Model: any) {
    return this.http.post<any>(this.apiURL + 'Event/AddNewEvent', Model);
  }

  AddEventSliderImage(Model: any) {
    return this.http.post<any>(this.apiURL + 'Event/AddEventSliderImage', Model);
  }

  UpdateEvent(Model: any) {
    return this.http.post<any>(this.apiURL + 'Event/UpdateEvent', Model);
  }

  DeleteEvent(EventId: number) {
    return this.http.get<any>(this.apiURL + 'Event/DeleteEvent?EventId=' + EventId);
  }

  ApplyEventFilesSorting(Model: FileSortingModel[], EventId: number) {
    return this.http.post<any>(this.apiURL + 'Event/ApplyEventFilesSorting?EventId=' + EventId, Model);
  }

  // ============================= Projects ==============================

  GetAllProjects(PagingFilter: PagingFilterModel) {
    return this.http.post<PagedResponseModel<any[]>>(this.apiURL + 'Projects/GetAllProjects', PagingFilter);
  }

  GetProjectsSliderImagesById(ProjectId: number) {
    return this.http.get<any>(this.apiURL + 'Projects/GetProjectsSliderImagesById?ProjectId=' + ProjectId);
  }

  AddNewProjects(Model: any) {
    return this.http.post<any>(this.apiURL + 'Projects/AddNewProjects', Model);
  }

  AddProjectsSliderImage(Model: any) {
    return this.http.post<any>(this.apiURL + 'Projects/AddProjectsSliderImage', Model);
  }

  UpdateProjects(Model: any) {
    return this.http.post<any>(this.apiURL + 'Projects/UpdateProjects', Model);
  }

  DeleteProjects(ProjectId: number) {
    return this.http.get<any>(this.apiURL + 'Projects/DeleteProjects?ProjectId=' + ProjectId);
  }

  GetWebSiteProjectsById(ProjectId: number) {
    return this.http.get<any>(this.apiURL + 'Projects/GetWebSiteProjectsById?ProjectId=' + ProjectId);
  }

  GetAllDeniedProjects() {
    return this.http.get<any[]>(this.apiURL + 'Projects/GetAllDeniedProjects');
  }
}
