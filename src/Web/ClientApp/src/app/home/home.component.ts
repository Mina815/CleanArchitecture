import { Component, OnInit, OnDestroy, ChangeDetectorRef } from '@angular/core';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { CentersClient, CenterDto } from '../web-api-client';

@Component({
  standalone: false,
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss']
})
export class HomeComponent implements OnInit, OnDestroy {
  private destroy$ = new Subject<void>();

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

  constructor(
    private centersClient: CentersClient,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.loadCenters();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  loadCenters(): void {
    this.loading = true;
    this.cdr.detectChanges();

    this.centersClient.getCenters(
      this.city || undefined,
      this.search || undefined,
      this.pageNumber,
      this.pageSize
    ).pipe(takeUntil(this.destroy$)).subscribe({
      next: result => {
        this.centers = result.items ?? [];
        this.totalCount = result.totalCount ?? 0;
        this.totalPages = result.totalPages ?? 0;
        this.hasPreviousPage = result.hasPreviousPage ?? false;
        this.hasNextPage = result.hasNextPage ?? false;
        this.loading = false;
        this.cdr.detectChanges();
      },
      error: () => {
        this.loading = false;
        this.cdr.detectChanges();
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
