import { Component, DOCUMENT, inject, OnInit } from '@angular/core';
import { Router, RouterOutlet } from '@angular/router';
import { ShellComponent } from './components/shared/shell/shell.component';
import { PartialObserver } from 'rxjs';
import { AuthService } from './services/auth.service';
import { NotificationService } from './services/notification.service';
import { AsyncPipe } from '@angular/common';
import { LocalStorageService } from './services/local-storage.service';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, ShellComponent, AsyncPipe],
  templateUrl: './app.html',
  styleUrl: './app.scss',
})
export class App implements OnInit {
  private document = inject(DOCUMENT);
  private readonly authService = inject(AuthService);
  protected readonly notificationService = inject(NotificationService);
  protected readonly router = inject(Router);
  protected readonly accessToken$ = this.authService.getAccessToken();

  private localStorageService = inject(LocalStorageService);
  protected themeValue: 'light' | 'dark' = this.localStorageService.getTheme();
  protected isDarkMode: boolean = this.themeValue == 'dark';

  public ngOnInit(): void {
    this.applyThemeToDocumentBody(this.themeValue);
  }

  public logout(): void {
    const sairObserver: PartialObserver<null> = {
      error: (err: string) => this.notificationService.error(err),
      complete: () => void this.router.navigate(['/auth', 'login']),
    };

    this.authService.logout().subscribe(sairObserver);
  }

  public onThemeChange(isDark: boolean): void {
    this.isDarkMode = isDark;
    this.themeValue = isDark ? 'dark' : 'light';

    this.applyThemeToDocumentBody(this.themeValue);

    this.localStorageService.setTheme(this.themeValue);
  }

  private applyThemeToDocumentBody(theme: 'light' | 'dark'): void {
    if (theme === 'dark') {
      this.document.body.dataset['theme'] = 'dark';
    } else {
      this.document.body.dataset['theme'] = 'light';
    }
  }
}
