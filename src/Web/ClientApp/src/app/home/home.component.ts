import { Component, OnInit, inject } from '@angular/core';
import { CentersClient, CenterDto } from '../web-api-client';

@Component({
  standalone: false,
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss']
})
export class HomeComponent implements OnInit {
  private centersClient = inject(CentersClient);

  centers: CenterDto[] = [];
  search = '';
  city = '';
  pageNumber = 1;
  pageSize = 12;
  loading = false;
  totalCount = 0;
  totalPages = 0;
  hasPreviousPage = false;
  hasNextPage = false;

  cities = ['الرياض', 'جدة', 'مكة', 'المدينة', 'الدمام', 'الخبر', 'تبوك', 'بريدة', 'حائل', 'أبها'];

  ngOnInit(): void {
    this.loadCenters();
  }

  loadCenters(): void {
    this.loading = true;
    this.centersClient.getCenters(
      this.city || undefined,
      this.search || undefined,
      this.pageNumber,
      this.pageSize
    ).subscribe({
      next: result => {
        this.centers = result.items ?? [];
        this.totalCount = result.totalCount ?? 0;
        this.totalPages = result.totalPages ?? 0;
        this.hasPreviousPage = result.hasPreviousPage ?? false;
        this.hasNextPage = result.hasNextPage ?? false;
        this.loading = false;
      },
      error: () => {
        this.loading = false;
      }
    });
  }

  searchCenters(): void {
    this.pageNumber = 1;
    this.loadCenters();
  }

  previousPage(): void {
    if (this.pageNumber > 1) {
      this.pageNumber--;
      this.loadCenters();
    }
  }

  nextPage(): void {
    this.pageNumber++;
    this.loadCenters();
  }

}
