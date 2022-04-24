import { Component, OnDestroy, OnInit } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ChatMessage } from 'src/app/models/chat-message';
import { SignalrService } from 'src/app/services/signalr.service';
import { Location } from '@angular/common';

@Component({
  selector: 'app-chat',
  templateUrl: './chat.component.html',
  styleUrls: ['./chat.component.scss']
})
export class ChatComponent implements OnInit, OnDestroy {

  name!: string;
  group!: string;
  client!: string;
  message!: string;

  groups: string[] = [];
  messages: ChatMessage[] = [];

  sendTo: string = 'All';
  sendToList: string[] = ['All', 'Group', 'Client'];

  constructor(
    private signaling: SignalrService,
    private snackBar: MatSnackBar,
    private location: Location,
  ) {
    const username = sessionStorage.getItem('username');
    if (!username) {
      this.location.back();
    } else {
      this.name = username;
    }
  }

  ngOnInit(): void {
    // #1 connect to signaling server
    this.signaling.connect('/main', true);

    // #2 define signaling communication
    this.defineSignaling();
  }

  
  defineSignaling(): void {
    this.signaling.define('connected', (client: string) => {
      this.notify(client + ' connected to the chat!');
    });
    this.signaling.define('disconnected', (client: string) => {
      this.notify(client + ' disconnected from the chat!');
    });
    this.signaling.define('message', (from: string, message: any) => {
      this.notify(from + ' sent a message: ' + message);
      this.messages.push({ from, message } as ChatMessage);
    });
  }

  async joinGroup(): Promise<void> {
    await this.signaling.invoke('JoinGroup', this.group).then(() => {
      this.notify('Joined a group: ' + this.group);
      this.groups.push(this.group);
      this.group = '';
    });
  }

  async leaveGroup(): Promise<void> {
    await this.signaling.invoke('LeaveGroup', this.group).then(() => {
      this.notify('Left a group: ' + this.group);
      this.groups = this.groups.filter(c => c.indexOf(this.group) === -1);
      this.group = '';
    });
  }

  async sendMessage(): Promise<void> {
    switch(this.sendTo) {
      case 'All':
        await this.signaling.invoke('SendMessage', this.name, this.message).then(() => {
          this.messages.push({ from: this.name, message: this.message } as ChatMessage);
          this.message = '';
        });
        break;
      case 'Group':
        await this.signaling.invoke('SendMessageToGroup', this.name, this.group, this.message).then(() => {
          this.messages.push({ from: this.name, message: this.message } as ChatMessage);
          this.message = '';
        });
        break;
      case 'Client':
        await this.signaling.invoke('SendMessageToClient', this.name, this.client, this.message).then(() => {
          this.messages.push({ from: this.name, message: this.message } as ChatMessage);
          this.message = '';
        });
        break;
      default:
        break;
    }

  }

  notify(message: string): void {
    this.snackBar.open(message, undefined, {
      duration: 3000
    });
  }

  ngOnDestroy(): void {
    this.signaling.disconnect();
  }
}
