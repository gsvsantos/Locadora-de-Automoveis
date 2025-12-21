import { Component, inject } from '@angular/core';
import { AccountService } from '../../../services/account.service';
import { AsyncPipe } from '@angular/common';
import { Observable, shareReplay } from 'rxjs';
import { ClientProfile } from '../../../models/account.models';
import { TranslocoModule } from '@jsverse/transloco';

@Component({
  selector: 'app-details-account.component',
  imports: [AsyncPipe, TranslocoModule],
  templateUrl: './details-account.component.html',
  styleUrl: './details-account.component.scss',
})
export class DetailsAccountComponent {
  protected readonly accountService = inject(AccountService);

  protected readonly account$: Observable<ClientProfile> = this.accountService
    .getProfile()
    .pipe(shareReplay({ bufferSize: 1, refCount: true }));
}
