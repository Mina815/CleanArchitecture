import { Component, OnInit, OnDestroy, ChangeDetectorRef } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { CentersClient, CenterDetailDto, ServicesClient, ServiceDto, BranchesClient, BranchDto, ReviewsClient, ReviewDto } from '../web-api-client';

@Component({
  standalone: false,
  selector: 'app-center-detail',
  templateUrl: './center-detail.component.html',
  styleUrls: ['./center-detail.component.scss']
})
export class CenterDetailComponent implements OnInit, OnDestroy {
  private destroy$ = new Subject<void>();

  center: CenterDetailDto | null = null;
  services: ServiceDto[] = [];
  branches: BranchDto[] = [];
  reviews: ReviewDto[] = [];
  loading = true;
  error = false;
  activeTab: 'services' | 'branches' | 'reviews' = 'services';

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private centersClient: CentersClient,
    private servicesClient: ServicesClient,
    private branchesClient: BranchesClient,
    private reviewsClient: ReviewsClient,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    if (id) {
      this.loadCenter(id);
    } else {
      this.error = true;
      this.loading = false;
    }
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  private loadCenter(id: number): void {
    this.loading = true;
    this.error = false;
    this.cdr.detectChanges();

    this.centersClient.getCenterById(id).pipe(takeUntil(this.destroy$)).subscribe({
      next: center => {
        this.center = center;
        this.branches = center.branches ?? [];
        this.loading = false;
        this.cdr.detectChanges();
      },
      error: () => {
        this.error = true;
        this.loading = false;
        this.cdr.detectChanges();
      }
    });

    this.servicesClient.getServices(id).pipe(takeUntil(this.destroy$)).subscribe({
      next: services => {
        this.services = services ?? [];
        this.cdr.detectChanges();
      }
    });

    this.branchesClient.getBranches(id).pipe(takeUntil(this.destroy$)).subscribe({
      next: branches => {
        this.branches = branches ?? [];
        this.cdr.detectChanges();
      }
    });

    this.reviewsClient.getReviews(id, undefined, undefined).pipe(takeUntil(this.destroy$)).subscribe({
      next: result => {
        this.reviews = result.items ?? [];
        this.cdr.detectChanges();
      }
    });
  }

  bookNow(): void {
    if (this.center?.id) {
      this.router.navigate(['/book', this.center.id]);
    }
  }

  getStars(rating: number | undefined): boolean[] {
    const r = Math.round(rating ?? 0);
    return [1, 2, 3, 4, 5].map(i => i <= r);
  }
}
