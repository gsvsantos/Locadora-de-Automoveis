import { AbstractControl, ValidationErrors, ValidatorFn } from '@angular/forms';

export const needsIndividualValidator: ValidatorFn = (
  formGroup: AbstractControl,
): ValidationErrors | null => {
  const clientTypeIsBusiness = formGroup.get('clientTypeIsBusiness')?.value as boolean;
  const individualClientControl = formGroup.get('individualClientId');
  const registerNew = formGroup.get('registerNewDriver')?.value as boolean;

  if (!clientTypeIsBusiness || !individualClientControl) {
    return null;
  }

  if (registerNew) {
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
