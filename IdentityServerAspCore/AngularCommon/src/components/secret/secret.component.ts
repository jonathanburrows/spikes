import { Component } from '@angular/core';

import { Planet } from '../../models';
import { ApiService } from '../../services';

@Component({
    selector: 'ic-secret',
    templateUrl: 'secret.component.html'
})
export class SecretComponent {
    private planets: Planet[];

    constructor(private apiService: ApiService) {
        apiService.get(Planet).subscribe(planets => this.planets = planets);
    }
}
