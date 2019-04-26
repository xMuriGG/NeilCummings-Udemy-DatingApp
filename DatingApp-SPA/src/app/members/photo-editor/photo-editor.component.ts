import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { Photo } from 'src/app/_models/photo';
import { FileUploader } from 'ng2-file-upload';
import { environment } from 'src/environments/environment';
import { AuthService } from 'src/app/_services/auth.service';
import { UserService } from 'src/app/_services/user.service';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-photo-editor',
  templateUrl: './photo-editor.component.html',
  styleUrls: ['./photo-editor.component.css']
})
export class PhotoEditorComponent implements OnInit {
  @Input() photos: Photo[];
  @Output() getMemberPhotoChange = new EventEmitter<string>();
  constructor(private authService: AuthService, private userService: UserService, private alertify: AlertifyService,
    private router: Router) { }
  baseUrl = environment.baseUrl;

  uploader: FileUploader;
  hasBaseDropZoneOver = false;
  currentMainPhoto: Photo;

  ngOnInit() {
    this.initializeUploader();
  }


  public fileOverBase(e: any): void {
    this.hasBaseDropZoneOver = e;
  }
  initializeUploader() {
    this.uploader = new FileUploader({
      url: this.baseUrl + 'users/' + this.authService.decodedToken.nameid + '/photos',
      authToken: 'Bearer ' + localStorage.getItem('token'),
      isHTML5: true,
      allowedFileType: ['image'],
      removeAfterUpload: true,
      autoUpload: false,
      maxFileSize: 10 * 1024 * 1024
    });
    // ovo se dodaje da se izbjegne CORS error prilikom slanja fajlova
    this.uploader.onAfterAddingFile = (file) => { file.withCredentials = false; };

    this.uploader.onSuccessItem = (item, response, status, headers) => {
      if (response) {
        const res: Photo = JSON.parse(response);
        const photo = {
          id: res.id,
          url: res.url,
          dateAdded: res.dateAdded,
          description: res.description,
          isMain: res.isMain
        };
        this.photos.push(photo);

        // u slucaju da je useru ovo prva fotografija API ce je postaviti kao glavnu
        if (photo.isMain) {
          this.authService.changeMemberPhoto(photo.url);
          this.authService.currentUser.photoUrl = photo.url;
          localStorage.setItem('user', JSON.stringify(this.authService.currentUser));
        }
      }
    };
  }
  setMainPhoto(photo: Photo) {
    this.userService.setMainPhoto(this.authService.decodedToken.nameid, photo.id).subscribe(() => {
      this.currentMainPhoto = this.photos.filter(f => f.isMain === true)[0];
      this.currentMainPhoto.isMain = false;
      photo.isMain = true;

      // Note: ovo se koristilo kao zamjena za BehaviorSubject da se photoUrl proslijedi i na druge komponente
      // this.getMemberPhotoChange.emit(photo.url);
      this.authService.changeMemberPhoto(photo.url);
      // ovo se moglo uraditi i bez BehaviorSubject samo sa linijom ispod
      this.authService.currentUser.photoUrl = photo.url;
      localStorage.setItem('user', JSON.stringify(this.authService.currentUser));

      // Note: kod za refresh komponente
      // this.router.navigateByUrl('RefreshComponent', { skipLocationChange: true }).then(() =>
      //   this.router.navigate(['member/edit']));

    },
      error => {
        this.alertify.error(error);
      });
  }

  deletePhoto(photo: Photo) {
    if (photo.isMain) {
      this.alertify.warning('Can\'t delete main photo');
      return;
    }

    this.alertify.confirm('Are you sure you want to delete this photo.', () => {
      this.userService.deletePhoto(this.authService.decodedToken.nameid, photo.id).subscribe(() => {
        const photoIndex = this.photos.indexOf(photo);
        // jos jedan nacin
        // const photoIndex = this.photos.findIndex(p => p.id == photo.id);
        this.photos.splice(photoIndex, 1);

      }, error => {
        this.alertify.error(error);
      });
    });


  }
}
