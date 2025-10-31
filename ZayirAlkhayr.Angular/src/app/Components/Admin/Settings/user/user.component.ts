import { Component, Injector, OnInit } from '@angular/core';
import { ZaBreadcrumbComponent } from "../../../../Shared/za-breadcrumb/za-breadcrumb.component";
import { ZaPaginationComponent } from "../../../../Shared/za-pagination/za-pagination.component";
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { NgbModal, NgbModule, NgbOffcanvas } from '@ng-bootstrap/ng-bootstrap';
import { SettingService } from '../../../../Services/settings/setting.service';
import { FormService } from '../../../../Services/shared/form.service';
import { ToastrService } from 'ngx-toastr';
import { ZaEmptyDataComponent } from '../../../../Shared/za-empty-data/za-empty-data.component';
import { NgFor, NgIf } from '@angular/common';
import { ZaInputWithLabelComponent } from '../../../../Shared/za-input-with-label/za-input-with-label.component';
import { CustomValidators, RegexType } from '../../../../Services/shared/custom-validators';
import { PagePermissionComponent } from "../page-permission/page-permission.component";
import { NgxLoadingModule } from "ngx-loading";

@Component({
  selector: 'app-user',
  standalone: true,
  imports: [ZaBreadcrumbComponent, ZaPaginationComponent, ZaEmptyDataComponent, NgbModule,
    NgIf, NgFor, ZaInputWithLabelComponent, ReactiveFormsModule, NgxLoadingModule],
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
    { value: 'SupperAdmin', name: 'مدير' }
  ];
  ManagerUserId = '321db4e1-e32b-4aeb-8802-b076f9d7227d';
  TotalCount = 0;
  formErrors = {
    userName: '',
    email: '',
    password: '',
    phoneNumber: '',
    address: ''
  };

  constructor(private modalService: NgbModal, private settingService: SettingService, private formService: FormService
    , private fb: FormBuilder, private toaster: ToastrService,private offcanvasService: NgbOffcanvas, private injector: Injector) {

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
      address: ['', Validators.required]
    });

    this.ItemForm.valueChanges.subscribe((data) => {
      this.formErrors = this.formService.validateForm(this.ItemForm, this.formErrors, true);
    });
  }

  FillEditForm(item: any) {
    this.UserId = item.userId;
    this.ItemForm.setValue({
      userId: item.userId,
      userName: item.userName,
      email: item.email,
      password: null,
      phoneNumber: item.phoneNumber,
      address: item?.address
    });
  }

  ResetForm() {
    this.ItemForm.reset();
    this.ItemForm.get('userId').setValue(0);
  }

  OpenUserPermissionSidePanel(item: any) {
    const injector = Injector.create({
      providers: [
        { provide: 'UserId', useValue: item.userId },
        { provide: 'UserName', useValue: item.userName },
        { provide: 'RoleName', useValue: item.roleNameAr }
      ],
      parent: this.injector
    });

    const ref = this.offcanvasService.open(PagePermissionComponent, {
      injector: injector,
      position: 'end'
    });
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
    this.showLoader = true;
    this.settingService.GetAllUsers().subscribe(data => {
      this.showLoader = false;
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

    this.showLoader = true;
    if (!this.ItemForm.controls['userId'].value) {
      this.settingService.CreateUser(this.ItemForm.value).subscribe(data => {
        this.showLoader = false;
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
        this.showLoader = false;
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

    this.showLoader = true;
    this.settingService.DeleteUser(this.UserId).subscribe(data => {
      this.showLoader = false;
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
