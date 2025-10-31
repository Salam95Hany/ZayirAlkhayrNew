import { ApplicationConfig, importProvidersFrom, provideZoneChangeDetection } from '@angular/core';
import { provideRouter } from '@angular/router';
import { provideAnimations } from '@angular/platform-browser/animations';
import { provideToastr } from 'ngx-toastr';
import { routes } from './app.routes';
import { provideHttpClient } from '@angular/common/http';
import { NgxLoadingModule, ngxLoadingAnimationTypes } from 'ngx-loading';

export const appConfig: ApplicationConfig = {
  providers: [
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(routes),
    provideAnimations(),
    provideHttpClient(),
    importProvidersFrom(
      NgxLoadingModule.forRoot({
        animationType: ngxLoadingAnimationTypes.circleSwish,
        backdropBackgroundColour: 'rgba(0, 0, 0, 0.5)',
        backdropBorderRadius: '4px',
        primaryColour: '#3b82f6',
        secondaryColour: '#8b5cf6',
        tertiaryColour: '#ec4899',
        fullScreenBackdrop: true,
      })
    ),
    provideToastr({
      positionClass: 'toast-top-right',
      timeOut: 5000,
      closeButton: false,
      progressBar: true,
      preventDuplicates: true
    })
  ]
};
