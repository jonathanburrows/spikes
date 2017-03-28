import { Injectable } from '@angular/core';

@Injectable()
export abstract class StorageService implements Storage {
    get length(): number { throw new Error('Not implemented'); }

    [key: string]: any;
    [index: number]: string;

    abstract clear(): void;
    abstract getItem(key: string): any;
    abstract key(index: number): string;
    abstract removeItem(key: string): void;
    abstract setItem(key: string, data: string): void;
}
