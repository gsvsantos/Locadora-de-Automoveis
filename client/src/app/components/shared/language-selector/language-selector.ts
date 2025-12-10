import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { LanguageCode, LocalStorageService } from '../../../services/local-storage.service';

@Component({
  selector: 'app-language-selector',
  imports: [CommonModule, FormsModule],
  templateUrl: './language-selector.html',
  styleUrl: './language-selector.scss',
  standalone: true,
})
export class LanguageSelector {
  private readonly localStorageService = inject(LocalStorageService);

  public readonly availableLanguages: AvailableLanguage[] = [
    { code: 'en-US', label: 'English (US)' },
    { code: 'pt-BR', label: 'Português (Brasil)' },
    { code: 'es-ES', label: 'Español (España)' },
  ];

  public currentLanguage: LanguageCode = this.localStorageService.getCurrentLanguage();

  public onLanguageChange(newLanguageCode: LanguageCode): void {
    if (newLanguageCode !== this.currentLanguage) {
      this.currentLanguage = newLanguageCode;
      this.localStorageService.setCurrentLanguage(newLanguageCode);
      window.location.reload();
    }
  }
}

interface AvailableLanguage {
  code: LanguageCode;
  label: string;
}
