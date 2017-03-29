import { Injectable } from '@angular/core';

@Injectable()
export class ConfigurationService {
    url = 'http://localhost:5003';
    authorizationServerUrl = 'https://localhost:44300';
    resourceServerUrl = 'http://localhost:5001';
    clientName = 'resource-owner-client';
    clientSecret = 'secret';
}
