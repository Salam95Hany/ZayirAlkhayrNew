import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, Router, RouterStateSnapshot } from '@angular/router';
import { Observable, of } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { environment } from '../../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class ProjectauthGuard implements CanActivate {
  apiURL = environment.apiUrl;
  constructor(private http: HttpClient, private router: Router) { }

  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<boolean> | Promise<boolean> | boolean {
    let ProjectId = route.paramMap.get('id');
    return this.http.get(this.apiURL + 'Projects/CheckProjectLinkIsActive?ProjectId=' + ProjectId).pipe(
      map((response: any) => {
        if (response.results) {
          return true;
        } else {
          this.router.navigate(['/project-denied',ProjectId]);
          return false;
        }
      }),
      catchError((error) => {
        this.router.navigate(['/project-denied',ProjectId]);
        return of(false);
      })
    );
  }

}
