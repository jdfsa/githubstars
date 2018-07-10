import { Component, OnInit, Input } from '@angular/core';
import { AuthService } from '../../auth/auth.service';
import { RepositoryService } from '../../services/repository.service';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-repo-item',
  templateUrl: './repo-item.component.html'
})
export class RepoItemComponent {
  @Input() data: any;

  constructor(
    private authService: AuthService,
    private repoService: RepositoryService,
    private toasterService: ToastrService) {}

  public starRepo(repoId: string, starring: boolean) {
    this.repoService.starRepository(this.authService.getUserId(), repoId, starring).subscribe(data => {
      this.data.viewerHasStarred = starring;
      this.data.stargazers += starring ? 1 : -1;
      this.toasterService.success(starring ? 'Repo starred with success!' : 'Repo unstarred successfully!', null, {
        disableTimeOut: false,
        tapToDismiss: false
      });
    });
  }
}
