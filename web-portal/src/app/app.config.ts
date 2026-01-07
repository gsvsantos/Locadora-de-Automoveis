/* eslint-disable id-length */
import {
  APP_INITIALIZER,
  LOCALE_ID,
  ApplicationConfig,
  isDevMode,
  provideBrowserGlobalErrorListeners,
  provideZonelessChangeDetection,
} from '@angular/core';
import { ActivatedRouteSnapshot, provideRouter, withViewTransitions } from '@angular/router';

import { provideTransloco } from '@jsverse/transloco';
import { provideEnvironments } from './providers/environment.provider';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';
import { TranslocoHttpLoader } from './transloco-loader';
import { routes } from './routes/app.routes';
import { provideOAuthClient } from 'angular-oauth2-oidc';
import { AuthService } from './services/auth.service';
import { RECAPTCHA_LOADER_OPTIONS } from 'ng-recaptcha-2';
import { provideNgxMask } from 'ngx-mask';

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideZonelessChangeDetection(),
    provideRouter(
      routes,
      withViewTransitions({
        onViewTransitionCreated: ({ transition, from, to }) => {
          const shouldAnimate =
            routeHasViewTransitionEnabled(from) && routeHasViewTransitionEnabled(to);

          if (!shouldAnimate) {
            transition.skipTransition();
          }
        },
      }),
    ),
    provideTransloco({
      config: {
        availableLangs: ['en-US', 'pt-BR', 'es-ES'],
        defaultLang: 'en-US',
        fallbackLang: 'en-US',
        reRenderOnLangChange: true,
        prodMode: !isDevMode(),
      },
      loader: TranslocoHttpLoader,
    }),
    provideEnvironments(),
    provideOAuthClient(),
    {
      provide: APP_INITIALIZER,
      useFactory: authInitializer,
      deps: [AuthService],
      multi: true,
    },
    {
      provide: RECAPTCHA_LOADER_OPTIONS,
      useFactory: (locale: string) => ({
        onBeforeLoad(url: URL): object {
          url.searchParams.set('hl', locale);

          return { url };
        },
      }),
      deps: [LOCALE_ID],
    },
    provideAnimationsAsync(),
    provideNgxMask(),
  ],
};

function authInitializer(authService: AuthService): object {
  return () => authService.initialize();
}

function routeHasViewTransitionEnabled(routeSnapshot: ActivatedRouteSnapshot): boolean {
  const deepestSnapshot = getDeepestSnapshot(routeSnapshot);

  const flag: undefined = deepestSnapshot.data['viewTransition'] as undefined;

  const hasCustomFlagDefined: boolean = flag !== undefined;

  const isFlagTrue: boolean = deepestSnapshot.data['viewTransition'] === true;

  const shouldAnimate: boolean = !hasCustomFlagDefined || isFlagTrue;

  return shouldAnimate;
}

function getDeepestSnapshot(routeSnapshot: ActivatedRouteSnapshot): ActivatedRouteSnapshot {
  let currentSnapshot: ActivatedRouteSnapshot = routeSnapshot;

  while (currentSnapshot.firstChild) {
    currentSnapshot = currentSnapshot.firstChild;
  }

  return currentSnapshot;
}
