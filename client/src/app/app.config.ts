import {
  ApplicationConfig,
  isDevMode,
  provideBrowserGlobalErrorListeners,
  provideZonelessChangeDetection,
} from '@angular/core';
import { ActivatedRouteSnapshot, provideRouter, withViewTransitions } from '@angular/router';

import { provideTransloco } from '@jsverse/transloco';
import { provideAuth } from './providers/auth.provider';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';
import { TranslocoHttpLoader } from './transloco-loader';
import { routes } from './routes/app.routes';
import { provideOAuthClient } from 'angular-oauth2-oidc';

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideZonelessChangeDetection(),
    provideRouter(
      routes,
      withViewTransitions({
        // eslint-disable-next-line id-length
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
        availableLangs: ['en-US'],
        defaultLang: 'en-US',
        fallbackLang: 'en-US',
        reRenderOnLangChange: true,
        prodMode: !isDevMode(),
      },
      loader: TranslocoHttpLoader,
    }),
    provideAuth(),
    provideOAuthClient(),
    provideAnimationsAsync(),
  ],
};

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
