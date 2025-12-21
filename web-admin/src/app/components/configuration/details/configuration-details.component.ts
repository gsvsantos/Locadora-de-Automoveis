import { Component, inject } from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { GsButtons, gsButtonTypeEnum, gsTabTargetEnum, gsVariant } from 'gs-buttons';
import { filter, map, shareReplay } from 'rxjs';
import { Configuration } from '../../../models/configuration.models';
import { ConfigurationService } from '../../../services/configuration.service';
import { AsyncPipe } from '@angular/common';
import { TranslocoModule } from '@jsverse/transloco';

@Component({
  selector: 'app-configuration-details.component',
  imports: [AsyncPipe, RouterLink, TranslocoModule, GsButtons],
  templateUrl: './configuration-details.component.html',
  styleUrl: './configuration-details.component.scss',
})
export class ConfigurationDetailsComponent {
  protected readonly route = inject(ActivatedRoute);
  protected readonly router = inject(Router);
  protected readonly configurationService = inject(ConfigurationService);
  protected readonly buttonType = gsButtonTypeEnum;
  protected readonly targetType = gsTabTargetEnum;
  protected readonly variantType = gsVariant;

  protected readonly configuration$ = this.route.data.pipe(
    filter((data) => data['configuration'] as boolean),
    map((data) => data['configuration'] as Configuration),
    shareReplay({ bufferSize: 1, refCount: true }),
  );
}
