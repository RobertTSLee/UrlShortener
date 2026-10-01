# Copilot Code Generation Instructions

## Comment Style Requirements

All code generated in this workspace must include structured C# XML documentation comments.

### Class Definitions

Every class must have a documentation comment with the following format:

```csharp
/// <summary>
/// Brief description of what this class does.
/// </summary>
/// <remarks>
/// Additional details about the class, its purpose, and usage patterns if applicable.
/// </remarks>
public class MyClass
{
	// class body
}
```

### Methods and Functions

Every method must have comprehensive documentation:

```csharp
/// <summary>
/// Brief one-line description of what the method does.
/// </summary>
/// <param name="paramName">Description of the parameter and its purpose.</param>
/// <param name="anotherParam">Description of another parameter.</param>
/// <returns>Description of what the method returns.</returns>
/// <remarks>
/// Additional context, edge cases, or usage notes if applicable.
/// </remarks>
/// <example>
/// Example usage if applicable:
/// <code>
/// var result = MyMethod(param1, param2);
/// </code>
/// </example>
public ReturnType MyMethod(string paramName, int anotherParam)
{
	// method body
}
```

### Properties

Properties should have brief documentation:

```csharp
/// <summary>
/// Gets or sets the description of this property.
/// </summary>
public string PropertyName { get; set; }
```

### Bootstrapping Methods

Bootstrapping and initialization methods (e.g., ConfigureServices, Setup, Initialize, Configure) must include:

```csharp
/// <summary>
/// Initializes and configures [component/service name].
/// </summary>
/// <param name="services">The service collection to configure.</param>
/// <returns>The configured service collection.</returns>
/// <remarks>
/// This method sets up all necessary dependencies and configurations for [specific purpose].
/// Call this during application startup before any dependent services are accessed.
/// </remarks>
public static IServiceCollection ConfigureMyServices(this IServiceCollection services)
{
	// bootstrap logic
	return services;
}
```

## Comment Content Guidelines

- **Summaries**: Use clear, concise language (one sentence when possible)
- **Parameters**: Describe the purpose and expected format/constraints
- **Returns**: Explain what is returned and when (null conditions, empty collections, etc.)
- **Remarks**: Include important context, dependencies, or special behavior
- **Examples**: Provide for complex or non-obvious methods

## When to Include Comments

- ✅ All public methods and classes
- ✅ All public properties and fields
- ✅ Bootstrapping methods (Setup, Configure, Initialize, Register, AddServices, etc.)
- ✅ Complex business logic
- ✅ Interface implementations

## Enforce This Everywhere

Apply these standards to:
- Service classes and dependency injection setup
- Controllers and API endpoints
- Entity models and data access layer
- Utility and helper classes
- Extension methods
- Any newly generated code

---

**Last Updated**: Applied to all Copilot generated code in this workspace
