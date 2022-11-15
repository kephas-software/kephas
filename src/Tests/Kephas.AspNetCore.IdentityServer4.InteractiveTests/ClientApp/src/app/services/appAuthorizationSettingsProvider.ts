import { AuthenticationSettingsProvider } from "@kephas/angular-oidc";
import { AppService, Priority } from "@kephas/core";

@AppService({ overridePriority: Priority.Highest })
export class AppAuthenticationSettingsProvider extends AuthenticationSettingsProvider {
  public static readonly instance = new AppAuthenticationSettingsProvider();
  /**
   * Creates an instance of AppAuthenticationSettingsProvider.
   * @memberof AppAuthenticationSettingsProvider
   */
  constructor() {
    super("my-id-server-test");
  }
}
