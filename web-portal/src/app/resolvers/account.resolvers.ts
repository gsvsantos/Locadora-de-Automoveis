import { inject } from '@angular/core';
import { ResolveFn } from '@angular/router';
import { ClientProfile } from '../models/account.models';
import { AccountService } from '../services/account.service';
import { Observable } from 'rxjs';

export const accountDetailsResolver: ResolveFn<ClientProfile> = (): Observable<ClientProfile> => {
  const accountService: AccountService = inject(AccountService);

  return accountService.getProfile();
};
