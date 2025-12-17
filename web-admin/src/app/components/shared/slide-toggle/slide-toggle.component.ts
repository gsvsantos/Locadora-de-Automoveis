import { Component, EventEmitter, Input, Output } from '@angular/core';

@Component({
  selector: 'app-slide-toggle',
  imports: [],
  templateUrl: './slide-toggle.component.html',
  styleUrl: './slide-toggle.component.scss',
})
export class SlideToggleComponent {
  protected icon: string = "'light_mode'";
  @Input() public checked: boolean = false;
  @Output() public toggleChange = new EventEmitter<boolean>();

  public onToggle(): void {
    this.checked = !this.checked;
    this.toggleChange.emit(this.checked);
    this.changeIcon(this.checked);
  }

  private changeIcon(checked: boolean): void {
    if (checked) {
      this.icon = "'dark_mode'";
    } else {
      this.icon = "'light_mode'";
    }
  }
}
