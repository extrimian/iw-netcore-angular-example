import { Injectable } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class AuthService {
    _token: string | null = null;

    constructor() { }

    setToken(token: string) {
        this._token = token;
    }

    getToken(): string | null {
        return this._token;
    }

    setDID(token: string) {
        this._token = token;
    }

    getDID(): string | null {
        return this._token;
    }
}