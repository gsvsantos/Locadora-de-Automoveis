import { inject, Injectable } from '@angular/core';
import { TranslocoService } from '@jsverse/transloco';
import { BehaviorSubject, Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class LocalStorageService {
  private readonly translocoService = inject(TranslocoService);

  private readonly themeKey: string = 'locadora-theme';
  private readonly languageKey: string = 'locadora-language';

  private readonly currentLanguageSubject: BehaviorSubject<LanguageCode> =
    new BehaviorSubject<LanguageCode>(this.loadInitialLanguage());

  public readonly currentLanguage$: Observable<LanguageCode> =
    this.currentLanguageSubject.asObservable();

  public constructor() {
    this.translocoService.setActiveLang(this.getCurrentLanguage());
  }

  public setTheme(theme: 'light' | 'dark'): void {
    localStorage.setItem(this.themeKey, theme);
  }

  public getTheme(): 'light' | 'dark' {
    return localStorage.getItem(this.themeKey) as 'light' | 'dark';
  }

  public getCurrentLanguage(): LanguageCode {
    return this.currentLanguageSubject.value;
  }

  public setCurrentLanguage(newLanguageCode: LanguageCode): void {
    if (!/^[a-z]{2}-[A-Z]{2}$/.test(newLanguageCode)) {
      throw new Error(`Invalid language: ${newLanguageCode}`);
    }
    localStorage.setItem(this.languageKey, newLanguageCode);
    this.translocoService.setActiveLang(newLanguageCode);
    this.currentLanguageSubject.next(newLanguageCode);
  }

  private loadInitialLanguage(): LanguageCode {
    const saved = localStorage.getItem(this.languageKey) as LanguageCode | null;
    return saved ?? 'en-US';
  }
}

export type LanguageCode = `${string}-${string}`;
