# Extensions - Validation

This package introduces primary support for validation.

The validation uses both the [Data Annotations](https://learn.microsoft.com/en-us/previous-versions/aspnet/ee256141(v=vs.100)?redirectedfrom=MSDN) and [FluentValidation](https://docs.fluentvalidation.net/en/latest/).

## Usage

#### Example

```csharp
    var serviceProvider = new ServiceCollection()
        .AddApplicationServices(configuration)
        .BuildServiceProvider();
    
    await using var scope = serviceProvider.CreateAsyncScope()
    var validationService = scope.ServiceProvider.GetRequiredService<IValidationService>();

    // USE CASE: get the validation result.    
    var result = validationService.Validate(myRequest);
    // alternatively, can use the async variation.
    var asyncResult = await validationService.ValidateAsync(myRequest);

    // USE CASE: validate and throw a ValidationException if not valid.    
    var result = validationService.EnsureValid(myRequest);
    // alternatively, can use the async variation.
    var asyncResult = await validationService.EnsureValidAsync(myRequest);
```

### Creating validators

When using the `IValidationService`, both fluent validation and data annotations can be used, single or combined.
All the validation rules will be applied.

#### Example

```csharp
public interface IPerson
{
    int Age { get; set; }
}

public class PersonEntity : IPerson
{
    int Age { get; set; }
    string? FamilyName { get; set; }
    string? GivenName { get; set; }
}

// Do the validation by using the data annotations
public class PersonDto : IPerson
{
    int Age { get; set; }
    [Required]
    [MinLength(2)]
    string FullName { get; set; } = null!;
}

/// <summary>
/// This validator will be applied to all instances inheriting from IPerson.
/// <summary>
public class PersonValidator : AbstractValidator<IPerson>
{
    public PersonValidator()
    {
        RuleFor(e => e.Age).InclusiveBetween(18, 130);
    }
}

/// <summary>
/// This validator will be applied to all PersonEntity instances.
/// <summary>
public class PersonEntityValidator : AbstractValidator<PersonEntity>
{
    public PersonEntityValidator()
    {
        RuleFor(e => e.FamilyName).MaxLength(100);
        RuleFor(e => e.GivenName).MaxLength(50);
    }
}
```

## Other resources

* [Kephas.Exceptions](https://www.nuget.org/packages/Kephas.Exceptions)
* [Kephas.DependencyInjection.Services](https://www.nuget.org/packages/Kephas.DependencyInjection.Services)
* [Kephas.Analyzers](https://www.nuget.org/packages/Kephas.Analyzers)

> Kephas Framework ("stone" in aramaic) aims to deliver a solid infrastructure for applications and application ecosystems.

