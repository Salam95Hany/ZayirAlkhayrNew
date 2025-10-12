import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { PagingFilterModel } from '../../Models/shared/PagingFilterModel ';
import { ApiResponseModel } from '../../Models/shared/ErrorResponseModel';
import { map } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class SettingService {
  apiURL = environment.apiUrl;
  constructor(private http: HttpClient) { }

  GetAllUsers() {
    return this.http.get<ApiResponseModel<any>>(this.apiURL + 'Auth/GetAllUsers');
  }

    CreateUser(Model: any) {
    return this.http.post<ApiResponseModel<any>>(this.apiURL + 'Auth/CreateUser', Model);
  }

  EditUser(Model: any) {
    return this.http.post<ApiResponseModel<any>>(this.apiURL + 'Auth/EditUser', Model);
  }

  DeleteUser(UserId: string) {
    return this.http.get<ApiResponseModel<any>>(this.apiURL + 'Auth/DeleteUser?UserId=' + UserId);
  }

   DownloadBackupFile(fileName: string) {
    return this.http.get(this.apiURL + 'DbBackup/SaveDbBackupFile', {
      responseType: 'blob',
      observe: 'response'
    }).pipe(
      map((response: any) => {
        const downloadLink = document.createElement('a');
        downloadLink.href = URL.createObjectURL(new Blob([response.body], { type: response.body.type }));
        downloadLink.download = fileName + '.bak';
        downloadLink.click();
      })
    );
  }

  DownloadZipFile(Folder: string, fileName: string) {
    return this.http.get(this.apiURL + 'DbBackup/DownloadImagesFolder?Folder=' + Folder, {
      responseType: 'blob',
      observe: 'response'
    }).pipe(
      map((response: any) => {
        const downloadLink = document.createElement('a');
        downloadLink.href = URL.createObjectURL(new Blob([response.body], { type: response.body.type }));
        downloadLink.download = fileName + '.zip';
        downloadLink.click();
      })
    );
  }
}
