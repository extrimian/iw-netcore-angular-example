import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { IdentityWallet, SDKOperationRequest, SDKResponse } from "@extrimian/identity-wallet";
import { environment } from 'src/environments/environment';
import { AuthService } from './auth.service';

@Injectable({ providedIn: 'root' })
export class IWService {
    iw: IdentityWallet;
    controller = `${environment.DAPP_API_URL}/identitywallet`;

    handshakeFinished: boolean = false;

    constructor(private router: Router,
        private httpClient: HttpClient,
        private authService: AuthService,
    ) {
        this.iw = new IdentityWallet({
            container: 'identity-wallet',
            url: environment.IW_FRONTEND_URL,
            sdkProcessMessageEndpoint: `${this.controller}/processIWMessage`,
            withCredential: false,
        });
    }

    async init() {
        this.iw.loggedIn = (response) => {
            console.log("LoggedIN", response.headers["x-accesstoken"]);
            this.authService.setToken(response.headers["x-accesstoken"]);
            this.authService.setDID(response.headers["x-did"]);
            this.router.navigate(["home"]);
        }

        await this.iw.init();
    }

    async handshake() {
        await this.iw.sdkRequest(async (state) => {
            return await this.httpClient.post<SDKResponse>(`${this.controller}/handshake`, state).toPromise();
        }, SDKOperationRequest.Handshake);

        this.handshakeFinished = true;
    }

    async login() {
        await this.iw.sdkRequest(async (state) => {
            return await this.httpClient.post<SDKResponse>(`${this.controller}/vc-login`, state).toPromise();
        }, SDKOperationRequest.Login);

        console.log("Login finished");
    }

    send(message: string) {
        return this.iw.send(message);
    }

    close(message: string) {
        return this.iw.close();
    }

    async extrSign() {
        const result = await this.iw.sdkRequest(async (state) => {
            return await this.httpClient.post<SDKResponse>(`${this.controller}/extr-sign-content`, state).toPromise();
        }, SDKOperationRequest.SignContent, "Extr");
        debugger;
        console.log(result);

        return result;
    }

    async extrIdentitySign() {
        const result = await this.iw.sdkRequest(async (state) => {
            return await this.httpClient.post<SDKResponse>(`${this.controller}/extr-sign-content`, state).toPromise();
        }, SDKOperationRequest.SignContent, "Extr");

        console.log(result);

        return result;
    }

    async decryptContent(encryptedContent: string) {
        await this.httpClient.post(`${this.controller}/decrypt-content`, {
            content: encryptedContent
        }).toPromise();
    }

    async sign() {
        const result = await this.iw.sdkRequest(async (state) => {
            return await this.httpClient.post<SDKResponse>(`${this.controller}/sign-content`, state).toPromise();
        }, SDKOperationRequest.SignContent);

        console.log(result);
    }

    async createDIDChangeOwner(): Promise<string> {
        const did = await this.authService.getDID();

        const result = await this.httpClient.post<{ newDid: string }>(`${this.controller}/create-did-change-owner`, {
            ownerDid: did
        }).toPromise();

        return result.newDid;
    }

    async addAssertionMethod(did: string) {
        const result = await this.iw.sdkRequest(async (state) => {
            return await this.httpClient.post<SDKResponse>(`${this.controller}/add-assertion-method`, {
                state: state,
                did: did
            }).toPromise();
        }, SDKOperationRequest.SignContent, "Extr");

        await this.httpClient.post<{ newDid: string }>(`${this.controller}/process-add-assertion-method`, {
            content: result.encryptedContent,
            did: did,
        }).toPromise();
    }
}