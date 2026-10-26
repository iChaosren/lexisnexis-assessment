import {ValidatorFn} from '@angular/forms';

export function oneOf(allowed: readonly unknown[]): ValidatorFn {
  return control =>
    allowed.includes(control.value) ? null : { oneOf: true };
}
