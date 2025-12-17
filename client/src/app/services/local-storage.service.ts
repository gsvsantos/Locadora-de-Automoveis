import { inject, Injectable } from '@angular/core';
import { TranslocoService } from '@jsverse/transloco';
import { BehaviorSubject } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class LocalStorageService {
  private readonly translocoService = inject(TranslocoService);

  private readonly themeKey: string = 'locadora-theme';
  private readonly languageKey: string = 'locadora-language';

  private readonly themeSubject: BehaviorSubject<ThemeType> = new BehaviorSubject<ThemeType>(
    this.getInitialTheme(),
  );

  private readonly languageSubject: BehaviorSubject<LanguageCode> =
    new BehaviorSubject<LanguageCode>(this.getInitialLanguage());

  public constructor() {
    this.translocoService.setActiveLang(this.getCurrentLanguage());
  }

  public setTheme(theme: ThemeType): void {
    localStorage.setItem(this.themeKey, theme);
    this.themeSubject.next(theme);
  }

  public getCurrentTheme(): ThemeType {
    return this.themeSubject.value;
  }

  public isDarkMode(): boolean {
    return this.themeSubject.value == 'dark';
  }

  private getInitialTheme(): ThemeType {
    const saved = localStorage.getItem(this.themeKey) as ThemeType | null;
    return saved ?? 'light';
  }

  public setLanguage(newLanguageCode: LanguageCode): void {
    if (!/^[a-z]{2}-[A-Z]{2}$/.test(newLanguageCode)) {
      throw new Error(`Invalid language: ${newLanguageCode}`);
    }
    localStorage.setItem(this.languageKey, newLanguageCode);
    this.translocoService.setActiveLang(newLanguageCode);
    this.languageSubject.next(newLanguageCode);
  }

  public getCurrentLanguage(): LanguageCode {
    return this.languageSubject.value;
  }

  private getInitialLanguage(): LanguageCode {
    const saved = localStorage.getItem(this.languageKey) as LanguageCode | null;
    return saved ?? 'en-US';
  }
}

export type LanguageCode = `${string}-${string}`;
export type ThemeType = 'light' | 'dark';
