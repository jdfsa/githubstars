// The file contents for the current environment will overwrite these during build.
// The build system defaults to the dev environment which uses `environment.ts`, but if you do
// `ng build --env=prod` then `environment.prod.ts` will be used instead.
// The list of which env maps to which file can be found in `.angular-cli.json`.

export const environment = {
  production: false,
  appData: {
    clientId: 'b4a16d2a11c38dd95924',
    redirectUri: 'http://localhost:4200/authorize'
  },
  endpoints: {
    authorize: 'http://localhost:62491/api/auth/url',
    token: 'http://localhost:62491/api/auth/token',
    user: 'http://localhost:62491/api/user'
  }
};
