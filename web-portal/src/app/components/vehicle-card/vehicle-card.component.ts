import { Component, Input } from '@angular/core';
import { CommonModule, CurrencyPipe } from '@angular/common';
import { RouterLink } from '@angular/router';
import { GsButtons, gsButtonTypeEnum, gsTabTargetEnum, gsVariant } from 'gs-buttons';
import { Vehicle } from '../../models/vehicle.model';
import { TranslocoModule } from '@jsverse/transloco';

@Component({
  selector: 'app-vehicle-card',
  standalone: true,
  imports: [CommonModule, RouterLink, CurrencyPipe, TranslocoModule, GsButtons],
  templateUrl: './vehicle-card.component.html',
  styleUrl: './vehicle-card.component.scss',
})
export class VehicleCardComponent {
  protected readonly buttonType = gsButtonTypeEnum;
  protected readonly targetType = gsTabTargetEnum;
  protected readonly variantType = gsVariant;

  @Input({ required: true }) public vehicle!: Vehicle;
  @Input({ required: true }) public imageBaseUrl!: string;

  protected get imageUrl(): string {
    const imageName = this.vehicle.image as unknown as string;

    return `${this.imageBaseUrl}${imageName}`;
  }
}
