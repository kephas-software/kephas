import { environment } from '../environments/environment';
import { StaticProvider, enableProdMode } from '@angular/core';
import { platformBrowserDynamic } from '@angular/platform-browser-dynamic';

import { AppServiceInfoRegistry } from '@kephas/core';
import { AngularAppServiceInfoRegistry } from '@kephas/angular';

import { AppModule } from './app.module';

/**
 * Gets the base URL.
 *
 * @export
 * @returns
 */
export function getBaseUrl() {
  return document.getElementsByTagName('base')[0].href;
}

/**
 * Interface for application context.
 */
export interface AppContext {
  /**
   * Gets the root providers.
   */
  readonly rootProviders: StaticProvider[];
}

/**
 * The application root class.
 */
export class App {

  /**
   * Bootstraps the application asynchronously
   * @returns {Promise{AppContext}} A promise of the application context.
   */
  public async bootstrapAsync(): Promise<AppContext> {

    // first of all load the modules to make sure the services are appropriately set up.
    await this.loadModules();

    const serviceRegistry = new AngularAppServiceInfoRegistry(AppServiceInfoRegistry.Instance);
    serviceRegistry.registerServices();
    const providers = serviceRegistry.getRootProviders() as StaticProvider[];
    providers.push({ provide: 'BASE_URL', useFactory: getBaseUrl, deps: [] });

    if (environment.production) {
      enableProdMode();
    }

    const appModuleRef = await platformBrowserDynamic(providers).bootstrapModule(AppModule);

    return {
      rootProviders: providers
    };
  }

  protected async loadModules(): Promise<void> {
    await import('@kephas/core');
    await import('@kephas/reflection');
    await import('@kephas/commands');
    await import('@kephas/messaging');
    await import('@kephas/ui');

    await import('@angular/common/http');
    await import('@kephas/angular');
    await import('@kephas/angular-oidc');

    // await import('./app.resolver');
    // await import('./app.state');
    // await import('./app.initializer');

    // await import('./services/defaultUserProfile.service');
    await import('./services/appAuthorizationSettingsProvider');

    await import('bootstrap');
  }
}
