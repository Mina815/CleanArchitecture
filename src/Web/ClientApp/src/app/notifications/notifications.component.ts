import { Component, OnInit } from '@angular/core';
import { JamalekApiService, NotificationDto } from '../services/jamalek-api.service';

@Component({
  standalone: false,
  selector: 'app-notifications',
  templateUrl: './notifications.component.html'
})
export class NotificationsComponent implements OnInit {
  notifications: NotificationDto[] = [];
  unreadOnly = false;
  loading = true;

  constructor(private api: JamalekApiService) {}

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.loading = true;
    this.api.getNotifications(this.unreadOnly).subscribe({
      next: data => { this.notifications = data; this.loading = false; },
      error: () => this.loading = false
    });
  }

  markRead(n: NotificationDto): void {
    if (n.isRead) return;
    this.api.markNotificationRead(n.id).subscribe(() => {
      n.isRead = true;
    });
  }

  markAllRead(): void {
    this.api.markAllNotificationsRead().subscribe(() => this.load());
  }
}
