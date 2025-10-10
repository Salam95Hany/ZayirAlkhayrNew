import { DOCUMENT } from '@angular/common';
import { Inject, Injectable, Renderer2, RendererFactory2 } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class StyleManagerService {
 private renderer: Renderer2;
  private linkId = 'module-theme-stylesheet';

  constructor(rendererFactory: RendererFactory2, @Inject(DOCUMENT) private document: Document) {
    this.renderer = rendererFactory.createRenderer(null, null);
  }

  setStyle(href: string) {
    if (!href) return;
    let linkEl = this.document.getElementById(this.linkId) as HTMLLinkElement | null;
    if (linkEl) {
      if (linkEl.getAttribute('href') === href) return;
      linkEl.setAttribute('href', href);
    } else {
      linkEl = this.renderer.createElement('link') as HTMLLinkElement;
      linkEl.id = this.linkId;
      linkEl.rel = 'stylesheet';
      linkEl.href = href;
      this.renderer.appendChild(this.document.head, linkEl);
    }
  }

  removeStyle() {
    const linkEl = this.document.getElementById(this.linkId);
    if (linkEl) this.renderer.removeChild(this.document.head, linkEl);
  }
}
