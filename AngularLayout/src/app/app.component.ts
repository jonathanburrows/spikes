import { Component } from '@angular/core';

@Component({
    selector: 'app-root',
    templateUrl: './app.component.html',
    styleUrls: ['./app.component.scss']
})
export class AppComponent {
    showHamburger = false;

    toggleHamburger() {
        this.showHamburger = !this.showHamburger;
    }
}
