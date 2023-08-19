# A Security Blanket for your API

![Nuget](https://img.shields.io/nuget/v/SecurityBlanket)
[![GitHub Workflow Status](https://img.shields.io/github/actions/workflow/status/tspence/security-blanket/dotnet.yml?branch=main)](https://github.com/tspence/security-blanket/actions/workflows/dotnet.yml)

When building an API, you have to always worry about data leaks: is it possible for
a customer to accidentally view data for another customer?  Does your API expose
some information they shouldn't be permitted to see?

You can tell your engineers to check their database queries carefully, but even the
tiniest mistake can leak customer data in a multi-tenant software-as-a-service 
environment.

With SecurityBlanket, you can add a second layer of protection: a middleware filter
that verifies all your data before the API serves up a response.

Here's how to use it.

## Step 1 - Add the middleware to your project

In your `Program.cs` file, mark all controllers to use the security blanket action filter.

```csharp
var builder = WebApplication.CreateBuilder(args);

// Add the Security Blanket middleware to all API responses
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add<SecurityBlanketActionFilter>();
});

var app = builder.Build();
```

## Step 2 - Give your API response objects security rules

Add the `ICustomSecurity` interface to your API response classes.
This interface allows the object to determine whether or not it is permitted to be seen
by the current `HttpContext`.  This independent check will help you ensure that all
database queries produce data the user is entitled to view.

Here's one way you could implement security on your objects:

```csharp
public class MyApiResultObject : ICustomSecurity {
    public int AccountId { get; set; }
    bool IsVisible(HttpContext context)
    {
        return this.AccountId == (int?)context.Items["accountId"];
    }
}
```

If you have nested objects, you'll want to implement `ICompoundSecurity`.  For data
that isn't considered private, tag them with `INoSecurity`.  You can easily audit
all your objects to ensure that each of them has a valid security policy that can be
tested against the API caller's HTTPContext.

## Step 3 - Monitor your logs for security exceptions

If one of your APIs attempts to show an object to a user who isn't entitled to see it,
you will get an exception.  Track these exceptions and make sure that you track down
all the sources of object visibility errors.

## Step 4 - Use SecurityBlanket for custom validation

You can use the validator in your code outside of the API action filter as well:

```csharp
var failures = await Validator.Validate(objectToValidate, context);
```
