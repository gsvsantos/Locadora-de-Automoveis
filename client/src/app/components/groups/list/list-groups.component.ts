import { Component, inject } from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { GsButtons, gsButtonTypeEnum, gsTabTargetEnum, gsVariant } from 'gs-buttons';
import { map, filter } from 'rxjs';
import { AsyncPipe, TitleCasePipe } from '@angular/common';
import { Group } from '../../../models/group.models';

@Component({
  selector: 'app-list-groups.component',
  imports: [AsyncPipe, TitleCasePipe, RouterLink, GsButtons],
  templateUrl: './list-groups.component.html',
  styleUrl: './list-groups.component.scss',
})
export class ListGroupsComponent {
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

  protected readonly groups$ = this.route.data.pipe(
    filter((data) => data['groups'] as boolean),
    map((data) => data['groups'] as Group[]),
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
