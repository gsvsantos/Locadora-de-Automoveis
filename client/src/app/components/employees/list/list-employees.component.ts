import { Component, inject } from '@angular/core';
import { Employee } from '../../../models/employee.models';
import { ActivatedRoute, RouterLink } from '@angular/router';
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
  protected readonly buttonType = gsButtonTypeEnum;
  protected readonly targetType = gsTabTargetEnum;
  protected readonly variantType = gsVariant;

  protected readonly employees$ = this.route.data.pipe(
    filter((data) => data['employees'] as boolean),
    map((data) => data['employees'] as Employee[]),
  );
}
