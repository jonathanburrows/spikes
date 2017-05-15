import { Component } from '@angular/core';

import { ListItem } from './list-item';

@Component({
    selector: 'app-list',
    templateUrl: 'list.component.html',
    styleUrls: ['list.component.scss']
})
export class ListComponent {
    private items: ListItem[] = [
        { category: 'family', name: 'Jane Smith' },
        { category: 'family', name: 'John Smith' },
        { category: 'family', name: 'Doe Smith' },
        { category: 'pet', name: 'Mittens' },
        { category: 'pet', name: 'Smittens' },
        { category: 'pet', name: 'Pewpew' },
        { category: 'pet', name: 'Captain Funbuns' },
        { category: 'pet', name: 'Leftenant Lumps' },
        { category: 'pet', name: 'Nugs' },
        { category: 'pet', name: 'Grumpasaraus' }
    ]

    public get family() {
        return this.items.filter(item => item.category === 'family');
    }

    public get pets() {
        return this.items.filter(item => item.category === 'pet');
    }
}
