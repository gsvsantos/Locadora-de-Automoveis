import { Component, EventEmitter, inject, Input, Output } from '@angular/core';
import { BreakpointObserver, Breakpoints } from '@angular/cdk/layout';
import { AsyncPipe } from '@angular/common';
import { Observable } from 'rxjs';
import { map, shareReplay, tap } from 'rxjs/operators';
import { AuthenticatedUserModel } from '../../../models/auth.models';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { SlideToggleComponent } from '../slide-toggle/slide-toggle.component';
import { GsButtons, gsButtonTypeEnum, gsTabTargetEnum, gsVariant } from 'gs-buttons';
import { LocalStorageService } from '../../../services/local-storage.service';
import { MultiSearchComponent } from '../../search/multi/multi-search.component';
import { LanguageSelector } from '../language-selector/language-selector';
import { NavbarItem } from '../../../models/navbar-item.model';
import { TranslocoModule } from '@jsverse/transloco';

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
  private breakpointObserver = inject(BreakpointObserver);
  protected isSidebarOpen: boolean = false;
  protected readonly buttonType = gsButtonTypeEnum;
  protected readonly targetType = gsTabTargetEnum;
  protected readonly variantType = gsVariant;

  private localStorageService = inject(LocalStorageService);
  protected themeValue: 'light' | 'dark' = this.localStorageService.getTheme();
  protected isDarkMode: boolean = this.themeValue == 'dark';

  @Input({ required: true }) public user?: AuthenticatedUserModel;
  @Output() public logoutEvent = new EventEmitter<void>();
  @Output() public themeEvent = new EventEmitter<boolean>();

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
    console.log(this.user?.roles);
    this.isSidebarOpen = !this.isSidebarOpen;
  }

  public closeSidebar(): void {
    this.isSidebarOpen = false;
  }

  protected getUserRole(): 'platform-admin' | 'admin' | 'employee' | 'user' {
    const roles = this.user?.roles ?? [];

    if (roles.includes('PlatformAdmin')) return 'platform-admin';
    if (roles.includes('Admin')) return 'admin';
    if (roles.includes('Employee')) return 'employee';

    return 'user';
  }

  protected get isEmployee(): boolean {
    return this.getUserRole() === 'employee';
  }

  protected get isAdmin(): boolean {
    return this.getUserRole() === 'admin';
  }

  protected get isPlatformAdmin(): boolean {
    return this.getUserRole() === 'platform-admin';
  }

  protected get isManager(): boolean {
    return this.isAdmin || this.isPlatformAdmin;
  }
}
