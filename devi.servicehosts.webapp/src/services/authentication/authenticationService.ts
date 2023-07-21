// OIDC client
import { Log, User, UserManager, WebStorageStateStore } from 'oidc-client-ts'

// Services
import { ConfigurationValues } from '../core/configurationService'

export default class AuthenticationService {
    private _userManager: UserManager

    constructor() {
        const settings = {
            authority: ConfigurationValues.authority,
            client_id: ConfigurationValues.clientId,
            client_secret: ConfigurationValues.clientSecret,
            redirect_uri: ConfigurationValues.redirectUri,
            silent_redirect_uri: ConfigurationValues.silentRedirectUri,
            post_logout_redirect_uri: ConfigurationValues.postLogoutRedirectUri,
            response_type: 'code',
            scope: 'openid profile api_public_v1',
            userStore: new WebStorageStateStore({ store: window.localStorage })
        }
        this._userManager = new UserManager(settings)

        Log.setLogger(console)
        Log.setLevel(Log.WARN)

        this._userManager.events.addAccessTokenExpired(() => console.log('Access token expired!'))
    }

    public getUser(): Promise<User | null> {
        return this._userManager.getUser()
    }

    public signin(): Promise<void> {
        return this._userManager.signinRedirect()
    }

    public signinCallback(): Promise<User> {
        return this._userManager.signinRedirectCallback()
    }

    public signinSilent(): Promise<User | null> {
        return this._userManager.signinSilent()
    }

    public signinSilentCallback(): Promise<void> {
        return this._userManager.signinSilentCallback()
    }

    public async signout(): Promise<void> {
        const user = await this._userManager.getUser()

        if (user != null) {
            return this._userManager.signoutRedirect()
        }
    }
}
