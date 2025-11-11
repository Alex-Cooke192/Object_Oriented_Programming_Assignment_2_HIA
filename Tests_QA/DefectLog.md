Jet Interior Design System – Defect Log (HT / QA)

| Defect ID | Component / Test Area             | Description of Defect                                                                 | Severity | Status     | Fixed By    | Validation Result | Notes |
||
| D-001      | ConfigurationManager              | NullReferenceException triggered when loading empty JSON configuration                 | High      | Resolved   | Alex     | Retested – Pass   | Added null-check validation |
| D-002      | Authentication                    | Password field accepted whitespace-only values                                         | Medium    | Resolved   | Hannah   | Retested – Pass   | Fixed by adding `Trim()` in input check |
| D-003      | UI Rendering                      | Seat duplication bug after namespace refactor                                          | High      | Resolved   | Isobel   | Verified – Pass   | Regression-tested in v2 build |
| D-004      | 
| D-005      | Validation Layer (Dimensions)     | Input allowed negative seat width                                                      | Low       | Resolved   | Hannah   | Retested – Pass   | Boundary test updated |
| D-006      | Navigation Service (LoginView)    | Navigation not returning to dashboard on failed login                                  | Low       | Active     | Hannah   | Pending Fix       | UI logic dependency mock failing in testhost |
| D-007      | System Stability / Build          | Testhost.dll not launching under BAE VPN                                               | Medium    | Workaround | Hannah   | N/A               | Manual `dotnet test` used on Mac |

---

 Summary


All resolved defects verified by test cases under `RegressionTests.cs`, `ValidationAndBoundaryTests.cs`, and `IntegrationVerification.cs`.  
Evidence attached in **EvidenceMapping.md** and screenshots of successful `dotnet test` run.
