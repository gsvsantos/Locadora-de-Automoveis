import { Injectable, Inject, Renderer2, RendererFactory2 } from '@angular/core';
import { DOCUMENT } from '@angular/common';

export type NotificationType = 'success' | 'warning' | 'error';

@Injectable({
  providedIn: 'root',
})
export class NotificationService {
  private renderer: Renderer2;
  private container: HTMLDivElement | null = null;

  public constructor(
    rendererFactory: RendererFactory2,
    @Inject(DOCUMENT) private document: Document,
  ) {
    this.renderer = rendererFactory.createRenderer(null, null);
  }

  public success(message: string): void {
    this.show(message, 'success');
  }

  public warning(message: string): void {
    this.show(message, 'warning');
  }

  public error(message: string): void {
    this.show(message, 'error');
  }

  private show(message: string, type: NotificationType): void {
    this.ensureContainer();

    if (!this.container) return;

    const toast = this.renderer.createElement('div') as HTMLDivElement;

    this.renderer.addClass(toast, 'toast');
    this.renderer.addClass(toast, type);

    const text = this.renderer.createText(message) as Text;
    this.renderer.appendChild(toast, text);

    this.renderer.appendChild(this.container, toast);

    setTimeout(() => {
      if (this.container) {
        this.renderer.removeChild(this.container, toast);

        if (this.container.childNodes.length === 0) {
          this.renderer.removeChild(this.document.body, this.container);
          this.container = null;
        }
      }
    }, 4000);
  }

  private ensureContainer(): void {
    if (this.container) {
      return;
    }

    this.container = this.document.querySelector('.toast-container');

    if (!this.container) {
      this.container = this.renderer.createElement('div') as HTMLDivElement;
      this.renderer.addClass(this.container, 'toast-container');
      this.renderer.appendChild(this.document.body, this.container);
    }
  }
}
