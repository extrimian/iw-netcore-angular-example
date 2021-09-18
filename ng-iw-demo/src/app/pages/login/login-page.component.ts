import { Component, OnInit } from '@angular/core';
import { IWService } from 'src/app/services/iw.service';

@Component({
    selector: 'login-page',
    templateUrl: 'login-page.component.html'
})

export class LoginPageComponent implements OnInit {
    loading = false;
    
    constructor(public iwService: IWService) { }

    ngOnInit() { }

    async login() {
        this.loading = true;
        await this.iwService.login();
        this.loading = false;
    }
}