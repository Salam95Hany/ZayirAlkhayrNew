import { Component } from '@angular/core';
import { NavigationEnd, Router, RouterModule } from '@angular/router';
import { StyleManagerService } from './Services/shared/style-manager.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterModule],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent {
  title = 'ZayirAlkhayr.Angular';

  constructor(private styleManager: StyleManagerService, private router: Router) {}

  ngOnInit() {
    this.router.events.subscribe(event => {
      if (event instanceof NavigationEnd) {
        if (event.url.startsWith('/admin')) {
          this.styleManager.setStyle('styles/style-admin.css?v=1');
        } else {
          this.styleManager.setStyle('styles/style-website.css?v=1');
        }
      }
    });
  }
  
}
