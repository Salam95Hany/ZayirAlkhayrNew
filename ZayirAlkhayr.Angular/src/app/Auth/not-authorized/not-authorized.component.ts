import { animate, keyframes, query, stagger, state, style, transition, trigger } from '@angular/animations';
import { CommonModule } from '@angular/common';
import { Component, ElementRef, ViewChild } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-not-authorized',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './not-authorized.component.html',
  styleUrl: './not-authorized.component.css',
  animations: [
    trigger('containerAnimation', [
      transition(':enter', [
        style({ opacity: 0, transform: 'translateY(100px) scale(0.8)' }),
        animate('800ms cubic-bezier(0.35, 0, 0.25, 1)',
          style({ opacity: 1, transform: 'translateY(0) scale(1)' }))
      ])
    ]),

    trigger('iconAnimation', [
      transition(':enter', [
        style({ transform: 'scale(0) rotate(-180deg)', opacity: 0 }),
        animate('1000ms 300ms cubic-bezier(0.68, -0.55, 0.265, 1.55)',
          style({ transform: 'scale(1) rotate(0deg)', opacity: 1 }))
      ])
    ]),

    trigger('textStagger', [
      transition(':enter', [
        query('.stagger-item', [
          style({ opacity: 0, transform: 'translateY(50px)' }),
          stagger(200, [
            animate('600ms cubic-bezier(0.35, 0, 0.25, 1)',
              style({ opacity: 1, transform: 'translateY(0)' }))
          ])
        ])
      ])
    ]),

    trigger('buttonHover', [
      state('normal', style({ transform: 'scale(1)' })),
      state('hovered', style({ transform: 'scale(1.05) translateY(-3px)' })),
      transition('normal <=> hovered', animate('300ms cubic-bezier(0.68, -0.55, 0.265, 1.55)'))
    ]),

    trigger('floatingRotate', [
      transition(':enter', [
        animate('20s linear', keyframes([
          style({ transform: 'rotate(0deg)', offset: 0 }),
          style({ transform: 'rotate(360deg)', offset: 1 })
        ]))
      ])
    ])
  ]
})
export class NotAuthorizedComponent {
  @ViewChild('particleCanvas', { static: false }) particleCanvas!: ElementRef<HTMLCanvasElement>;

  buttonStates: { [key: string]: string } = {
    home: 'normal',
    login: 'normal'
  };

  floatingElements = Array(8).fill(0).map((_, i) => ({
    id: i,
    size: Math.random() * 30 + 10,
    left: Math.random() * 100,
    animationDelay: Math.random() * 5,
    duration: Math.random() * 3 + 4
  }));

  constructor(private router: Router) { }

  ngOnInit() {
    this.createGlowEffect();
  }

  ngAfterViewInit() {
    this.initParticleSystem();
  }

  onButtonHover(button: string, isHovered: boolean) {
    this.buttonStates[button] = isHovered ? 'hovered' : 'normal';
  }

  goToHome() {
    this.router.navigate(['/home']);
  }

  goToLogin() {
    this.router.navigate(['/']);
  }

  private createGlowEffect() {
    const glowElements = document.querySelectorAll('.glow-effect');
    glowElements.forEach((element, index) => {
      setTimeout(() => {
        element.classList.add('active');
      }, index * 200);
    });
  }

  private initParticleSystem() {
    if (!this.particleCanvas) return;

    const canvas = this.particleCanvas.nativeElement;
    const ctx = canvas.getContext('2d');
    if (!ctx) return;

    canvas.width = window.innerWidth;
    canvas.height = window.innerHeight;

    const particles: any[] = [];
    const particleCount = 50;

    for (let i = 0; i < particleCount; i++) {
      particles.push({
        x: Math.random() * canvas.width,
        y: Math.random() * canvas.height,
        size: Math.random() * 3 + 1,
        speedX: (Math.random() - 0.5) * 2,
        speedY: (Math.random() - 0.5) * 2,
        opacity: Math.random() * 0.5 + 0.2
      });
    }

    const animate = () => {
      ctx.clearRect(0, 0, canvas.width, canvas.height);

      particles.forEach(particle => {
        particle.x += particle.speedX;
        particle.y += particle.speedY;

        if (particle.x < 0 || particle.x > canvas.width) particle.speedX *= -1;
        if (particle.y < 0 || particle.y > canvas.height) particle.speedY *= -1;

        ctx.globalAlpha = particle.opacity;
        ctx.fillStyle = '#ffffff';
        ctx.beginPath();
        ctx.arc(particle.x, particle.y, particle.size, 0, Math.PI * 2);
        ctx.fill();
      });

      requestAnimationFrame(animate);
    };

    animate();
  }

  trackByFn(index: number, item: any): number {
    return item.id;
  }
}




