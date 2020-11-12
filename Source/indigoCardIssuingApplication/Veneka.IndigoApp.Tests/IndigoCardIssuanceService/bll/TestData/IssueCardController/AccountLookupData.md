## Testing of the account lookup logic

The GetAccountDetails() on IssuerCardController first call the CBS integration for account details and then passes the account details to the CMS integration for further details.
An attempt to build name on cards will happen if the name on card property is empty.
Validations on account type and currency are performed

| Test                                                                                                                    | Unit Test                               | Test Data                             |
|-------------------------------------------------------------------------------------------------------------------------|-----------------------------------------|---------------------------------------|
| Positive result on account lookup                                                                                       | GetAccountDetailTest()                  | AccountLookupData.cs                  |
| AccountDetails null on successful CBS lookup. Happens if developer has not properly handled error oe returned data      | GetAccountDetailNullTest()              | NullAccountLookupData.cs              |
| ProductField not returned from CBS integration. Must be returned by integration layer, if PrintFields were passed to it | GetAccountDetailNullProductFieldsTest() | NullProductFieldsAccountLookupData.cs |
| ProductField count different from PrintField count. Integration must return same number of fields                       | GetAccountDetailBadProductFieldsTest()  | BadProductFieldsAccountLookupData.cs  |
| Validation of account type mapping. If returned CBS account type not configured for product, return error               | GetAccountDetailAccounTypeNotMappedTest | AccTypeMapAccountLookupData.cs        |
| If NameOnCard supplied from integration bypass BuildNameOnCard                                                          | GetAccountDetailNameOnCardTest()        | NameOnCardAccountLookupData.cs        |
| CBS account lookup failed. Must fail gracefully                                                                         | GetAccountDetailFailedCBSTest()         | FailedCBSAccountLookupData.cs         |
| CMS account lookup failed. Must fail gracefully                                                                         | GetAccountDetailFailedCMSTest()         | FailedCMSAccountLookupData.cs         |
| CMS sets passed in account type to null. Happens if developer doesn'y handle account type correctly                     | GetAccountDetailCmsNullTest()           | CmsNullAccountLookupData.cs           |
| Validation of account currency. If returned currency from CBS isn't configured for the product, return error            | GetAccountDetailCurrencymapTest()       | CurrencyMapAccountLookupData.cs       |
| validate account type... not sure if this is needed                                                                     |                                         |                                       |