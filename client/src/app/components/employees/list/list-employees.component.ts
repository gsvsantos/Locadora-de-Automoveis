import { Component, inject } from '@angular/core';
import { Employee } from '../../../models/employee.models';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { filter, map } from 'rxjs';
import { AsyncPipe, DatePipe, TitleCasePipe } from '@angular/common';
import { GsButtons, gsButtonTypeEnum, gsTabTargetEnum, gsVariant } from 'gs-buttons';

@Component({
  selector: 'app-list-employees.component',
  imports: [AsyncPipe, TitleCasePipe, DatePipe, RouterLink, GsButtons],
  templateUrl: './list-employees.component.html',
  styleUrl: './list-employees.component.scss',
})
export class ListEmployeesComponent {
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

  protected readonly employees$ = this.route.data.pipe(
    filter((data) => data['employees'] as boolean),
    map((data) => data['employees'] as Employee[]),
  );

  public toggleFilter(filter: boolean): void {
    const newFilter = filter;

    console.log(newFilter);

    void this.router.navigate([], {
      relativeTo: this.route,
      queryParams: { isActive: newFilter },
      queryParamsHandling: 'merge',
    });
  }
}
