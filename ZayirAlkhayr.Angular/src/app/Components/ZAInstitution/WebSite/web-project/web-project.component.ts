import { AfterViewInit, Component, ElementRef, Renderer2 } from '@angular/core';

@Component({
  selector: 'app-web-project',
  standalone: true,
  imports: [],
  templateUrl: './web-project.component.html',
  styleUrl: './web-project.component.css'
})
export class WebProjectComponent implements AfterViewInit {
  constructor(private el: ElementRef, private renderer: Renderer2) { }

  ngAfterViewInit(): void {
    const cards = this.el.nativeElement.querySelectorAll('.project-card');
    cards.forEach((card: HTMLElement, index: number) => {
      this.renderer.setStyle(card, 'animationDelay', `${index * 0.2}s`);
    });

    const donateButtons = this.el.nativeElement.querySelectorAll('.donate-btn');
    donateButtons.forEach((button: HTMLElement) => {
      this.renderer.listen(button, 'click', () => {
        this.renderer.setStyle(button, 'transform', 'scale(0.95)');
        setTimeout(() => {
          this.renderer.setStyle(button, 'transform', 'translateY(-2px)');
          alert('شكراً لك! سيتم توجيهك إلى صفحة التبرع');
        }, 150);
      });
    });

    const progressBars = this.el.nativeElement.querySelectorAll('.progress-fill');
    const observer = new IntersectionObserver((entries) => {
      entries.forEach(entry => {
        if (entry.isIntersecting) {
          this.renderer.setStyle(entry.target, 'animation', 'progressAnimation 2s ease-out forwards');
        }
      });
    });

    progressBars.forEach((bar: Element) => {
      observer.observe(bar);
    });
  }
}
