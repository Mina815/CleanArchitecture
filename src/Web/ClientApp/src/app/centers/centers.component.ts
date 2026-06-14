import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CenterDto, JamalekApiService } from '../services/jamalek-api.service';

@Component({
  standalone: false,
  selector: 'app-centers',
  templateUrl: './centers.component.html'
})
export class CentersComponent implements OnInit {
  centers: CenterDto[] = [];
  city = 'Cairo';
  search = '';
  loading = true;
  error = '';
  private loadingTimeout?: ReturnType<typeof setTimeout>;

  constructor(private api: JamalekApiService, private cdr: ChangeDetectorRef) {}

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.loading = true;
    this.error = '';
    this.cdr.detectChanges();
    this.loadingTimeout = setTimeout(() => { this.loading = false; this.error = 'Server not responding. Try again.'; this.cdr.detectChanges(); }, 15000);
    this.api.getCenters(this.city || undefined, this.search || undefined).subscribe({
      next: data => { clearTimeout(this.loadingTimeout); this.centers = data; this.loading = false; this.cdr.detectChanges(); },
      error: () => { clearTimeout(this.loadingTimeout); this.error = 'Failed to load centers.'; this.loading = false; this.cdr.detectChanges(); }
    });
  }
}
