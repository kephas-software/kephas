import { HttpClient } from '@angular/common/http';
import { Component, Inject } from '@angular/core';

@Component({
  selector: 'app-counter-component',
  templateUrl: './counter.component.html'
})
export class CounterComponent {
  constructor(private http: HttpClient, @Inject('BASE_URL') private baseUrl: string) {
  }
  public currentCount = 0;

  public incrementCounter() {
    this.currentCount++;
  }

  public async ping() {
    try {
      this.pingResponse = (await this.http.get<{ message: string }>(this.baseUrl + 'api/cmd/ping').toPromise()).message;
    }
    catch(err) {
      this.pingResponse = err.message;
    }
  }
}
