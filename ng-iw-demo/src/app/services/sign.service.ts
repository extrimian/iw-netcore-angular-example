import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { SDKOperationRequest, SDKResponse } from '@extrimian/identity-wallet';
import { environment } from 'src/environments/environment';
import { IWService } from './iw.service';

@Injectable({ providedIn: 'root' })
export class SignService {
    controller = `${environment.DAPP_API_URL}/sign`;

    constructor(private iwService: IWService,
        private http: HttpClient) { }


    async sign() {
        const result = await this.iwService.iw.sdkRequest(async (state) => {
            return await this.http.post<SDKResponse>(`${this.controller}/sign-content`, state).toPromise();
        }, SDKOperationRequest.SignContent);

        console.log(result);
    }

    async extrSign() {
        const result = await this.iwService.iw.sdkRequest(async (state) => {
            return await this.http.post<SDKResponse>(`${this.controller}/extr-sign-content`, state).toPromise();
        }, SDKOperationRequest.SignContent, "Extr");
        
        return await this.http.post<SDKResponse>(`${this.controller}/process-signature`, result).toPromise();
    }
}