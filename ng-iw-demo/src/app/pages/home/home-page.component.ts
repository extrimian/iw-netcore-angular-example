import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from 'src/app/services/auth.service';
import { IWService } from 'src/app/services/iw.service';
import { SignService } from 'src/app/services/sign.service';

@Component({
    selector: 'home-page',
    templateUrl: 'home-page.component.html'
})

export class HomePageComponent implements OnInit {
    mainAddress?: string;
    mainDID?: string;

    constructor(private iwService: IWService,
        private authService: AuthService,
        private router: Router) { }

    async ngOnInit(): Promise<void> {
        this.mainAddress = await this.iwService.iw.getMainAddress();
        this.mainDID = await this.iwService.iw.getMainDID();
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

    async createDIDAndAddAssertionMethod() {
        const did: any = await this.iwService.createDIDChangeOwner();
        await this.iwService.addAssertionMethod(did);
    }

    async addAssertionMethod() {
        const did = await this.authService.getDID();
        if (!did) throw new Error("Not did, please login");
        await this.iwService.addAssertionMethod(did);
    }

    async logout() {
        await this.iwService.logout();
        await this.router.navigate([""]);
        window.location.reload();
    }
}