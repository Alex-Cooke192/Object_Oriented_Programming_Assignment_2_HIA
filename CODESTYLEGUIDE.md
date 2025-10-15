# Code Styling Guide

## Namespaces

PascalCase

```C#
JetConfigurationSystem.Models
```

## Classes / Interfaces

PascalCase

```C#
JetConfiguration, IValidatable
```

## Methods

PascalCase()

```C#
SaveConfig(), ValidateLayout()
```

## Properties

PascalCase

```C#
SeatingCapacity, UserName
```

## Private Fields

camelCase

```C#
_configManager, _isValid
```

## Local Variables

camelCase

```C#
seatCount, errorList
```

## Constants / Enums

PascalCase

```C#
SeatTier.Business
```

## Events / Delegates

PascalCase, suffix with "Event"

```C#
PropertyChanged, ValidationCompletedEvent
```

## Async Methods

PascalCase, suffix with "Async"

```C#
SaveConfigAsync()
```

# Commenting and Documentation

Use /// for structured documentation comments that appear above public classes, methods or properties in C#.
Include tags such as <summary>, <param>, and <returns>.

```C#
/// <summary>
/// Validates the current jet configuration against safety rules.
/// </summary>
/// <returns>True if the configuration passes all checks; otherwise, false.</returns>
public bool ValidateLayout()
{
// Validation logic here
return true;
}
```

Use // for inline comments of short explanatory notes placed directly above or beside a specific line or block of code.

Comments should:

- Explain intent or reasoning
- Clarify complex algorithms or edge cases
- Be brief and relevant
- Avoid stating the obvious

```C#
// Ensure aisle width meets minimum requirement
if (aisleWidth < MinAisleWidth)
    LogError("Aisle too narrow");

// Clamp seat recline angle to safe range
seat.ReclineAngle = Math.Clamp(seat.ReclineAngle, 0, MaxRecline);

```

# Git Workflow and Branch Naming Conventions

## Repository

## Branch Stucture

## Commit Message Format

## Pull Requests (PR's)

## Code Review Guidelines

## Merging Branches

##
