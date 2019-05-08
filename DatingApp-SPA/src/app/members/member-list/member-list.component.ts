import { Component, OnInit } from '@angular/core';
import { User } from 'src/app/_models/user';
import { UserService } from 'src/app/_services/user.service';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { ActivatedRoute } from '@angular/router';
import { PaginatedResult } from 'src/app/_models/pagination';


@Component({
  selector: 'app-member-list',
  templateUrl: './member-list.component.html',
  styleUrls: ['./member-list.component.css']
})
export class MemberListComponent implements OnInit {
  // users: User[];
  paginatedUsersList: PaginatedResult<User[]>;
  user: User = JSON.parse(localStorage.getItem('user'));
  genderList = [{ value: 'male', display: 'Males' }, { value: 'female', display: 'Females' }];
  // filterList = [{ value: 'created', display: 'Date created' }, { value: 'createdDesc', display: 'Date created Desc' }];

  userParams: any = {};
  constructor(private userService: UserService, private alertify: AlertifyService, private route: ActivatedRoute) { }

  ngOnInit() {
    this.route.data.subscribe(data => {
      this.paginatedUsersList = data.paginatedUsersList; // data.paginatedUsersList zbog routes.ts 'members' route
      // this.users = this.paginatedUsersList.result;

      this.setFilters();

    });
  }

  loadUsers() {

    this.userService.getUsers(this.paginatedUsersList.pagiantion.currentPage, this.paginatedUsersList.pagiantion.itemsPerPage,
      this.userParams)
      .subscribe((paginatedList: PaginatedResult<User[]>) => {
        this.paginatedUsersList = paginatedList;
      }, error => {
        this.alertify.error(error);
      });
  }

  pageChanged(event: any): void {
    this.paginatedUsersList.pagiantion.currentPage = event.page;
    this.loadUsers();
  }

  setFilters() {
    this.userParams.gender = this.user.gender === 'male' ? 'female' : 'male';
    this.userParams.minAge = '';
    this.userParams.maxAge = '';
    this.userParams.orderBy = '';
  }
  resetFilters() {
    this.setFilters();
    this.loadUsers();
  }
}


  // Note: ovo nije više potrebno, jer je korišten resolver member-datail.resolver.ts
  // loadUsers() {
  //   this.userService.getUsers().subscribe((users: User[]) => {
  //     this.users = users;
  //     this.alertify.success('Users found: ' + users.length);
  //   }, error => {
  //     this.alertify.error(error);
  //   });
  // }


