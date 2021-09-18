import { Component, OnInit } from '@angular/core';
import { IWService } from 'src/app/services/iw.service';
import { SignService } from 'src/app/services/sign.service';

@Component({
    selector: 'home-page',
    templateUrl: 'home-page.component.html'
})

export class HomePageComponent implements OnInit {

    constructor(private iwService: IWService) { }

    ngOnInit() { }

    async sign() {
        await this.iwService.sign();
    }

    async extrSign() {
        await this.iwService.extrSign();
    }
}