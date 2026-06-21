import { Injectable, inject } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { BookingStore } from '../stores/booking.store';

@Injectable({ providedIn: 'root' })
export class BookingHubService {
  private readonly bookingStore = inject(BookingStore);
  private hubConnection: signalR.HubConnection | null = null;

  connect(token: string, branchId: number): void {
    if (this.hubConnection?.state === signalR.HubConnectionState.Connected) return;
    if (this.hubConnection) { this.hubConnection.stop(); }

    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl('/hub/bookings', { accessTokenFactory: () => token })
      .withAutomaticReconnect()
      .build();

    this.hubConnection.on('NewBooking', (data: any) => {
      this.bookingStore.pushBooking(data);
    });

    this.hubConnection.on('BookingConfirmed', (bookingId: number) => {
      this.bookingStore.updateBookingStatus(bookingId, 'Confirmed');
    });

    this.hubConnection.on('BookingCompleted', (bookingId: number) => {
      this.bookingStore.updateBookingStatus(bookingId, 'Completed');
    });

    this.hubConnection.on('BookingCancelled', (data: { bookingId: number; reason?: string }) => {
      this.bookingStore.updateBookingStatus(data.bookingId, 'Cancelled');
    });

    this.hubConnection.start().then(() => {
      this.hubConnection!.invoke('JoinBranchGroup', branchId);
    });
  }

  disconnect(): void {
    if (this.hubConnection) {
      this.hubConnection.stop();
      this.hubConnection = null;
    }
  }
}
