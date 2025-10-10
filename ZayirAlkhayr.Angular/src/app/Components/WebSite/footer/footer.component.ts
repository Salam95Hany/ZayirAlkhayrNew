import { Component, OnInit } from '@angular/core';
import { AuthService } from '../../../Auth/auth.service';

@Component({
  selector: 'app-footer',
  standalone: true,
  imports:[],
  templateUrl: './footer.component.html',
  styleUrls: ['./footer.component.css']
})
export class FooterComponent implements OnInit {

  constructor(private authService: AuthService) {

  }

  ngOnInit(): void {
    this.CreateSessionId();
  }

  CreateSessionId() {
    this.authService.CreateSessionId();
  }

}
