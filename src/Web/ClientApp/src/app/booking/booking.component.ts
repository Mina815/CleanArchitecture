import { Component, OnInit, OnDestroy, ChangeDetectorRef } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { BookingsClient, CentersClient, BranchesClient, ServicesClient, BranchDto, ServiceDto, TimeSlotDto, CenterDetailDto, CreateBookingCommand } from '../web-api-client';

@Component({
  standalone: false,
  selector: 'app-booking',
  templateUrl: './booking.component.html'
})
export class BookingComponent implements OnInit, OnDestroy {
  private destroy$ = new Subject<void>();

  currentStep = 1;
  center: CenterDetailDto | null = null;
  branches: BranchDto[] = [];
  services: ServiceDto[] = [];
  selectedBranch: BranchDto | null = null;
  selectedService: ServiceDto | null = null;
  selectedDate = '';
  selectedTime = '';
  availableSlots: TimeSlotDto[] = [];
  timeSlots: TimeSlotDto[] = [];
  customerNotes = '';
  loading = false;
  submitting = false;
  error = '';

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private bookingsClient: BookingsClient,
    private centersClient: CentersClient,
    private branchesClient: BranchesClient,
    private servicesClient: ServicesClient,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    const centerId = Number(this.route.snapshot.paramMap.get('centerId'));
    this.selectedDate = this.todayString();
    this.loading = true;
    this.cdr.detectChanges();

    this.centersClient.getCenterById(centerId).pipe(takeUntil(this.destroy$)).subscribe({
      next: center => { this.center = center; this.loading = false; this.cdr.detectChanges(); },
      error: () => { this.loading = false; this.error = 'Failed to load center details.'; this.cdr.detectChanges(); }
    });

    this.branchesClient.getBranches(centerId).pipe(takeUntil(this.destroy$)).subscribe({
      next: branches => { this.branches = branches; this.cdr.detectChanges(); },
      error: () => { this.error = 'Failed to load branches.'; this.cdr.detectChanges(); }
    });

    this.servicesClient.getServices(centerId).pipe(takeUntil(this.destroy$)).subscribe({
      next: services => { this.services = services; this.cdr.detectChanges(); },
      error: () => { this.error = 'Failed to load services.'; this.cdr.detectChanges(); }
    });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  selectBranch(branch: BranchDto): void {
    this.selectedBranch = branch;
    this.currentStep = 2;
    this.error = '';
  }

  changeStep(step: number): void {
    this.currentStep = step;
    this.error = '';
    if (step <= 2) { this.selectedService = null; this.selectedDate = this.todayString(); this.selectedTime = ''; this.timeSlots = []; }
    if (step <= 3) { this.selectedTime = ''; this.timeSlots = []; }
  }

  selectService(service: ServiceDto): void {
    this.selectedService = service;
    this.currentStep = 3;
    this.error = '';
    this.loadAvailableSlots();
  }

  selectDate(date: string): void {
    this.selectedDate = date;
    this.loadAvailableSlots();
  }

  selectTime(slot: TimeSlotDto): void {
    this.selectedTime = slot.startTime ?? '';
    this.currentStep = 4;
    this.error = '';
  }

  loadAvailableSlots(): void {
    if (!this.selectedBranch?.id || !this.selectedService?.id || !this.selectedDate) return;
    this.loading = true;
    this.error = '';
    this.cdr.detectChanges();
    this.bookingsClient.getAvailableSlots(
      this.selectedBranch.id,
      this.selectedService.id,
      new Date(this.selectedDate),
      undefined
    ).pipe(takeUntil(this.destroy$)).subscribe({
      next: slots => {
        this.availableSlots = slots;
        this.timeSlots = slots;
        this.loading = false;
        this.cdr.detectChanges();
      },
      error: () => {
        this.loading = false;
        this.error = 'Failed to load available slots.';
        this.cdr.detectChanges();
      }
    });
  }

  confirmBooking(): void {
    if (!this.selectedBranch?.id || !this.selectedService?.id || !this.selectedDate || !this.selectedTime) return;
    this.submitting = true;
    this.error = '';

    const command = new CreateBookingCommand({
      centerId: this.center?.id,
      branchId: this.selectedBranch.id,
      serviceId: this.selectedService.id,
      bookingDate: new Date(this.selectedDate),
      startTime: this.selectedTime,
      customerNotes: this.customerNotes || undefined
    });

    this.bookingsClient.createBooking(command).pipe(takeUntil(this.destroy$)).subscribe({
      next: () => {
        this.submitting = false;
        this.router.navigate(['/my-bookings']);
      },
      error: () => {
        this.submitting = false;
        this.error = 'Failed to create booking. Please try again.';
        this.cdr.detectChanges();
      }
    });
  }

  back(): void {
    if (this.currentStep > 1) {
      this.currentStep--;
      this.error = '';
    }
  }

  todayString(): string {
    return new Date().toISOString().split('T')[0];
  }

  getAvailableDates(): { date: string; dayName: string; dayNumber: number; month: string }[] {
    const dayNames = ['Sun', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat'];
    const monthNames = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];
    const result = [];
    for (let i = 0; i < 7; i++) {
      const d = new Date();
      d.setDate(d.getDate() + i);
      result.push({
        date: d.toISOString().split('T')[0],
        dayName: dayNames[d.getDay()],
        dayNumber: d.getDate(),
        month: monthNames[d.getMonth()]
      });
    }
    return result;
  }
}
