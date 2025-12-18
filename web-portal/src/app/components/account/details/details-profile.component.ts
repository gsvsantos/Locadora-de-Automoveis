import { Component, inject } from '@angular/core';
import { AccountService } from '../../../services/account.service';
import { AsyncPipe } from '@angular/common';
import { Observable, shareReplay } from 'rxjs';
import { ClientProfile } from '../../../models/account.models';
import { TranslocoModule } from '@jsverse/transloco';

@Component({
  selector: 'app-details-profile.component',
  imports: [AsyncPipe, TranslocoModule],
  templateUrl: './details-profile.component.html',
  styleUrl: './details-profile.component.scss',
})
export class DetailsProfileComponent {
  protected readonly accountService = inject(AccountService);

  protected readonly account$: Observable<ClientProfile> = this.accountService
    .getProfile()
    .pipe(shareReplay({ bufferSize: 1, refCount: true }));
}
