import { Component, inject } from '@angular/core';
import { RouterLink, ActivatedRoute, Router } from '@angular/router';
import { TranslocoModule } from '@jsverse/transloco';
import { GsButtons, gsButtonTypeEnum, gsTabTargetEnum, gsVariant } from 'gs-buttons';
import { AuthService } from '../../../services/auth.service';

@Component({
  selector: 'app-configuration-menu.component',
  imports: [RouterLink, TranslocoModule, GsButtons],
  templateUrl: './configuration-menu.component.html',
  styleUrl: './configuration-menu.component.scss',
})
export class ConfigurationMenuComponent {
  private readonly authService = inject(AuthService);
  protected readonly route = inject(ActivatedRoute);
  protected readonly router = inject(Router);
  protected readonly buttonType = gsButtonTypeEnum;
  protected readonly targetType = gsTabTargetEnum;
  protected readonly variantType = gsVariant;
  protected readonly accessToken$ = this.authService.getAccessToken();

  protected get isEmployee(): boolean {
    return this.authService.isEmployee;
  }
}
