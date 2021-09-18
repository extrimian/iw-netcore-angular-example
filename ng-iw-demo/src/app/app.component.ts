import { Component, OnInit } from '@angular/core';
import { IWService } from './services/iw.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit {
  title = 'ng-iw-demo';

  constructor(private iwService: IWService) {
  }

  async ngOnInit(): Promise<void> {
    await this.iwService.init();
    await this.iwService.handshake();
  }
}
