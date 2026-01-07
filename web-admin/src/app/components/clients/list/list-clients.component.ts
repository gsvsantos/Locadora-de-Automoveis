import { Component, inject } from '@angular/core';
import { Client } from '../../../models/client.models';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { GsButtons, gsButtonTypeEnum, gsTabTargetEnum, gsVariant } from 'gs-buttons';
import { map, filter } from 'rxjs';
import { AsyncPipe, TitleCasePipe } from '@angular/common';
import { TranslocoModule } from '@jsverse/transloco';
import { NgxMaskPipe } from 'ngx-mask';

@Component({
  selector: 'app-list-clients.component',
  imports: [AsyncPipe, NgxMaskPipe, TitleCasePipe, TranslocoModule, RouterLink, GsButtons],
  templateUrl: './list-clients.component.html',
  styleUrl: './list-clients.component.scss',
})
export class ListClientsComponent {
  protected readonly route = inject(ActivatedRoute);
  protected readonly router = inject(Router);
  protected readonly buttonType = gsButtonTypeEnum;
  protected readonly targetType = gsTabTargetEnum;
  protected readonly variantType = gsVariant;

  protected readonly showingInactive$ = this.route.queryParams.pipe(
    map((params) => ({
      isInactive: params['isActive'] === 'false',
    })),
  );

  protected readonly clients$ = this.route.data.pipe(
    filter((data) => data['clients'] as boolean),
    map((data) => data['clients'] as Client[]),
  );

  public toggleFilter(filter: boolean): void {
    const newFilter = filter;

    void this.router.navigate([], {
      relativeTo: this.route,
      queryParams: { isActive: newFilter },
      queryParamsHandling: 'merge',
    });
  }
}
