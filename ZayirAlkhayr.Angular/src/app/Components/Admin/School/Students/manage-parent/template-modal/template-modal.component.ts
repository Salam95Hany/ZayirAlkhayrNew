import { Component, ElementRef, EventEmitter, HostListener, Input, OnDestroy, OnInit, Output, ViewChild } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ParentService } from '../../../../../../Services/school/parent.service';
import { DatePipe, NgFor, NgIf } from '@angular/common';
import { ToastrService } from 'ngx-toastr';
import { NgxLoadingModule } from "ngx-loading";
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { FormService } from '../../../../../../Services/shared/form.service';
import { CustomValidators, RegexType } from '../../../../../../Services/shared/custom-validators';
import { ZaInputWithLabelComponent } from '../../../../../../Shared/za-input-with-label/za-input-with-label.component';
import { AuthService } from '../../../../../../Auth/auth.service';
import { TemplateEmojiPickerComponent } from './template-emoji-picker.component';

@Component({
  selector: 'app-template-modal',
  standalone: true,
  imports: [NgFor, NgIf, NgxLoadingModule, ReactiveFormsModule, ZaInputWithLabelComponent, TemplateEmojiPickerComponent],
  templateUrl: './template-modal.component.html',
  styleUrl: './template-modal.component.css',
  providers: [DatePipe]
})
export class TemplateModalComponent implements OnInit, OnDestroy {
  @ViewChild('Editor', { static: true }) editor!: ElementRef<HTMLDivElement>;
  @ViewChild('emojiTrigger') emojiTrigger?: ElementRef<HTMLButtonElement>;
  @ViewChild('emojiPopover') emojiPopover?: ElementRef<HTMLDivElement>;
  @Input() TemplateId: number;
  @Input() TemplateVariables: any[] = [];
  @Output() RefreshData = new EventEmitter<boolean>();
  savedRange: Range | null = null;
  isEmojiPickerOpen = false;
  emojiPickerPosition = { top: '0px', left: '0px', width: '22rem' };

  charactersCount = 0;
  variablesCount = 0;
  previewHtml = '';
  previewTime: any;
  ItemForm: FormGroup;
  PreviewTemp = false;
  showLoader = false;
  groupedVariables: {
    category: string;
    categoryName: string;
    color: string;
    backColor: string;
    icon: string;
    variables: any[];
  }[] = [];
  formErrors = {
    name: ''
  };
  constructor(private modalService: NgbModal, private parentService: ParentService, private datePipe: DatePipe, private toaster: ToastrService,
    private formService: FormService, private fb: FormBuilder, private authService: AuthService
  ) {

  }

  ngOnInit(): void {
    this.PreviewTemp = false;
    this.FormInit();
    this.groupVariables();
    if (this.TemplateId)
      this.GetTemplateById();
  }

  FormInit() {
    this.ItemForm = this.fb.group({
      id: 0,
      name: ['', [Validators.required, CustomValidators.regexPattern(RegexType.noSpace)]],
      body: [''],
      variableIds: [''],
      insertUser: null
    });

    this.ItemForm.valueChanges.subscribe((data) => {
      this.formErrors = this.formService.validateForm(this.ItemForm, this.formErrors, true);
    });
  }

  GetTemplateById() {
    this.parentService.GetTemplateById(this.TemplateId).subscribe(data => {
      let obj = data.results;
      this.ItemForm.patchValue({
        id: obj.id,
        name: obj.name
      });

      this.BuildEditor(obj.body);
    })
  }

  groupVariables() {
    const map = new Map<string, any>();
    this.TemplateVariables.forEach(item => {

      if (!map.has(item.category)) {
        map.set(item.category, {
          category: item.category,
          categoryName: item.categoryName,
          color: item.color,
          backColor: item.backColor,
          icon: item.icon,
          variables: []
        });
      }

      map.get(item.category).variables.push(item);

    });

    this.groupedVariables = Array.from(map.values());
  }

  UpdateStatistics(): void {
    const editor = this.editor.nativeElement;
    this.variablesCount = editor.querySelectorAll('.var-chip').length;
    const clone = editor.cloneNode(true) as HTMLElement;
    clone.querySelectorAll('.var-chip').forEach(chip => {
      chip.remove();
    });

    this.charactersCount = (clone.textContent ?? '').length;
  }

  SaveSelection() {
    const selection = window.getSelection();
    if (!selection || selection.rangeCount === 0)
      return;

    const range = selection.getRangeAt(0);
    if (this.editor.nativeElement.contains(range.commonAncestorContainer)) {
      this.savedRange = range.cloneRange();
    }
  }

  PreserveSelectionForEmoji(event: MouseEvent): void {
    event.preventDefault();
    this.SaveSelection();
  }

  ToggleEmojiPicker(): void {
    if (this.isEmojiPickerOpen) {
      this.CloseEmojiPicker();
      return;
    }

    this.isEmojiPickerOpen = true;
    requestAnimationFrame(() => {
      this.AttachEmojiPickerToViewport();
      this.PositionEmojiPicker();
    });
  }

