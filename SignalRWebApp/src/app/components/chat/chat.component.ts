import { Component, OnInit } from '@angular/core';
import { SignalrService } from 'src/app/services/signalr.service';

@Component({
  selector: 'app-chat',
  templateUrl: './chat.component.html',
  styleUrls: ['./chat.component.scss']
})
export class ChatComponent implements OnInit {

  name!: string;

  constructor(
    private signaling: SignalrService
  ) {
    this.name = sessionStorage.getItem('username') || '';
  }

  async ngOnInit(): Promise<void> {
    await this.signaling.connect('/main', true);
  }

}
