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

Use /// for structured documentation comments that appear above public classes, methods or properties in C#. <br>
Include tags such as (summary), (param), and (returns).

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

- Hosted on GitHub under Alex Cooke's account.
- Every member must clone the main repository to their local machine

## Branch Stucture

- main
  - Contains only reviewed and tested code.
- feature
  - For developing new modules or functionality
    - feature/HAI-(ticket-number)-(ticket-name)
    - e.g. feature/HAI-1-add-seat-module
- fix
  - For fixing bugs
  - fix/(issue)
  - e.g. fix/fixing-seat-error

## Commit Message Format

Short, clear and action based

```pgsql
Added seat validation logic
Fixed JSON loading error in ConfigurationManager
Refactored User class for MFA integration
Updated README with testing instructions
```

## Pull Requests (PR's)

- Each feature branch must be merged into main via a Pull Request (PR)
- PR's must:
  - Include a summary of changes
  - Reference related requiements or issues (if applicable)
  - Be reviewed by at least one other team member

## Code Review Guidelines

Before merging:

- Check naming conventions and comment clarity
- Verify MVVM is maintained
- Ensure any applicable tests pass
- Confirm no console/debug errors
- Review for SOLID and OOP compliance

## Merging Branches

- Never commit directly to main: raise a PR
- Resolve conflicts locally before merging
- Always pull latest changes before pushing to avoid rebasing
