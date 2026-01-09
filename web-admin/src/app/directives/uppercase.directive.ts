import { Directive, HostListener, Optional, Self } from '@angular/core';
import { NgControl } from '@angular/forms';

@Directive({
  selector: '[appUppercase]',
  standalone: true,
})
export class UppercaseDirective {
  public constructor(@Optional() @Self() private ngControl: NgControl) {}

  @HostListener('input', ['$event'])
  public onInput(event: Event): void {
    const input = event.target as HTMLInputElement;
    const value = input.value;

    const upper = value.toUpperCase();

    if (value !== upper) {
      input.value = upper;

      if (this.ngControl && this.ngControl.control) {
        this.ngControl.control.setValue(upper, { emitEvent: false });
      }
    }
  }
}
