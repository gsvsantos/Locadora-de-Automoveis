import { inject } from '@angular/core';
import { HttpInterceptorFn } from '@angular/common/http';
import { LocalStorageService } from '../services/local-storage.service';

export const languageInterceptor: HttpInterceptorFn = (req, next) => {
  const localStorageService: LocalStorageService = inject(LocalStorageService);
  const languageCode: string = localStorageService.getCurrentLanguage();

  const requestWithLanguage = req.clone({
    setHeaders: {
      'Accept-Language': languageCode,
    },
  });

  return next(requestWithLanguage);
};
