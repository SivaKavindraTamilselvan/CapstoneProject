import { Injectable, signal } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { NotificationResponseModel } from '../models/notification/notification.model';
import { BaseURL } from '../environment';

@Injectable({ providedIn: 'root' })
export class NotificationHubService {
    private hubConnection: signalR.HubConnection | null = null;

    connectionStatus = signal<'connected' | 'connecting' | 'reconnecting' | 'disconnected'>('disconnected');
    latestNotification = signal<NotificationResponseModel | null>(null);

    private readonly hubUrl = BaseURL.replace('/api', '') + '/notificationhub';

    start(): void {
        if (this.hubConnection) {
            console.log('Hub already exists, skipping start');
            return;
        }

        const token = sessionStorage.getItem('token');
        console.log('Hub start() called, token present?', !!token);
        if (!token) {
            console.log('No token — aborting hub start');
            return;
        }

        this.hubConnection = new signalR.HubConnectionBuilder()
            .withUrl(this.hubUrl, {
                accessTokenFactory: () => token,
            })
            .withAutomaticReconnect()
            .configureLogging(signalR.LogLevel.Information)
            .build();

        this.hubConnection.on('ReceiveNotification', (data: NotificationResponseModel) => {
            this.latestNotification.set(data);
        });

        this.hubConnection.onreconnecting(() => this.connectionStatus.set('reconnecting'));
        this.hubConnection.onreconnected(() => this.connectionStatus.set('connected'));
        this.hubConnection.onclose(() => this.connectionStatus.set('disconnected'));

        this.connectionStatus.set('connecting');

        this.hubConnection
            .start()
            .then(() => {
                console.log('SignalR connected!');
                this.connectionStatus.set('connected');
            })
            .catch((err) => {
                console.error('SignalR connection error:', err);
                this.connectionStatus.set('disconnected');
            });
    }
    stop(): void {
        this.hubConnection?.stop();
        this.hubConnection = null;
        this.connectionStatus.set('disconnected');
    }
}