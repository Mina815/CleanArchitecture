import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { CenterDetailDto, JamalekApiService, StaffMemberDto } from '../services/jamalek-api.service';
import { JamalekAuthService } from 'src/api-authorization/jamalek-auth.service';

@Component({
  standalone: false,
  selector: 'app-center-detail',
  templateUrl: './center-detail.component.html'
})
export class CenterDetailComponent implements OnInit {
  center?: CenterDetailDto;
  selectedBranchId?: number;
  selectedServiceId?: number;
  selectedStaffId?: number;
  staff: StaffMemberDto[] = [];
  bookingDate = '';
  slots: { startTime: string; endTime: string }[] = [];
  selectedSlot?: string;
  notes = '';
  message = '';
  error = '';
  loadingSlots = false;

  constructor(
    private route: ActivatedRoute,
    private api: JamalekApiService,
    public auth: JamalekAuthService
  ) {}

  ngOnInit(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    this.api.getCenter(id).subscribe({
      next: c => {
        this.center = c;
        this.selectedBranchId = c.branches[0]?.id;
        this.selectedServiceId = c.services[0]?.id;
        this.bookingDate = this.minDate();
        this.onBranchChange();
      },
      error: () => this.error = 'Center not found.'
    });
  }

  minDate(): string {
    return new Date(Date.now() + 86400000).toISOString().slice(0, 10);
  }

  onBranchChange(): void {
    this.slots = [];
    this.selectedSlot = undefined;
    if (this.selectedBranchId) {
      this.api.getBranchStaff(this.selectedBranchId).subscribe(s => this.staff = s);
    }
  }

  loadSlots(): void {
    if (!this.selectedBranchId || !this.selectedServiceId || !this.bookingDate) return;
    this.loadingSlots = true;
    this.api.getAvailability(this.selectedBranchId, this.selectedServiceId, this.bookingDate, this.selectedStaffId)
      .subscribe({
        next: slots => { this.slots = slots; this.loadingSlots = false; },
        error: () => { this.loadingSlots = false; this.error = 'Could not load availability.'; }
      });
  }

  selectedService() {
    return this.center?.services.find(s => s.id === Number(this.selectedServiceId));
  }

  book(): void {
    if (!this.auth.isAuthenticated) {
      this.message = '';
      this.error = 'Please log in to book an appointment.';
      return;
    }
    if (!this.selectedBranchId || !this.selectedServiceId || !this.selectedSlot) return;

    this.error = '';
    this.api.createBooking({
      branchId: Number(this.selectedBranchId),
      serviceId: Number(this.selectedServiceId),
      staffId: this.selectedStaffId ? Number(this.selectedStaffId) : undefined,
      bookingDate: this.bookingDate,
      startTime: this.selectedSlot.length > 5 ? this.selectedSlot.slice(0, 5) : this.selectedSlot,
      customerNotes: this.notes,
      paymentProvider: 0
    }).subscribe({
      next: () => {
        this.message = 'Booking submitted! The salon will confirm your appointment shortly.';
        this.selectedSlot = undefined;
      },
      error: () => this.error = 'Booking failed. The slot may no longer be available.'
    });
  }
}
