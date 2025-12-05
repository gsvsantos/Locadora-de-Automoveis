import { Component, inject } from '@angular/core';
import { Router, RouterOutlet } from '@angular/router';
import { ShellComponent } from './components/shared/shell/shell.component';
import { PartialObserver } from 'rxjs';
import { AuthService } from './services/auth.service';
import { NotificationService } from './services/notification.service';
import { AsyncPipe } from '@angular/common';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, ShellComponent, AsyncPipe],
  templateUrl: './app.html',
  styleUrl: './app.scss',
})
export class App {
  private readonly authService = inject(AuthService);
  protected readonly notificationService = inject(NotificationService);
  protected readonly router = inject(Router);
  protected readonly accessToken$ = this.authService.getAccessToken();

  public logout(): void {
    const sairObserver: PartialObserver<null> = {
      error: (err: string) => this.notificationService.error(err),
      complete: () => void this.router.navigate(['/auth', 'login']),
    };

    this.authService.logout().subscribe(sairObserver);
  }
}
