import { Injectable } from '@angular/core';

import { StorageService } from './storage.service';

@Injectable()
export class LocalStorageService extends StorageService {
    get length() {
        return localStorage.length;
    }

    clear() {
        localStorage.clear();
    }

    getItem(key: string) {
        return localStorage.getItem(key);
    }

    key(index: number) {
        return localStorage.key(index);
    }

    removeItem(key: string) {
        localStorage.removeItem(key);
    }

    setItem(key: string, data: string) {
        localStorage.setItem(key, data);
    }
}
