import { AbstractControl, ValidationErrors, ValidatorFn } from '@angular/forms';

export const needsIndividual: ValidatorFn = (
  formGroup: AbstractControl,
): ValidationErrors | null => {
  const clientTypeIsBusiness = formGroup.get('clientTypeIsBusiness')?.value as boolean;
  const individualClientControl = formGroup.get('individualClientId');

  if (!clientTypeIsBusiness || !individualClientControl) {
    return null;
  }

  if (individualClientControl.disabled) {
    return null;
  }

  const individualClientId = individualClientControl.value as string | null;

  if (individualClientId == null) {
    return { needsIndividual: true };
  }

  return null;
};
