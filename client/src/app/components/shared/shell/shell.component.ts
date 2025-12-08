import { Component, EventEmitter, inject, Input, Output } from '@angular/core';
import { BreakpointObserver, Breakpoints } from '@angular/cdk/layout';
import { AsyncPipe } from '@angular/common';
import { Observable } from 'rxjs';
import { map, shareReplay, tap } from 'rxjs/operators';
import { AuthenticatedUserModel } from '../../../models/auth.models';
import { RouterLink } from '@angular/router';
import { SlideToggleComponent } from '../slide-toggle/slide-toggle.component';
import { GsButtons, gsButtonTypeEnum, gsTabTargetEnum, gsVariant } from 'gs-buttons';
import { LocalStorageService } from '../../../services/local-storage.service';

@Component({
  selector: 'app-shell',
  templateUrl: './shell.component.html',
  styleUrl: './shell.component.scss',
  imports: [AsyncPipe, RouterLink, SlideToggleComponent, GsButtons],
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

  public navbarItems = [
    {
      title: 'Home',
      icon: 'home',
      link: 'home',
    },
    {
      title: 'Groups',
      icon: 'bookmarks',
      link: 'groups',
    },
    {
      title: 'Billing Plans',
      icon: 'card_membership',
      link: 'billing-plans',
    },
    {
      title: 'Vehicles',
      icon: 'garage',
      link: 'vehicles',
    },
    {
      title: 'Clients',
      icon: 'person',
      link: 'clients',
    },
    {
      title: 'Drivers',
      icon: 'local_taxi',
      link: 'drivers',
    },
    {
      title: 'Extras',
      icon: 'currency_exchange',
      link: 'extras',
    },
    {
      title: 'Rentals',
      icon: 'car_rental',
      link: 'rentals',
    },
    {
      title: 'Example',
      icon: '#',
      link: '#',
    },
    {
      title: 'Example',
      icon: '#',
      link: '#',
    },
    {
      title: 'Example',
      icon: '#',
      link: '#',
    },
    {
      title: 'Example',
      icon: '#',
      link: '#',
    },
  ];

  public openSidebar(): void {
    this.isSidebarOpen = !this.isSidebarOpen;
  }

  public closeSidebar(): void {
    this.isSidebarOpen = false;
  }
}
