import { Injectable } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class LocalStorageService {
  private readonly themeKey: string = 'user-theme';

  public setTheme(theme: 'light' | 'dark'): void {
    localStorage.setItem(this.themeKey, theme);
  }

  public getTheme(): 'light' | 'dark' {
    return localStorage.getItem(this.themeKey) as 'light' | 'dark';
  }
}
