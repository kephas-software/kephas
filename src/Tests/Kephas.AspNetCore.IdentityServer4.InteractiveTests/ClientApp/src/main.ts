import { App } from './app/app';

new App().bootstrapAsync()
  .then(_ => {  })
  .catch(err => console.log(err));
