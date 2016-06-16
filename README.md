ober
====

[![Build status](https://img.shields.io/appveyor/ci/janjoris/ober/master.svg)](https://ci.appveyor.com/project/JanJoris/ober) [![Coverage](https://img.shields.io/coveralls/JanJoris/ober.svg)](https://coveralls.io/github/JanJoris/ober) [![GitHub release](https://img.shields.io/github/release/janjoris/ober.svg)](https://github.com/JanJoris/ober/releases) [![chocolatey](https://img.shields.io/chocolatey/v/ober.svg)](https://chocolatey.org/packages/ober) 

##  Automate your Windows Store App deploys

This CLI tool is made to help you make use of the Windows Store Submission API. The setup is similar to the Analytics API, a guide can be found [here](https://blogs.windows.com/buildingapps/2016/03/01/windows-store-analytics-api-now-available/).

Just make sure the App ID URL is `https://manage.devcenter.microsoft.com` and not anything app specific. This will also allow access to the analytics API, the documentation is a bit off.

My steps were:

* Navigate to the [old Azure portal](https://manage.windowsazure.com) and select the correct directory (the one associated with the Store)
* Select Applications
* Add new and select `Add an application my organization is developing`
* Enter a name and select ` Web application and/or web API`
* Sign on URL doesn't matter, APP ID URL is `https://manage.devcenter.microsoft.com`
* Wait for it to configure
* Tap CONFIGURE
* You will have to create a new key and copy that and the Client ID
* Get the tenant ID from the URL above: `https://manage.windowsazure.com/@<your_tenant_name>#workspaces/ActiveDirectoryExtension/Directory/<your_tenant_ID>/direcotryQuickStart`
* Add the application by selecting it from the list in your Windows Store's settings (Manage Users)

## Configuration

Ober get's it's settings from a [YAML](http://yaml.org/) config file called .oberconfig in your user's root folder (~\.oberconfig). Create or edit that file to get started, the template is as follows:

```yaml
Credentials:
  ClientId: <clientID>
  Key: <key>
  TenantId: <tenantID>
```

## Usage

Currently only the submit command has been implemented, it can be used to deploy packages to the Store for both regular releases and flights. The submission API wants all assets bundled in a `.zip` file, so provide all `.appxupload` files in the zip. Ober will not allow the submission when no packages are found inside of the zip file.

#### submit

##### release

```bash
ober submit -a "<appID>" -p "<zip location>"
```
##### flight

```bash
ober submit -a "<appID>" -f "<flightID>" -p "<zip location>"
```

If you need a more detailed log (WIP, it might break the progress indicators for now), you can use `-v` to toggle verbose logging.

Jan-Joris, Ober and Out!
