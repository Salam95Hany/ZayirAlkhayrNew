import { Component, OnInit } from '@angular/core';
import { ZaBreadcrumbComponent } from "../../../../Shared/za-breadcrumb/za-breadcrumb.component";
import { ZaPaginationComponent } from "../../../../Shared/za-pagination/za-pagination.component";
import { ZaFiltersComponent } from "../../../../Shared/za-filters/za-filters.component";
import { ZaLoaderComponent } from "../../../../Shared/za-loader/za-loader.component";
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { NgbModal, NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { SettingService } from '../../../../Services/settings/setting.service';
import { FormService } from '../../../../Services/shared/form.service';
import { ToastrService } from 'ngx-toastr';
import { ZaEmptyDataComponent } from '../../../../Shared/za-empty-data/za-empty-data.component';
import { NgFor, NgIf } from '@angular/common';
import { ZaInputWithLabelComponent } from '../../../../Shared/za-input-with-label/za-input-with-label.component';
import { ZaDropDownFormControlComponent } from '../../../../Shared/za-drop-down-form-control/za-drop-down-form-control.component';
import { CustomValidators, RegexType } from '../../../../Services/shared/custom-validators';

@Component({
  selector: 'app-user',
  standalone: true,
  imports: [ZaLoaderComponent, ZaBreadcrumbComponent, ZaPaginationComponent, ZaEmptyDataComponent, NgbModule,
    NgIf, NgFor, ZaInputWithLabelComponent, ReactiveFormsModule, ZaDropDownFormControlComponent],
  templateUrl: './user.component.html',
  styleUrl: './user.component.css'
})
export class UserComponent implements OnInit {
  TitleList = ['الاعدادات', 'المستخدمين'];
  UsersData: any[] = [];
  showLoader = false;
  ItemForm: FormGroup;
  UserId: any;
  Roles = [
    { value: 'SupperAdmin', name: 'مدير' },
    { value: 'WebSite', name: 'موقع زائر الخير' },
    { value: 'Services', name: 'خدمات اجتماعية' },
    { value: 'BeneFactors', name: 'متبرعين' },
    { value: 'Accounts', name: 'حسابات' },
    { value: 'Admin', name: 'مشرف' }
  ];
  ManagerUserId = '321db4e1-e32b-4aeb-8802-b076f9d7227d';
  TotalCount = 0;
  formErrors = {
    userName: '',
    email: '',
    password: '',
    phoneNumber: '',
    address: '',
    role: ''
  };

  constructor(private modalService: NgbModal, private settingService: SettingService, private formService: FormService
    , private fb: FormBuilder, private toaster: ToastrService) {

  }

  ngOnInit(): void {
    this.FormInit();
    this.GetAllUsers();
  }

  FormInit() {
    this.ItemForm = this.fb.group({
      userId: null,
      userName: ['', [Validators.required, CustomValidators.regexPattern(RegexType.englishLettersOnly), CustomValidators.regexPattern(RegexType.noSpace)]],
      email: ['', [Validators.required, CustomValidators.regexPattern(RegexType.email)]],
      password: ['', [Validators.required, CustomValidators.regexPattern(RegexType.FourMinLength), CustomValidators.regexPattern(RegexType.noSpace)]],
      phoneNumber: ['', Validators.required],
      address: ['', Validators.required],
      role: ['', Validators.required]
    });

    this.ItemForm.valueChanges.subscribe((data) => {
      this.formErrors = this.formService.validateForm(this.ItemForm, this.formErrors, true);
    });
  }

  FillEditForm(item: any) {
    debugger;
    this.UserId = item.userId;
    this.ItemForm.setValue({
      userId: item.userId,
      userName: item.userName,
      email: item.email,
      password: null,
      phoneNumber: item.phoneNumber,
      address: item?.address,
      role: item.role
    });
  }

  ResetForm() {
    this.ItemForm.reset();
    this.ItemForm.get('userId').setValue(0);
  }

  openItemModal(content: any, item: any) {
    this.formService.updateFieldsRequiredValidation(this.ItemForm, 'password', true);
    this.ResetForm();
    if (item) {
      this.FillEditForm(item);
      this.formService.updateFieldsRequiredValidation(this.ItemForm, 'password', false);
    }

    this.modalService.open(content, {
      size: 'xl',
      scrollable: true,
      centered: true
    });
  }

  openDeleteItemModal(content: any, userId: any) {
    this.UserId = userId;
    this.modalService.open(content, {
      size: 'md',
      scrollable: true,
      centered: true
    });
  }

  GetAllUsers() {
    this.settingService.GetAllUsers().subscribe(data => {
      this.UsersData = data.results;
      this.UsersData.forEach(item => {
        let role = this.Roles.find(i => i.value == item.role);
        if (role)
          item.roleNameAr = role.name;
      });
      this.TotalCount = this.UsersData.length;
    })
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

  AddNewItem() {
    let isValid = this.validateForm();
    if (!isValid) {
      return;
    }

    if (!this.ItemForm.controls['userId'].value) {
      this.settingService.CreateUser(this.ItemForm.value).subscribe(data => {
        if (data.isSuccess) {
          this.toaster.success(data.message);
          this.GetAllUsers();
          this.modalService.dismissAll();
        }
        else
          this.toaster.error(data.message);
      });
    } else {
      if (this.ManagerUserId == this.UserId) {
        this.toaster.warning('لا يمكن التعديل على هذا المستخدم');
        return;
      }
      this.settingService.EditUser(this.ItemForm.value).subscribe(data => {
        if (data.isSuccess) {
          this.toaster.success(data.message);
          this.GetAllUsers();
          this.modalService.dismissAll();
        }
        else
          this.toaster.error(data.message);
      });
    }
  }

  DeleteItem() {
    if (this.ManagerUserId == this.UserId) {
      this.toaster.warning('لا يمكن حذف هذا المستخدم');
      return;
    }
    this.settingService.DeleteUser(this.UserId).subscribe(data => {
      if (data.isSuccess) {
        this.toaster.success(data.message);
        this.GetAllUsers();
        this.modalService.dismissAll();
      }
      else
        this.toaster.error(data.message);
    });
  }
}
