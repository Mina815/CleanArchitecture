import { Component, OnInit, inject } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { CentersClient, CenterDetailDto, ServicesClient, ServiceDto, BranchesClient, BranchDto, ReviewsClient, ReviewDto } from '../web-api-client';

@Component({
  standalone: false,
  selector: 'app-center-detail',
  templateUrl: './center-detail.component.html',
  styleUrls: ['./center-detail.component.scss']
})
export class CenterDetailComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private centersClient = inject(CentersClient);
  private servicesClient = inject(ServicesClient);
  private branchesClient = inject(BranchesClient);
  private reviewsClient = inject(ReviewsClient);

  center: CenterDetailDto | null = null;
  services: ServiceDto[] = [];
  branches: BranchDto[] = [];
  reviews: ReviewDto[] = [];
  loading = true;
  error = false;
  activeTab: 'services' | 'branches' | 'reviews' = 'services';

  ngOnInit(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    if (id) {
      this.loadCenter(id);
    } else {
      this.error = true;
      this.loading = false;
    }
  }

  private loadCenter(id: number): void {
    this.loading = true;
    this.error = false;

    this.centersClient.getCenterById(id).subscribe({
      next: center => {
        this.center = center;
        this.branches = center.branches ?? [];
        this.loading = false;
      },
      error: () => {
        this.error = true;
        this.loading = false;
      }
    });

    this.servicesClient.getServices(id).subscribe({
      next: services => this.services = services ?? []
    });

    this.branchesClient.getBranches(id).subscribe({
      next: branches => this.branches = branches ?? []
    });

    this.reviewsClient.getReviews(id, undefined, undefined).subscribe({
      next: result => this.reviews = result.items ?? []
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
