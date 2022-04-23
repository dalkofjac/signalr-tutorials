import { Component, OnDestroy, OnInit } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { SignalrService } from 'src/app/services/signalr.service';

@Component({
  selector: 'app-chat',
  templateUrl: './chat.component.html',
  styleUrls: ['./chat.component.scss']
})
export class ChatComponent implements OnInit, OnDestroy {

  name!: string;

  constructor(
    private signaling: SignalrService,
    private snackBar: MatSnackBar
  ) {
    this.name = sessionStorage.getItem('username') || '';
  }

  ngOnDestroy(): void {
    this.signaling.disconnect();
  }

  ngOnInit(): void {
    this.signaling.connect('/main', true);
    this.defineSignaling();
  }

  private defineSignaling(): void {
    this.signaling.define('connected', (client: string) => {
      this.notify(client + ' connected to the chat!');
    });
    this.signaling.define('disconnected', (client: string) => {
      this.notify(client + ' disconnected from the chat!');
    });
    this.signaling.define('message', (from: string, message: any) => {
      this.notify(from + ' sent a message: ' + message);
    });
  }

  private notify(message: string): void {
    this.snackBar.open(message, 'Dismiss', {
      duration: 5000
    });
  }
}
