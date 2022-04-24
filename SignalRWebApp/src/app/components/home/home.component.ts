import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { SignalrService } from 'src/app/services/signalr.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss']
})
export class HomeComponent implements OnInit {

  name!: string;

  constructor(
    private router: Router,
    private signaling: SignalrService
  ) { }

  ngOnInit(): void {
  }

  startChat(): void {
    this.signaling.connect('/auth', false).then(() => {
      if (this.signaling.isConnected()) {
        this.signaling.invoke('Authorize', this.name).then((token: string) => {
          if (token) {
            sessionStorage.setItem('username', this.name);
            sessionStorage.setItem('token', token);
            
            this.router.navigate(['chat']);
          }
        });
      }
    });
  }

}