  InsertEmoji(emoji: string): void {
    const range = this.GetInsertionRange();
    range.deleteContents();
    const emojiNode = document.createTextNode(emoji);
    range.insertNode(emojiNode);
    range.setStartAfter(emojiNode);
    range.collapse(true);

    this.editor.nativeElement.focus();
    const selection = window.getSelection();
    selection?.removeAllRanges();
    selection?.addRange(range);
    this.savedRange = range.cloneRange();
    this.UpdateStatistics();
  }

  private GetInsertionRange(): Range {
    if (this.savedRange && this.editor.nativeElement.contains(this.savedRange.commonAncestorContainer))
      return this.savedRange.cloneRange();

    const range = document.createRange();
    range.selectNodeContents(this.editor.nativeElement);
    range.collapse(false);
    return range;
  }

  private PositionEmojiPicker(): void {
    if (!this.emojiTrigger || !this.emojiPopover)
      return;

    const margin = 12;
    const trigger = this.emojiTrigger.nativeElement.getBoundingClientRect();
    const popover = this.emojiPopover.nativeElement;
    const width = Math.min(352, window.innerWidth - margin * 2);
    const availableBelow = window.innerHeight - trigger.bottom - margin - 8;
    const height = Math.min(popover.offsetHeight || 435, Math.max(availableBelow, 180));
    const left = Math.min(Math.max(trigger.right - width, margin), window.innerWidth - width - margin);
    const top = Math.min(trigger.bottom + 8, window.innerHeight - height - margin);

    this.emojiPickerPosition = { top: `${top}px`, left: `${left}px`, width: `${width}px` };
    Object.assign(popover.style, {
      top: `${top}px`,
      left: `${left}px`,
      width: `${width}px`,
      maxHeight: `${Math.max(availableBelow, 180)}px`
    });
  }

  private AttachEmojiPickerToViewport(): void {
    const popover = this.emojiPopover?.nativeElement;
    if (popover && popover.parentElement !== document.body)
      document.body.appendChild(popover);
  }

  private CloseEmojiPicker(restoreEditorFocus = false): void {
    if (!this.isEmojiPickerOpen)
      return;

    const popover = this.emojiPopover?.nativeElement;
    this.isEmojiPickerOpen = false;
    popover?.remove();
    if (restoreEditorFocus)
      requestAnimationFrame(() => {
        this.editor.nativeElement.focus();
        const range = this.GetInsertionRange();
        const selection = window.getSelection();
        selection?.removeAllRanges();
        selection?.addRange(range);
      });
  }

  @HostListener('document:pointerdown', ['$event'])
  OnDocumentPointerDown(event: PointerEvent): void {
    if (!this.isEmojiPickerOpen)
      return;

    const target = event.target as Node;
    if (!this.emojiPopover?.nativeElement.contains(target) && !this.emojiTrigger?.nativeElement.contains(target))
      this.CloseEmojiPicker();
  }

  @HostListener('document:keydown.escape', ['$event'])
  OnEscape(event: KeyboardEvent): void {
    if (!this.isEmojiPickerOpen)
      return;

    event.preventDefault();
    this.CloseEmojiPicker(true);
  }

  @HostListener('window:resize')
  OnViewportResize(): void {
    if (this.isEmojiPickerOpen)
      this.PositionEmojiPicker();
  }

  ngOnDestroy(): void {
    this.emojiPopover?.nativeElement.remove();
  }

  insertVariable(variable: any) {
    const selection = window.getSelection();

    if (!selection || !this.savedRange)
      return;

    selection.removeAllRanges();
    selection.addRange(this.savedRange);

    const range = this.savedRange;
    const chip = this.CreateChip(variable);
    range.insertNode(chip);

    const space = document.createTextNode(" ");
    chip.after(space);

    range.setStartAfter(space);
    range.collapse(true);

    selection.removeAllRanges();
    selection.addRange(range);

    this.savedRange = range.cloneRange();
    this.UpdateStatistics();
  }

  EditorClick(event: MouseEvent) {
    const target = event.target as HTMLElement;
    const removeButton = target.closest(".chip-remove");

    if (!removeButton)
      return;

    event.preventDefault();
    event.stopPropagation();

    const chip = removeButton.closest(".var-chip");

    if (!chip)
      return;

    chip.remove();
    this.UpdateStatistics();
    this.editor.nativeElement.focus();
    this.SaveSelection();
  }

  OnEditorKeyDown(event: KeyboardEvent): void {
    if (event.key !== 'Enter')
      return;

    event.preventDefault();
    const selection = window.getSelection();
    if (!selection || selection.rangeCount === 0)
      return;

    const range = selection.getRangeAt(0);
    const br1 = document.createElement('br');
    const br2 = document.createElement('br');

    range.deleteContents();
    range.insertNode(br2);
    range.insertNode(br1);
    range.setStartAfter(br1);
    range.setEndAfter(br1);

    selection.removeAllRanges();
    selection.addRange(range);

    this.savedRange = range.cloneRange();
  }

