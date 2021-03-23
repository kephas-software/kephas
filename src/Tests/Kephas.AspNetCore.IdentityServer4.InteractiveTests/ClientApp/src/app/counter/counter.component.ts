import { HttpClient } from '@angular/common/http';
import { Component } from '@angular/core';

@Component({
  selector: 'app-counter-component',
  templateUrl: './counter.component.html'
})
export class CounterComponent {
  constructor(private http: HttpClient) {
  }
  public currentCount = 0;
  public pingResponse = 'Ping not invoked';

  public incrementCounter() {
    this.currentCount++;
  }

  public async ping() {
    try {
      this.pingResponse = (await this.http.get<{ message: string }>('api/cmd/ping').toPromise()).message;
    }
    catch(err) {
      this.pingResponse = err.message;
    }
  }
}
