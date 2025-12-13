import { AbstractControl, ValidationErrors, ValidatorFn } from '@angular/forms';

export const passwordMatchValidator: ValidatorFn = (
  group: AbstractControl,
): ValidationErrors | null => {
  const password = group.get('password')?.value as string | null;
  const confirmPassword = group.get('confirmPassword')?.value as string | null;

  if (!password || !confirmPassword) {
    return null;
  }

  if (password !== confirmPassword) {
    return { passwordMismatch: true };
  }

  return null;
};
