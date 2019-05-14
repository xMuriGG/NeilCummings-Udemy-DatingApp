import { Component, OnInit } from '@angular/core';
import { User } from '../_models/user';
import { ActivatedRoute } from '@angular/router';
import { AuthService } from '../_services/auth.service';
import { UserService } from '../_services/user.service';
import { AlertifyService } from '../_services/alertify.service';
import { Pagination, PaginatedResult } from '../_models/pagination';

@Component({
  selector: 'app-lists',
  templateUrl: './lists.component.html',
  styleUrls: ['./lists.component.css']
})
export class ListsComponent implements OnInit {
  paginatedUsersList: PaginatedResult<User[]>;
  likesParam: string;

  constructor(private authservice: AuthService, private userService: UserService,
    private alertify: AlertifyService, private route: ActivatedRoute) { }

  ngOnInit() {
    this.route.data.subscribe(data => {
      this.paginatedUsersList = data.paginatedUsersList;
    });
    this.likesParam = 'Likers';
  }


  loadUsers() {
    this.userService.getUsers(this.paginatedUsersList.pagination.currentPage, this.paginatedUsersList.pagination.itemsPerPage,
      null, this.likesParam)
      .subscribe((paginatedList: PaginatedResult<User[]>) => {
        this.paginatedUsersList = paginatedList;
      }, error => {
        this.alertify.error(error);
      });
  }

  pageChanged(event: any): void {
    this.paginatedUsersList.pagination.currentPage = event.page;
    this.loadUsers();
  }

}
