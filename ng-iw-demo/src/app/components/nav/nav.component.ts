import { Component, OnInit} from '@angular/core';
import {IWService} from "../../services/iw.service";
import {Router, NavigationStart,Event as NavigationEvent} from "@angular/router";

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.scss']
})
export class NavComponent implements OnInit {

  loading = false;
  handshake = true;
  changeRouteEvent$;
  route = "";

  constructor(public iwService: IWService,
  private router: Router
) {
    this.changeRouteEvent$ = this.router.events.subscribe(
      (event: NavigationEvent) => {
        if(event instanceof NavigationStart) {
          this.route = event.url;
        }
      });

    this.iwService.handshakeFinished.subscribe(message => {
      if (message !== this.handshake) {
        this.handshake = message;
      }
    });

  }

  ngOnDestroy() {
    this.changeRouteEvent$.unsubscribe();
    this.iwService.handshakeFinished.unsubscribe();
  }

  ngOnInit(): void {
  }

  async login(){
    this.loading = true;
    await this.iwService.login();
    this.loading = false;
  }

  async logout() {
    await this.iwService.logout();
    await this.router.navigate([""]);
    window.location.reload();
  }
}
