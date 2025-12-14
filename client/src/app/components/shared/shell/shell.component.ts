import { Component, EventEmitter, inject, Input, Output } from '@angular/core';
import { BreakpointObserver, Breakpoints } from '@angular/cdk/layout';
import { AsyncPipe } from '@angular/common';
import { EMPTY, merge, Observable, Subject } from 'rxjs';
import { catchError, exhaustMap, map, shareReplay, startWith, tap } from 'rxjs/operators';
import { AuthenticatedUserModel } from '../../../models/auth.models';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';
import { SlideToggleComponent } from '../slide-toggle/slide-toggle.component';
import { GsButtons, gsButtonTypeEnum, gsTabTargetEnum, gsVariant } from 'gs-buttons';
import { LocalStorageService } from '../../../services/local-storage.service';
import { MultiSearchComponent } from '../../search/multi/multi-search.component';
import { LanguageSelector } from '../language-selector/language-selector';
import { NavbarItem } from '../../../models/navbar-item.model';
import { TranslocoModule } from '@jsverse/transloco';
import { AuthService } from '../../../services/auth.service';
import { NotificationService } from '../../../services/notification.service';

@Component({
  selector: 'app-shell',
  templateUrl: './shell.component.html',
  styleUrl: './shell.component.scss',
  imports: [
    AsyncPipe,
    RouterLink,
    RouterLinkActive,
    SlideToggleComponent,
    GsButtons,
    MultiSearchComponent,
    TranslocoModule,
    LanguageSelector,
  ],
})
export class ShellComponent {
  protected readonly breakpointObserver = inject(BreakpointObserver);
  protected readonly authService = inject(AuthService);
  protected readonly localStorageService = inject(LocalStorageService);
  protected readonly notificationService = inject(NotificationService);
  protected readonly router = inject(Router);

  protected readonly buttonType = gsButtonTypeEnum;
  protected readonly targetType = gsTabTargetEnum;
  protected readonly variantType = gsVariant;
  protected isSidebarOpen: boolean = false;
  protected themeValue: 'light' | 'dark' = this.localStorageService.getTheme();
  protected isDarkMode: boolean = this.themeValue == 'dark';

  @Input({ required: true }) public user?: AuthenticatedUserModel;
  @Output() public logoutEvent = new EventEmitter<void>();
  @Output() public themeEvent = new EventEmitter<boolean>();

  private readonly deimpersonateClickSubject$ = new Subject<void>();

  public isHandset$: Observable<boolean> = this.breakpointObserver
    .observe([Breakpoints.XSmall, Breakpoints.Small, Breakpoints.Handset])
    .pipe(
      map((mediaQueryResult) => mediaQueryResult.matches),
      tap((isHandset) => {
        if (!isHandset) {
          this.isSidebarOpen = false;
        }
      }),
      shareReplay(),
    );

  public navbarItems: NavbarItem[] = [
    {
      titleKey: 'navbar.home',
      icon: 'home',
      link: 'home',
    },
    {
      titleKey: 'navbar.groups',
      icon: 'bookmarks',
      link: 'groups',
    },
    {
      titleKey: 'navbar.billingPlans',
      icon: 'card_membership',
      link: 'billing-plans',
    },
    {
      titleKey: 'navbar.vehicles',
      icon: 'garage',
      link: 'vehicles',
    },
    {
      titleKey: 'navbar.clients',
      icon: 'person',
      link: 'clients',
    },
    {
      titleKey: 'navbar.drivers',
      icon: 'local_taxi',
      link: 'drivers',
    },
    {
      titleKey: 'navbar.extras',
      icon: 'currency_exchange',
      link: 'extras',
    },
    {
      titleKey: 'navbar.rentals',
      icon: 'car_rental',
      link: 'rentals',
    },
    {
      titleKey: 'navbar.partners',
      icon: 'handshake',
      link: 'partners',
    },
    {
      titleKey: 'navbar.coupons',
      icon: 'local_activity',
      link: 'coupons',
    },
  ];

  public openSidebar(): void {
    this.isSidebarOpen = !this.isSidebarOpen;
  }

  public closeSidebar(): void {
    this.isSidebarOpen = false;
  }

  protected get getUserRole(): string {
    return this.authService.getUserRole();
  }

  protected get isEmployee(): boolean {
    return this.authService.isEmployee;
  }

  protected get isAdmin(): boolean {
    return this.authService.isAdmin;
  }

  protected get isPlatformAdmin(): boolean {
    return this.authService.isPlatformAdmin;
  }

  protected get isManager(): boolean {
    return this.authService.isManager;
  }

  protected readonly isImpersonating$ = this.authService
    .getAuthMode()
    .pipe(map((mode) => mode === 'impersonated'));

  protected readonly effects$ = merge(
    this.deimpersonateClickSubject$.pipe(
      exhaustMap(() =>
        this.authService.stopImpersonation().pipe(
          tap(() => {
            this.notificationService.success('Back to PlatformAdmin.');
            void this.router.navigate(['/admin/tenants']);
          }),
          catchError((err: string) => {
            this.notificationService.error(String(err));
            return EMPTY;
          }),
        ),
      ),
    ),
  ).pipe(
    map(() => true),
    startWith(true),
  );

  protected deimpersonate(): void {
    this.deimpersonateClickSubject$.next();
  }
}
