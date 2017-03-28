import { Component, OnInit } from '@angular/core';

import { SecurityService } from '../../services';

@Component({
    selector: 'ic-logout-local',
    templateUrl: 'logout-local.component.html'
})
export class LogoutLocalComponent implements OnInit {
    constructor(private securityService: SecurityService) {}

    ngOnInit() {
        this.securityService.logoutLocal();
    }
}
