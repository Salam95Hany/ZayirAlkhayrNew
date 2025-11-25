import { Injectable } from '@angular/core';
import { environment } from '../../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class NotificationsService {
  apiURL = environment.apiUrl;
  constructor() { }

  connect() {
    return new EventSource(this.apiURL + 'SmsSender/stream');
  }
}
