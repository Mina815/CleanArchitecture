import { Component, OnInit, OnDestroy, ChangeDetectorRef } from '@angular/core';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { BranchesClient, CentersClient, BranchDto, CreateBranchCommand, UpdateBranchCommand, CenterDto, WorkingHourDto, TimeOffDto, SetWorkingHoursCommand, CreateTimeOffCommand } from '../web-api-client';

@Component({
  standalone: false,
  selector: 'app-branch-management',
  templateUrl: './branch-management.component.html',
  styleUrls: ['./branch-management.component.scss']
})
export class BranchManagementComponent implements OnInit, OnDestroy {
  private destroy$ = new Subject<void>();

  centers: CenterDto[] = [];
  selectedCenterId: number | null = null;
  branches: BranchDto[] = [];
  loading = false;
  saving = false;

  showForm = false;
  editBranch: BranchDto | null = null;

  formCenterId = 0;
  formName = '';
  formNameAr = '';
  formAddress = '';
  formCity = '';
  formDistrict = '';
  formPhone = '';
  formWhatsapp = '';
  formLatitude: number | undefined;
  formLongitude: number | undefined;

  selectedBranch: BranchDto | null = null;
  manageSection: 'hours' | 'timeoff' | null = null;

  workingHours: WorkingHourDto[] = [];
  hoursLoading = false;
  hoursSaving = false;

  timeOffs: TimeOffDto[] = [];
  timeOffsLoading = false;
  showTimeOffForm = false;
  timeOffFromDate = '';
  timeOffToDate = '';
  timeOffFromTime = '';
  timeOffToTime = '';
  timeOffReason = '';
  timeOffType = 0;
  timeOffSaving = false;

  dayNames = ['الأحد', 'الإثنين', 'الثلاثاء', 'الأربعاء', 'الخميس', 'الجمعة', 'السبت'];

