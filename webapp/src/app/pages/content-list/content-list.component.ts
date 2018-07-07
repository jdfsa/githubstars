import { Component, OnInit } from '@angular/core';
import { RepositoryService } from '../../services/repository.service';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-content-list',
  templateUrl: './content-list.component.html'
})
export class ContentListComponent implements OnInit {
  noResult: boolean;
  data: any;

  constructor(
    private route: ActivatedRoute,
    private repoService: RepositoryService) { }

  ngOnInit() {
    this.route.queryParams.subscribe(params => {
      this.search(params['search']);
    });
  }

  private search(criteria: string) {
    this.repoService.getRepositories(criteria).subscribe(response => {
      const edges = response.data && response.data.search && response.data.search.edges ? response.data.search.edges : [];
      this.data = edges.length > 0 ? edges[0].node : null;
      this.noResult = !this.data;
    });
  }

}
