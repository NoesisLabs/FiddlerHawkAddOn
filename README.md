Hawk Authentication AddOn
============================

This Fiddler AddOn provides users with the ability to pass [Hawk Authentication](https://github.com/hueniverse/hawk) headers as part of their requests.


Installation for Fiddler4 (Beta)
------------

Copy the files `NoesisLabs.Fiddler.HawkAddOn.dll`, `Thinktecture.IdentityModel.Core.dll`, and `Thinktecture.IdentityModel.Hawk.dll` from the folder `NoesisLabs.Fiddler.HawkAddOn\bin\Release\`
into `C:\Program Files\Fiddler2\Scripts` and restart Fiddler.


Usage
----------

1. Navigate to `Hawk Authentication` tab within Fiddler
2. Check the `Enable` checkbox to begin including Hawk Authentication headers
3. Check the `Composer Only` checkbox if you would like to limit the Hawk Authentication header inclusion to requests sent via Fiddler's Composer -> Parsed tool.
4. Choose the correct encryption algorithm for your authentication provider
5. Enter a valid Hawk Id for your authentication provider
6. Enter a valid Hake Key for your authentication provider
