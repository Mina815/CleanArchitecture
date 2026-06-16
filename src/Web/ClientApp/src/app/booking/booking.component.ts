import { Component, OnInit, inject } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { BookingsClient, CentersClient, BranchesClient, ServicesClient, BranchDto, ServiceDto, TimeSlotDto, CenterDetailDto, CreateBookingCommand } from '../web-api-client';

@Component({
  standalone: false,
  selector: 'app-booking',
  templateUrl: './booking.component.html'
})
export class BookingComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private bookingsClient = inject(BookingsClient);
  private centersClient = inject(CentersClient);
  private branchesClient = inject(BranchesClient);
  private servicesClient = inject(ServicesClient);

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

  ngOnInit(): void {
    const centerId = Number(this.route.snapshot.paramMap.get('centerId'));
    this.selectedDate = this.todayString();
    this.loading = true;

    this.centersClient.getCenterById(centerId).subscribe({
      next: center => { this.center = center; this.loading = false; },
      error: () => { this.loading = false; this.error = 'Failed to load center details.'; }
    });

    this.branchesClient.getBranches(centerId).subscribe({
      next: branches => { this.branches = branches; },
      error: () => { this.error = 'Failed to load branches.'; }
    });

    this.servicesClient.getServices(centerId).subscribe({
      next: services => { this.services = services; },
      error: () => { this.error = 'Failed to load services.'; }
    });
  }

  selectBranch(branch: BranchDto): void {
    this.selectedBranch = branch;
    this.currentStep = 2;
    this.error = '';
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
    this.bookingsClient.getAvailableSlots(
      this.selectedBranch.id,
      this.selectedService.id,
      new Date(this.selectedDate),
      undefined
    ).subscribe({
      next: slots => {
        this.availableSlots = slots;
        this.timeSlots = slots;
        this.loading = false;
      },
      error: () => {
        this.loading = false;
        this.error = 'Failed to load available slots.';
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

    this.bookingsClient.createBooking(command).subscribe({
      next: () => {
        this.submitting = false;
        this.router.navigate(['/my-bookings']);
      },
      error: () => {
        this.submitting = false;
        this.error = 'Failed to create booking. Please try again.';
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