  constructor(
    private branchesClient: BranchesClient,
    private centersClient: CentersClient,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.loadCenters();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  loadCenters(): void {
    this.centersClient.getCenters(undefined, undefined, 1, 100).pipe(takeUntil(this.destroy$)).subscribe({
      next: result => {
        this.centers = result.items ?? [];
        this.cdr.detectChanges();
      }
    });
  }

  selectCenter(centerId: number): void {
    this.selectedCenterId = centerId;
    this.branches = [];
    this.showForm = false;
    this.editBranch = null;
    this.selectedBranch = null;
    this.manageSection = null;
    this.cdr.detectChanges();
    this.loadBranches(centerId);
  }

  loadBranches(centerId: number): void {
    this.loading = true;
    this.cdr.detectChanges();
    this.branchesClient.getBranches(centerId).pipe(takeUntil(this.destroy$)).subscribe({
      next: result => {
        this.branches = result ?? [];
        this.loading = false;
        this.cdr.detectChanges();
      },
      error: () => {
        this.loading = false;
        this.cdr.detectChanges();
      }
    });
  }

  openCreateForm(): void {
    this.editBranch = null;
    this.formCenterId = this.selectedCenterId ?? 0;
    this.formName = '';
    this.formNameAr = '';
    this.formAddress = '';
    this.formCity = '';
    this.formDistrict = '';
    this.formPhone = '';
    this.formWhatsapp = '';
    this.formLatitude = undefined;
    this.formLongitude = undefined;
    this.showForm = true;
  }

  openEditForm(branch: BranchDto): void {
    this.editBranch = branch;
    this.formCenterId = this.selectedCenterId ?? 0;
    this.formName = branch.name ?? '';
    this.formNameAr = branch.nameAr ?? '';
    this.formAddress = branch.address ?? '';
    this.formCity = branch.city ?? '';
    this.formDistrict = '';
    this.formPhone = branch.phone ?? '';
    this.formWhatsapp = '';
    this.formLatitude = undefined;
    this.formLongitude = undefined;
    this.showForm = true;
  }

  cancelForm(): void {
    this.showForm = false;
    this.editBranch = null;
  }

  saveBranch(): void {
    if (!this.formName || !this.formNameAr || !this.formAddress || !this.formCity) return;
    this.saving = true;
    this.cdr.detectChanges();

    if (this.editBranch) {
      const command = new UpdateBranchCommand({
        id: this.editBranch.id,
        name: this.formName || undefined,
        nameAr: this.formNameAr || undefined,
        address: this.formAddress || undefined,
        city: this.formCity || undefined,
        district: this.formDistrict || undefined,
        phone: this.formPhone || undefined,
        whatsappNumber: this.formWhatsapp || undefined,
        latitude: this.formLatitude,
        longitude: this.formLongitude
      });
      this.branchesClient.updateBranch(this.editBranch.id!, command).pipe(takeUntil(this.destroy$)).subscribe({
        next: () => {
          this.saving = false;
          this.showForm = false;
          this.editBranch = null;
          this.loadBranches(this.selectedCenterId!);
        },
        error: () => {
          this.saving = false;
          this.cdr.detectChanges();
        }
      });
    } else {
      const command = new CreateBranchCommand({
        centerId: this.formCenterId,
        name: this.formName,
        nameAr: this.formNameAr,
        address: this.formAddress,
        city: this.formCity,
        district: this.formDistrict || undefined,
        phone: this.formPhone || undefined,
        whatsappNumber: this.formWhatsapp || undefined,
        latitude: this.formLatitude,
        longitude: this.formLongitude
      });
      this.branchesClient.createBranch(command).pipe(takeUntil(this.destroy$)).subscribe({
        next: () => {
          this.saving = false;
          this.showForm = false;
          this.loadBranches(this.selectedCenterId!);
        },
        error: () => {
          this.saving = false;
          this.cdr.detectChanges();
        }
      });
    }
  }

  toggleActive(branch: BranchDto): void {
    const command = new UpdateBranchCommand({
      id: branch.id,
      isActive: !branch.isActive
    });
    this.branchesClient.updateBranch(branch.id!, command).pipe(takeUntil(this.destroy$)).subscribe({
      next: () => this.loadBranches(this.selectedCenterId!)
    });
  }

  selectBranch(branch: BranchDto, section: 'hours' | 'timeoff'): void {
    this.selectedBranch = branch;
    this.manageSection = section;
    this.showForm = false;
    this.cdr.detectChanges();
    if (section === 'hours') {
      this.loadWorkingHours(branch.id!);
    } else {
      this.loadTimeOffs(branch.id!);
    }
  }

  closeManagement(): void {
    this.selectedBranch = null;
    this.manageSection = null;
  }

  loadWorkingHours(branchId: number): void {
    this.hoursLoading = true;
    this.cdr.detectChanges();
    this.branchesClient.getBranchWorkingHoursEndpoint(branchId).pipe(takeUntil(this.destroy$)).subscribe({
      next: result => {
        if (result && result.length > 0) {
          this.workingHours = result;
        } else {
          this.workingHours = Array.from({ length: 7 }, (_, i) => {
            const wh = new WorkingHourDto();
            wh.dayOfWeek = i;
            wh.openTime = '09:00:00';
            wh.closeTime = '18:00:00';
            wh.isClosed = i === 5;
            return wh;
          });
        }
        this.hoursLoading = false;
        this.cdr.detectChanges();
      },
      error: () => {
        this.hoursLoading = false;
        this.cdr.detectChanges();
      }
    });
  }

  saveWorkingHours(): void {
    if (!this.selectedBranch?.id) return;
    this.hoursSaving = true;
    this.cdr.detectChanges();
    const command = new SetWorkingHoursCommand({
      branchId: this.selectedBranch.id,
      workingHours: this.workingHours
    });
    this.branchesClient.setBranchWorkingHours(this.selectedBranch.id, command).pipe(takeUntil(this.destroy$)).subscribe({
      next: () => {
        this.hoursSaving = false;
        this.cdr.detectChanges();
      },
      error: () => {
        this.hoursSaving = false;
        this.cdr.detectChanges();
      }
    });
  }

  loadTimeOffs(branchId: number): void {
    this.timeOffsLoading = true;
    this.cdr.detectChanges();
    this.branchesClient.getBranchTimeOffsEndpoint(branchId).pipe(takeUntil(this.destroy$)).subscribe({
      next: result => {
        this.timeOffs = result ?? [];
        this.timeOffsLoading = false;
        this.cdr.detectChanges();
      },
      error: () => {
        this.timeOffsLoading = false;
        this.cdr.detectChanges();
      }
    });
  }

  openTimeOffForm(): void {
    this.showTimeOffForm = true;
    this.timeOffFromDate = '';
    this.timeOffToDate = '';
    this.timeOffFromTime = '';
    this.timeOffToTime = '';
    this.timeOffReason = '';
    this.timeOffType = 0;
  }

  cancelTimeOffForm(): void {
    this.showTimeOffForm = false;
  }

  saveTimeOff(): void {
    if (!this.selectedBranch?.id || !this.timeOffFromDate) return;
    this.timeOffSaving = true;
    this.cdr.detectChanges();
    const command = new CreateTimeOffCommand({
      branchId: this.selectedBranch.id,
      fromDate: new Date(this.timeOffFromDate),
      toDate: this.timeOffToDate ? new Date(this.timeOffToDate) : new Date(this.timeOffFromDate),
      fromTime: this.timeOffFromTime || undefined,
      toTime: this.timeOffToTime || undefined,
      reason: this.timeOffReason || undefined,
      type: this.timeOffType
    });
    this.branchesClient.createBranchTimeOff(this.selectedBranch.id, command).pipe(takeUntil(this.destroy$)).subscribe({
      next: () => {
        this.timeOffSaving = false;
        this.showTimeOffForm = false;
        this.loadTimeOffs(this.selectedBranch!.id!);
      },
      error: () => {
        this.timeOffSaving = false;
        this.cdr.detectChanges();
      }
    });
  }
}
