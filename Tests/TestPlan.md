# Test Plan — HAI Optimise Jet Interior Configurator (v0)

## Scope
- Unit tests (C#): validation rules (incompatibilities, zone capacity, deprecated materials).
- ViewModel tests: select → validate → save/load (XAML config file).
- Persistence tests: XAML round-trip (save → load → equals).
- Manual checks: XAML bindings, keyboard navigation, error banners.

## Traceability
| Req ID | Description | Planned Test |
|---|---|---|
| R-005 | Reject incompatible textile–seat | UT-VAL-001 |
| R-006 | Enforce zone max capacity | UT-ZONE-002 |
| R-012 | Block deprecated materials | UT-DEPR-003 |
| R-020 | Configure → Validate → Save → Resume | VM-FLOW-004 |

## Next Steps
- need to add `JetConfigTests.cs` with the three rule tests + VM flow test.
- ned to add `XamlConfigStore` round-trip tests.
