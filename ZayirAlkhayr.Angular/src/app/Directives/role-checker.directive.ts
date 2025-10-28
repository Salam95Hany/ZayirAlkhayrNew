import { Directive, ElementRef, Input } from '@angular/core';
import { AuthService } from '../Auth/auth.service';

@Directive({
  selector: '[appRole]',
  standalone: true
})
export class RoleCheckerDirective {
  @Input() pageKey: string;
  @Input() action: string;

  constructor(private ref: ElementRef<HTMLElement>, private authService: AuthService) { }

  ngOnInit(): void {
    if (!this.authService.isSupperAdmin)
      if (!this.authService.hasPermission(this.pageKey, this.action))
        this.ref.nativeElement?.remove();
  }
}
