import { Component, OnInit } from '@angular/core';
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

  constructor(private api: JamalekApiService) {}

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.loading = true;
    this.api.getCenters(this.city || undefined, this.search || undefined).subscribe({
      next: data => { this.centers = data; this.loading = false; },
      error: () => { this.error = 'Failed to load centers.'; this.loading = false; }
    });
  }
}
