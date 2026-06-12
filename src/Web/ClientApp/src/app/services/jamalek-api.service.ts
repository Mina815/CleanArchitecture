import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface CenterDto {
  id: number;
  name: string;
  nameAr: string;
  description?: string;
  logoUrl?: string;
  averageRating: number;
  totalReviews: number;
  city?: string;
}

export interface CenterDetailDto extends CenterDto {
  branches: BranchSummaryDto[];
  services: ServiceSummaryDto[];
  images: string[];
}

export interface BranchSummaryDto {
  id: number;
  name: string;
  nameAr: string;
  address: string;
  city: string;
  phone: string;
}

export interface CreateBranchRequest {
  centerId: number;
  name: string;
  nameAr: string;
  address: string;
  city: string;
  district?: string;
  latitude?: number;
  longitude?: number;
  phone: string;
  whatsappNumber?: string;
}

export interface UpdateBranchRequest {
  id: number;
  name: string;
  nameAr: string;
  address: string;
  city: string;
  district?: string;
  phone: string;
  whatsappNumber?: string;
  isActive: boolean;
}

export interface WorkingHourDto {
  dayOfWeek: number;
  openTime: string;
  closeTime: string;
  isClosed: boolean;
}

export interface CreateTimeOffRequest {
  staffId?: number;
  fromDate: string;
  toDate: string;
  fromTime?: string;
  toTime?: string;
  reason?: string;
  type: number;
}

export enum TimeOffType {
  Holiday = 0,
  Vacation = 1,
  Sick = 2,
  Custom = 3
}

export interface ServiceSummaryDto {
  id: number;
  name: string;
  price: number;
  durationMinutes: number;
}

export interface StaffMemberDto {
  id: number;
  name: string;
  specialization?: string;
}

export interface BookingDto {
  id: number;
  centerName?: string;
  branchName?: string;
  serviceName?: string;
  bookingDate: string;
  startTime: string;
  endTime: string;
  status: string;
  totalAmount: number;
}

export interface TimeSlotDto {
  startTime: string;
  endTime: string;
  isAvailable: boolean;
}

export interface NotificationDto {
  id: number;
  title: string;
  message: string;
  isRead: boolean;
  type?: string;
  createdAt?: string;
}

export interface ReviewDto {
  id: number;
  rating: number;
  comment?: string;
  created: string;
}

@Injectable({ providedIn: 'root' })
export class JamalekApiService {
  constructor(private http: HttpClient) {}

  getCenters(city?: string, search?: string): Observable<CenterDto[]> {
    let params = new HttpParams();
    if (city) params = params.set('city', city);
    if (search) params = params.set('search', search);
    return this.http.get<CenterDto[]>('/api/centers', { params });
  }

  getCenter(id: number): Observable<CenterDetailDto> {
    return this.http.get<CenterDetailDto>(`/api/centers/${id}`);
  }

  getCenterReviews(centerId: number, page = 1): Observable<{ items: ReviewDto[]; totalCount: number }> {
    return this.http.get<{ items: ReviewDto[]; totalCount: number }>(`/api/reviews/center/${centerId}`, {
      params: { page: page.toString(), pageSize: '10' }
    });
  }

  getAvailability(branchId: number, serviceId: number, date: string, staffId?: number): Observable<TimeSlotDto[]> {
    let params = new HttpParams()
      .set('branchId', branchId)
      .set('serviceId', serviceId)
      .set('date', date);
    if (staffId) params = params.set('staffId', staffId);
    return this.http.get<TimeSlotDto[]>('/api/bookings/availability', { params });
  }

  createBooking(data: {
    branchId: number; serviceId: number; staffId?: number;
    bookingDate: string; startTime: string; customerNotes?: string; paymentProvider?: number;
  }): Observable<number> {
    return this.http.post<number>('/api/bookings', data);
  }

  getMyBookings(): Observable<BookingDto[]> {
    return this.http.get<BookingDto[]>('/api/bookings/my-bookings');
  }

  cancelBooking(id: number, reason?: string): Observable<void> {
    return this.http.put<void>(`/api/bookings/${id}/cancel`, { reason });
  }

  getBranchTodayBookings(branchId: number): Observable<BookingDto[]> {
    return this.http.get<BookingDto[]>(`/api/bookings/branch/${branchId}/today`);
  }

  confirmBooking(id: number): Observable<void> {
    return this.http.put<void>(`/api/bookings/${id}/confirm`, {});
  }

  completeBooking(id: number): Observable<void> {
    return this.http.put<void>(`/api/bookings/${id}/complete`, {});
  }

  getBranchStaff(branchId: number): Observable<StaffMemberDto[]> {
    return this.http.get<StaffMemberDto[]>(`/api/staff/branch/${branchId}`);
  }

  getNotifications(unreadOnly = false): Observable<NotificationDto[]> {
    return this.http.get<NotificationDto[]>('/api/notifications', { params: { unreadOnly } });
  }

  markNotificationRead(id: number): Observable<void> {
    return this.http.put<void>(`/api/notifications/${id}/mark-read`, {});
  }

  markAllNotificationsRead(): Observable<void> {
    return this.http.put<void>('/api/notifications/mark-all-read', {});
  }

  createBranch(data: CreateBranchRequest): Observable<number> {
    return this.http.post<number>('/api/branches', data);
  }

  updateBranch(id: number, data: UpdateBranchRequest): Observable<void> {
    return this.http.put<void>(`/api/branches/${id}`, data);
  }

  setWorkingHours(branchId: number, hours: WorkingHourDto[]): Observable<void> {
    return this.http.post<void>(`/api/branches/${branchId}/working-hours`, { hours });
  }

  createTimeOff(branchId: number, data: CreateTimeOffRequest): Observable<number> {
    return this.http.post<number>(`/api/branches/${branchId}/time-off`, data);
  }
}
