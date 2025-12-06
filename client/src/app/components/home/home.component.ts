import { AsyncPipe, DatePipe, TitleCasePipe } from '@angular/common';
import { Component, inject } from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { TranslocoModule } from '@jsverse/transloco';

@Component({
  selector: 'app-home',
  imports: [AsyncPipe, TitleCasePipe, DatePipe, RouterLink, TranslocoModule],
  templateUrl: './home.component.html',
  styleUrl: './home.component.scss',
})
export class Home {
  protected readonly route = inject(ActivatedRoute);
  protected readonly router = inject(Router);
}
