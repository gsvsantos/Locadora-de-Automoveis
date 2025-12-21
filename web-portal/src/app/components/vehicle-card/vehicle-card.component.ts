import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { GsButtons, gsButtonTypeEnum, gsTabTargetEnum, gsVariant } from 'gs-buttons';
import { Vehicle } from '../../models/vehicle.models';
import { TranslocoModule } from '@jsverse/transloco';
import { RentalBlockReason } from '../../models/rental.models';

@Component({
  selector: 'app-vehicle-card',
  standalone: true,
  imports: [CommonModule, RouterLink, TranslocoModule, GsButtons],
  templateUrl: './vehicle-card.component.html',
  styleUrl: './vehicle-card.component.scss',
})
export class VehicleCardComponent {
  protected readonly buttonType = gsButtonTypeEnum;
  protected readonly targetType = gsTabTargetEnum;
  protected readonly variantType = gsVariant;

  @Input({ required: true }) public vehicle!: Vehicle;
  @Input({ required: true }) public imageBaseUrl!: string;
  @Input() public canRent: boolean = true;
  @Input() public rentalBlockReason: RentalBlockReason = null;
  @Input() public activeRentalPlate: string | null = null;

  protected get imageUrl(): string {
    const imageName = this.vehicle.image as unknown as string;

    return `${this.imageBaseUrl}${imageName}`;
  }

  protected get rentDisabled(): boolean {
    return !this.canRent;
  }
}
