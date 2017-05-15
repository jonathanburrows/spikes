import { Component } from '@angular/core';

import { NavigationItem } from './navigation-item';

@Component({
    selector: 'app-layout',
    templateUrl: 'layout.component.html',
    styleUrls: ['layout.component.scss']
})
export class LayoutComponent {
    public showSideNav = false;
    public selectedNavigation = 'Inbox';

    public navigationItems: NavigationItem[] = [
        {
            name: 'Mail',
            children: [
                { name: 'Contacts', icon: 'contacts' },
                { name: 'Inbox', icon: 'inbox' },
                { name: 'Sent', icon: 'send' },
                { name: 'Drafts', icon: 'drafts' },
                { name: 'Favourites', icon: 'contacts' }
            ]
        }, {
            name: 'Account',
            children: [
                { name: 'Personal', icon: 'person' },
                { name: 'Password', icon: 'update' },
                { name: 'Roles', icon: 'security' },
                { name: 'Developer', icon: 'settings' }
            ]
        }
    ];

    public openSideNav() {
        this.showSideNav = true;
    }

    public closeSideNav() {
        this.showSideNav = false;
    }
}
