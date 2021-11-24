import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from 'src/app/services/auth.service';
import { IWService } from 'src/app/services/iw.service';
import { SignService } from 'src/app/services/sign.service';

@Component({
    selector: 'home-page',
    templateUrl: 'home-page.component.html',
    styleUrls: ['home-page.component.scss']
})

export class HomePageComponent implements OnInit {
    mainAddress?: string;
    mainDID?: string;
    loading = false;
    handshake = false;
    loadingMainAddress=false;
    loadingMainDID = false;
    did = "";


  constructor(private iwService: IWService,
        private authService: AuthService,
    ) { }

    async ngOnInit(): Promise<void> {
      this.loadingMainAddress = true;
      this.loadingMainDID = true;
      this.loading = true;

      this.iwService.handshakeFinished.subscribe(message => {
        if (message) {
          this.handshake = message;
          this.getMainAddress();
          this.getMainDid();
          this.loading = false;
        }
      });

    }

    ngOnDestroy() {
    this.iwService.handshakeFinished.unsubscribe();
   }

    async getMainAddress (){
      this.mainAddress = await this.iwService.iw.getMainAddress();
      this.loadingMainAddress = false;
    }

    async getMainDid(){
      this.mainDID = await this.iwService.iw.getMainDID();
      this.loadingMainDID = false;
    }

    async sign() {
      this.loading = true;
      await this.iwService.sign();
      this.loading = false;
    }

    async extrSign() {
      this.loading = true;
      const response: any = await this.iwService.extrSign();
      this.loading = false;
      await this.iwService.decryptContent(response.encryptedContent);
    }

    async extrIdentitySign() {
      this.loading = true;
      const response: any = await this.iwService.extrIdentitySign();
      this.loading = false;
      await this.iwService.decryptContent(response.encryptedContent);
    }

    async createDID() {
      this.loading = true;
      this.did = await this.iwService.createDIDChangeOwner();
      this.loading = false;

    }

    async addAssertionMethod(){
      this.loading = true;
      await this.iwService.addAssertionMethod(this.did);
      this.loading = false;
    }

    /*async addAssertionMethod() {
      this.loading = true;
      const did = await this.authService.getDID();
      this.loading = false;
      if (!did) throw new Error("Not did, please login");
      await this.iwService.addAssertionMethod(did);
    }*/


}
