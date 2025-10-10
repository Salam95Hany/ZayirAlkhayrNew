import { Component, EventEmitter, Input, OnInit, Output, ViewChild } from "@angular/core";
import { Slide } from "./media.interface";
import { trigger, transition, useAnimation } from "@angular/animations";

import {
  AnimationType,
  scaleIn,
  scaleOut,
  fadeIn,
  fadeOut,
  flipIn,
  flipOut,
  jackIn,
  jackOut
} from "./media.animations";
import { NgFor, NgIf } from "@angular/common";

@Component({
  selector: 'media',
  standalone: true,
  imports: [NgIf,NgFor],
  templateUrl: './media.component.html',
  styleUrls: ['./media.component.css'],
  animations: [
    trigger('slideAnimation', [
      /* scale */
      transition('void => scale', [
        useAnimation(scaleIn, { params: { time: '900ms' } }),
      ]),
      transition('scale => void', [
        useAnimation(scaleOut, { params: { time: '900ms' } }),
      ]),

      /* fade */
      transition('void => fade', [
        useAnimation(fadeIn, { params: { time: '500ms' } }),
      ]),
      transition('fade => void', [
        useAnimation(fadeOut, { params: { time: '500ms' } }),
      ]),

      /* flip */
      transition('void => flip', [
        useAnimation(flipIn, { params: { time: '500ms' } }),
      ]),
      transition('flip => void', [
        useAnimation(flipOut, { params: { time: '500ms' } }),
      ]),

      /* JackInTheBox */
      transition('void => jackInTheBox', [
        useAnimation(jackIn, { params: { time: '700ms' } }),
      ]),
      transition('jackInTheBox => void', [
        useAnimation(jackOut, { params: { time: '700ms' } }),
      ]),
    ]),
  ],
})
export class CarouselComponent implements OnInit {
  // Main Array
  @Input() slides: Slide[];
  // FOR CAROUSEL Options
  @Input() autoPlay = false;
  @Input() autoplayTime = 5000;
  @Input() isCarousel = false;
  currentSlide = 0;
  intervalSlides: any;
  // FOR GALLERY Options
  @Input() isGallery = false;
  @Input() currentGallery = 0;
  galleryIndexText = 0;
  rotating = 0;
  initZooming = 1;
  zooming = 0;

  // FOR Animation Options
  animationTypeImg = AnimationType.Scale;
  animationTypeText = AnimationType.JackInTheBox;


  constructor() {}
// FOR CAROUSEL Event
  onPreviousClick() {
    const previous = this.currentSlide - 1;
    this.currentSlide = previous < 0 ? this.slides.length - 1 : previous;
    clearInterval(this.intervalSlides);
    this.autoplay();
  }
  onNextClick() {
    const next = this.currentSlide + 1;
    this.currentSlide = next === this.slides.length ? 0 : next;
    clearInterval(this.intervalSlides);
    this.autoplay();
  }
  autoplay() {
    this.intervalSlides = setInterval(() => {
      if (this.currentSlide >= 0) {
        this.onNextClick();
      }
    }, this.autoplayTime);
  }




  ngOnInit() {
    if (this.autoPlay) {
      this.autoplay();
    }
  }

  preloadImages() {
    for (const slide of this.slides) {
      new Image().src = slide.src;
    }
  }

// FOR GALLERY Event
  onPreviousClickGallery() {
    const previous = this.currentGallery - 1;
    this.currentGallery = previous < 0 ? this.slides.length - 1 : previous;
  }

  onNextClickGallery() {
    const next = this.currentGallery + 1;
    this.currentGallery = next === this.slides.length ? 0 : next;
  }

  @Output() closeGallery = new EventEmitter<boolean>();
  closeGAllery(event) {
    var target = event.target || event.srcElement || event.currentTarget;
    if (target.classList.contains("noClose")) {
      return;
    } else {
      this.closeGallery.emit();
      this.initZooming = 1;
      this.zooming = 0;
      this.rotating = 0;
    }
  }

  onRotate(rotate: string) {
    if (rotate === 'right') {
      this.rotating += 90;
    }
    if (rotate === 'left') {
      this.rotating -= 90;
    }
  }

  onZoom(zoom: string) {
    if (zoom === 'in') {
      this.zooming += 1;
      if (this.initZooming === 1 && this.zooming >= 5) {
        this.zooming = 5;
      }
      if (this.initZooming === 0 && this.zooming > 9) {
        this.initZooming = 1;
        this.zooming = 0;
      }
    }
    if (zoom === 'out') {
      this.zooming -= 1;
      if (this.zooming < 0) {
        this.zooming = 9;
        this.initZooming = 0;
      }
      if (this.initZooming === 0 && this.zooming <= 5) {
        this.zooming = 5;
      }
    }
  }

}
