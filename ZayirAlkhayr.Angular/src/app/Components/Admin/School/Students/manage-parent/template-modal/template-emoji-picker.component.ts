import { Component, EventEmitter, Output } from '@angular/core';
import { PickerComponent } from '@ctrl/ngx-emoji-mart';
import { EmojiEvent } from '@ctrl/ngx-emoji-mart/ngx-emoji';

@Component({
  selector: 'app-template-emoji-picker',
  standalone: true,
  imports: [PickerComponent],
  templateUrl: './template-emoji-picker.component.html',
  styleUrl: './template-emoji-picker.component.css'
})
export class TemplateEmojiPickerComponent {
  @Output() emojiSelected = new EventEmitter<string>();

  readonly i18n = {
    search: 'بحث عن رمز تعبيري',
    emojilist: 'قائمة الرموز التعبيرية',
    notfound: 'لم يتم العثور على رموز',
    clear: 'مسح',
    categories: {
      search: 'نتائج البحث', recent: 'المستخدمة كثيراً', people: 'الوجوه والأشخاص',
      nature: 'الحيوانات والطبيعة', foods: 'الطعام والشراب', activity: 'الأنشطة',
      places: 'السفر والأماكن', objects: 'الأشياء', symbols: 'الرموز', flags: 'الأعلام', custom: 'مخصصة'
    },
    skintones: {
      1: 'اللون الافتراضي', 2: 'فاتح', 3: 'فاتح متوسط', 4: 'متوسط', 5: 'داكن متوسط', 6: 'داكن'
    }
  };

  selectEmoji(event: EmojiEvent): void {
    if (event.emoji.native)
      this.emojiSelected.emit(event.emoji.native);
  }
}
