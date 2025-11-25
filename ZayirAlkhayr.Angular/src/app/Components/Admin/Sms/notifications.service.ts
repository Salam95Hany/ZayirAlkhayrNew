import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class NotificationsService {

  constructor() { }

  connect() {
    return new EventSource('http://localhost:52091/api/SmsSender/stream');
  }
}
