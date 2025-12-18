import { Component, EventEmitter, inject, Input, Output } from '@angular/core';
import { AsyncPipe } from '@angular/common';
import { AuthenticatedUserModel } from '../../../models/auth.models';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { SlideToggleComponent } from '../slide-toggle/slide-toggle.component';
import { GsButtons, gsButtonTypeEnum, gsTabTargetEnum, gsVariant } from 'gs-buttons';
import { MultiSearchComponent } from '../../search/multi/multi-search.component';
import { LanguageSelector } from '../language-selector/language-selector';
import { TranslocoModule } from '@jsverse/transloco';
import { AuthService } from '../../../services/auth.service';
import { LocalStorageService } from '../../../services/local-storage.service';

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
  protected readonly localStorageService = inject(LocalStorageService);
  protected readonly authService = inject(AuthService);
  protected readonly buttonType = gsButtonTypeEnum;
  protected readonly targetType = gsTabTargetEnum;
  protected readonly variantType = gsVariant;

  protected readonly year: number = new Date().getUTCFullYear();

  protected isMobileMenuOpen: boolean = false;
  protected isDarkMode: boolean = this.localStorageService.isDarkMode();

  @Input() public user?: AuthenticatedUserModel | null;
  @Output() public logoutEvent = new EventEmitter<void>();
  @Output() public themeEvent = new EventEmitter<boolean>();

  public navLinks = [
    { label: 'home', link: '/home', icon: 'home' },
    { label: 'myRentals', link: '/my-rentals', icon: 'key' },
  ];

  public toggleMobileMenu(): void {
    this.isMobileMenuOpen = !this.isMobileMenuOpen;
  }

  public closeMobileMenu(): void {
    this.isMobileMenuOpen = false;
  }
}
