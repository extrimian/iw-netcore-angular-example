import { Component, OnInit } from '@angular/core';
import { IWService } from 'src/app/services/iw.service';
import { SignService } from 'src/app/services/sign.service';

@Component({
    selector: 'home-page',
    templateUrl: 'home-page.component.html'
})

export class HomePageComponent implements OnInit {
    mainAddress?: string;

    constructor(private iwService: IWService) { }

    async ngOnInit(): Promise<void> {
        this.mainAddress = await this.iwService.iw.getMainAddress();
    }

    async sign() {
        await this.iwService.sign();
    }

    async extrSign() {
        const response: any = await this.iwService.extrSign();
        console.log(response);
        await this.iwService.decryptContent(response.encryptedContent);
    }

    async extrIdentitySign() {
        const response: any = await this.iwService.extrIdentitySign();
        await this.iwService.decryptContent(response.encryptedContent);
    }
}