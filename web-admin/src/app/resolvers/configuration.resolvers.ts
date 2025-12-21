import { inject } from '@angular/core';
import { ResolveFn } from '@angular/router';
import { Configuration } from '../models/configuration.models';
import { ConfigurationService } from '../services/configuration.service';

export const configurationDetailsResolver: ResolveFn<Configuration> = () => {
  const configurationService: ConfigurationService = inject(ConfigurationService);

  return configurationService.details();
};
