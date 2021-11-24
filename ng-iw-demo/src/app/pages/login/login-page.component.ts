import { Component, OnInit } from '@angular/core';
import { IWService } from 'src/app/services/iw.service';

@Component({
    selector: 'login-page',
    templateUrl: 'login-page.component.html',
    styleUrls: ['./login-page.component.scss']
})

export class LoginPageComponent implements OnInit {
    constructor(public iwService: IWService) { }

    ngOnInit() { }

    async login() {
        await this.iwService.login();
    }
}
