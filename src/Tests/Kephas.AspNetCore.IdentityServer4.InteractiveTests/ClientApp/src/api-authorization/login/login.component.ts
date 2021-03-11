import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { BehaviorSubject } from 'rxjs';
import {
  LoginActions, QueryParameterNames, AuthenticationSettingsProvider,
  ReturnUrlType, AuthenticationService, AuthenticationResultStatus
} from '@kephas/angular-oidc';

// The main responsibility of this component is to handle the user's login process.
// This is the starting point for the login process. Any component that needs to authenticate
// a user can simply perform a redirect to this component with a returnUrl query parameter and
// let the component perform the login and return back to the return url.
@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {
  public message = new BehaviorSubject<string>(null);

  constructor(
    private authenticationService: AuthenticationService,
    private activatedRoute: ActivatedRoute,
    private router: Router,
    private authSettingsProvider: AuthenticationSettingsProvider) { }

  async ngOnInit() {
    const action = this.activatedRoute.snapshot.url[1];
    switch (action.path) {
      case LoginActions.Login:
        await this.login(this.getReturnUrl());
        break;
      case LoginActions.LoginCallback:
        await this.processLoginCallback();
        break;
      case LoginActions.LoginFailed:
        const message = this.activatedRoute.snapshot.queryParamMap.get(QueryParameterNames.Message);
        this.message.next(message);
        break;
      case LoginActions.Profile:
        this.redirectToProfile();
        break;
      case LoginActions.Register:
        this.redirectToRegister();
        break;
      default:
        throw new Error(`Invalid action '${action}'`);
    }
  }

  private async login(returnUrl: string): Promise<void> {
    const state: INavigationState = { returnUrl };
    const result = await this.authenticationService.signIn(state);
    this.message.next(undefined);

    const applicationPaths = this.authSettingsProvider.settings.applicationPaths;
    switch (result.status) {
      case AuthenticationResultStatus.Redirect:
        break;
      case AuthenticationResultStatus.Success:
        await this.navigateToReturnUrl(returnUrl);
        break;
      case AuthenticationResultStatus.Fail:
        await this.router.navigate(applicationPaths.LoginFailedPathComponents, {
          queryParams: { [QueryParameterNames.Message]: result.message }
        });
        break;
      default:
        throw new Error(`Invalid status result ${(result as any).status}.`);
    }
  }

  private async processLoginCallback(): Promise<void> {
    const url = window.location.href;
    const result = await this.authenticationService.completeSignIn(url);
    switch (result.status) {
      case AuthenticationResultStatus.Redirect:
        // There should not be any redirects as completeSignIn never redirects.
        throw new Error('Should not redirect.');
      case AuthenticationResultStatus.Success:
        await this.navigateToReturnUrl(this.getReturnUrl(result.state));
        break;
      case AuthenticationResultStatus.Fail:
        this.message.next(result.message);
        break;
    }
  }

  private redirectToRegister(): any {
    const applicationPaths = this.authSettingsProvider.settings.applicationPaths;
    this.redirectToApiAuthorizationPath(
      `${applicationPaths.IdentityRegisterPath}?returnUrl=${encodeURI('/' + applicationPaths.Login)}`);
  }

  private redirectToProfile(): void {
    const applicationPaths = this.authSettingsProvider.settings.applicationPaths;
    this.redirectToApiAuthorizationPath(applicationPaths.IdentityManagePath);
  }

  private async navigateToReturnUrl(returnUrl: string) {
    // It's important that we do a replace here so that we remove the callback uri with the
    // fragment containing the tokens from the browser history.
    await this.router.navigateByUrl(returnUrl, {
      replaceUrl: true
    });
  }

  private getReturnUrl(state?: INavigationState): string {
    const applicationPaths = this.authSettingsProvider.settings.applicationPaths;
    const fromQuery = (this.activatedRoute.snapshot.queryParams as INavigationState).returnUrl;
    // If the url is comming from the query string, check that is either
    // a relative url or an absolute url
    if (fromQuery &&
      !(fromQuery.startsWith(`${window.location.origin}/`) ||
        /\/[^\/].*/.test(fromQuery))) {
      // This is an extra check to prevent open redirects.
      throw new Error('Invalid return url. The return url needs to have the same origin as the current page.');
    }
    return (state && state.returnUrl) ||
      fromQuery ||
      applicationPaths.DefaultLoginRedirectPath;
  }

  private redirectToApiAuthorizationPath(apiAuthorizationPath: string) {
    // It's important that we do a replace here so that when the user hits the back arrow on the
    // browser they get sent back to where it was on the app instead of to an endpoint on this
    // component.
    const redirectUrl = `${window.location.origin}${apiAuthorizationPath}`;
    window.location.replace(redirectUrl);
  }
}

interface INavigationState {
  [ReturnUrlType]: string;
}
