import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { CenterDetailDto, JamalekApiService, StaffDto } from '../services/jamalek-api.service';
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
  staff: StaffDto[] = [];
  bookingDate = '';
  slots: { startTime: string; endTime: string }[] = [];
  selectedSlot?: string;
  notes = '';
  message = '';
  error = '';
  loading = false;
  loadingSlots = false;
  private loadingTimeout?: ReturnType<typeof setTimeout>;

  constructor(
    private route: ActivatedRoute,
    private api: JamalekApiService,
    public auth: JamalekAuthService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.loading = true;
    this.cdr.detectChanges();
    this.loadingTimeout = setTimeout(() => { this.loading = false; this.error = 'Server not responding. Try again.'; this.cdr.detectChanges(); }, 15000);
    const id = Number(this.route.snapshot.paramMap.get('id'));
    this.api.getCenter(id).subscribe({
      next: c => {
        clearTimeout(this.loadingTimeout);
        this.loading = false;
        this.center = c;
        this.selectedBranchId = c.branches[0]?.id;
        this.selectedServiceId = c.services[0]?.id;
        this.bookingDate = this.minDate();
        this.cdr.detectChanges();
        this.onBranchChange();
      },
      error: () => { clearTimeout(this.loadingTimeout); this.loading = false; this.error = 'Center not found.'; this.cdr.detectChanges(); }
    });
  }

  minDate(): string {
    return new Date(Date.now() + 86400000).toISOString().slice(0, 10);
  }

  onBranchChange(): void {
    this.slots = [];
    this.selectedSlot = undefined;
    if (this.selectedBranchId) {
      this.api.getBranchStaff(this.selectedBranchId).subscribe({
        next: s => { this.staff = s; this.cdr.detectChanges(); },
        error: () => { this.staff = []; this.cdr.detectChanges(); }
      });
    }
  }

  loadSlots(): void {
    if (!this.selectedBranchId || !this.selectedServiceId || !this.bookingDate) return;
    this.loadingSlots = true;
    this.cdr.detectChanges();
    this.api.getAvailability(this.selectedBranchId, this.selectedServiceId, this.bookingDate, this.selectedStaffId)
      .subscribe({
        next: slots => { this.slots = slots; this.loadingSlots = false; this.cdr.detectChanges(); },
        error: () => { this.loadingSlots = false; this.error = 'Could not load availability.'; this.cdr.detectChanges(); }
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