  BuildEditor(template: string): void {
    const editor = this.editor.nativeElement;
    editor.innerHTML = "";
    const regex = /{{(.*?)}}/g;
    let lastIndex = 0;
    let match: RegExpExecArray | null;
    while ((match = regex.exec(template)) !== null) {
      const text = template.substring(lastIndex, match.index);
      text.split(/\r?\n/).forEach((line, index) => {
        if (index > 0) {
          editor.appendChild(document.createElement('br'));
        }

        editor.appendChild(document.createTextNode(line));
      });
      const key = match[1];
      const variable = this.TemplateVariables.find(x => x.displayKey === key);
      if (variable) {
        editor.appendChild(this.CreateChip(variable));
      }
      else {
        editor.appendChild(document.createTextNode(match[0]));
      }

      lastIndex = regex.lastIndex;
    }

    const remaining = template.substring(lastIndex);
    remaining.split(/\r?\n/).forEach((line, index) => {
      if (index > 0) {
        editor.appendChild(document.createElement('br'));
      }

      editor.appendChild(document.createTextNode(line));
    });
  }

  CreateChip(variable: any): HTMLElement {
    const chip = document.createElement("span");
    chip.className = "var-chip";
    chip.contentEditable = "false";
    chip.dataset["key"] = variable.displayKey;
    chip.style.backgroundColor = variable.backColor;
    chip.style.color = variable.color;

    const remove = document.createElement("span");
    remove.className = "chip-ic chip-remove";
    remove.contentEditable = "false";
    remove.style.backgroundColor = variable.color;
    remove.style.cursor = "pointer";
    remove.innerHTML = `<i class="fa-solid fa-xmark"></i>`;

    const text = document.createElement("span");
    text.innerText = variable.displayName;
    chip.appendChild(remove);
    chip.appendChild(text);

    return chip;
  }

  ExtractTemplate(): string {
    const clone = this.editor.nativeElement.cloneNode(true) as HTMLElement;
    clone.querySelectorAll(".var-chip").forEach(chip => {
      const key = chip.getAttribute("data-key");
      chip.replaceWith(document.createTextNode(`{{${key}}}`));
    });

    clone.querySelectorAll("br").forEach(br => {
      br.replaceWith(document.createTextNode("\n"));
    });

    return clone.textContent ?? "";
  }

  PreviewHtml() {
    let template = this.ExtractTemplate();
    this.TemplateVariables.forEach(variable => {
      const regex = new RegExp(`{{${variable.displayKey}}}`, 'g');
      template = template.replace(regex, variable.defaultValue ?? variable.displayName
      );
    });

    this.previewHtml = template.replace(/\n/g, '<br>');
    this.previewTime = this.datePipe.transform(new Date(), 'hh:mm');
    this.PreviewTemp = true;
  }

  GetVariableIds(): number[] {
    const ids: number[] = [];

    this.editor.nativeElement.querySelectorAll(".var-chip").forEach(chip => {
      const key = chip.getAttribute("data-key");
      const variable = this.TemplateVariables.find(x => x.displayKey == key);
      if (variable && !ids.includes(variable.id))
        ids.push(variable.id);
    });

    return ids;
  }

  validateForm(): boolean {
    this.formService.markFormGroupTouched(this.ItemForm);
    if (this.ItemForm.valid) {
      return true;
    } else {
      this.formErrors = this.formService.validateForm(this.ItemForm, this.formErrors, false)
      return false;
    }
  }

  SaveTemplate() {
    this.ItemForm = this.formService.TrimFormInputValue(this.ItemForm);
    let isValid = this.validateForm();

    if (!isValid) {
      return;
    }

    let temp = this.ExtractTemplate();

    if (!temp) {
      this.toaster.warning('برجاء ملئ محتوى للقالب');
      return;
    }

    let ids = this.GetVariableIds();
    this.ItemForm.patchValue({ body: temp, variableIds: ids, insertUser: this.authService.userId });
    if (!this.TemplateId) {
      this.parentService.AddNewTemplate(this.ItemForm.value).subscribe(data => {
        if (data.isSuccess) {
          this.toaster.success(data.message);
          this.RefreshData.emit(true);
          this.modalService.dismissAll();
        }
        else
          this.toaster.error(data.message);
        this.showLoader = false;
      });
    } else {
      this.ItemForm.patchValue({ id: this.TemplateId });
      this.parentService.UpdateTemplate(this.ItemForm.value).subscribe(data => {
        if (data.isSuccess) {
          this.toaster.success(data.message);
          this.RefreshData.emit(true);
          this.modalService.dismissAll();
        }
        else
          this.toaster.error(data.message);
        this.showLoader = false;
      });
    }
  }

  CloseModal() {
    this.modalService.dismissAll();
  }

}
