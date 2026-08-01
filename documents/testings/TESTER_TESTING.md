# Tester Testing Report

## Test Information

| Field | Value |
|---|---|
| Project | Bug Tracking System |
| Role | Tester |
| Primary Tester Account | Pranay Patil |
| Email | `pranay.qa.tester@example.com` |
| Project | `QA-BTS-001` |
| Environment | Local Development |
| Test Date | 2026-08-01 |
| Status | Completed |

## Scope

This report contains only the logical Tester functionality executed during the MVP workflow:

- Tester login and project access
- bug creation
- bug verification
- bug reopening
- bug closing
- bug comments

Repeated permission combinations and unexecuted test cases are intentionally excluded.

## Authentication and Project Access

| Test ID | Test Scenario | Expected Result | Actual Result | Status |
|---|---|---|---|---|
| TESTER-001 | Login as Pranay Patil | Tester Dashboard opens | Login succeeded | Passed |
| TESTER-002 | Open assigned project P1 | Tester can access `QA-BTS-001` | Project opened correctly | Passed |

## Bug Creation Tests

| Test ID | Test Scenario | Expected Result | Actual Result | Status |
|---|---|---|---|---|
| TESTER-003 | Create Bug A: Login button does not respond | Bug is created with Open status and no assignee | Bug A was created correctly | Passed |
| TESTER-004 | Create Bug B: Dashboard bug count does not refresh | Bug is created with Open status and no assignee | Bug B was created correctly | Passed |
| TESTER-005 | Open created bug details | Reporter, priority, status and description are visible | Bug details appeared correctly | Passed |

## Bug Verification and Status Tests

| Test ID | Test Scenario | Expected Result | Actual Result | Status |
|---|---|---|---|---|
| TESTER-006 | Verify resolved Bug A and close it | Status changes from Resolved to Closed | Bug A was closed | Passed |
| TESTER-007 | Verify Bug B and reopen it because issue remains | Status changes from Resolved to Reopened | Bug B was reopened | Passed |
| TESTER-008 | Verify Bug B after second fix and close it | Status changes from Resolved to Closed | Bug B was closed | Passed |

## Comment Tests

| Test ID | Test Scenario | Expected Result | Actual Result | Status |
|---|---|---|---|---|
| TESTER-009 | Add failed-verification comment to Bug B | Comment appears under Pranay Patil | Comment was created successfully | Passed |
| TESTER-010 | Add successful-verification comment to Bug A | Comment appears and remains visible | Comment was created successfully | Passed |
| TESTER-011 | Add final verification comment to Bug B | Comment appears before closing the bug | Comment was created successfully | Passed |

## Test Data

### Bug A

| Field | Value |
|---|---|
| Title | Login button does not respond |
| Initial Priority | Medium |
| Final Priority | High |
| Reporter | Pranay Patil |
| Developer | Rahul Verma |
| Final Status | Closed |

### Bug B

| Field | Value |
|---|---|
| Title | Dashboard bug count does not refresh |
| Priority | Low |
| Reporter | Pranay Patil |
| Initial Developer | Siddhant Gajbhiye |
| Reassigned Developer | Rahul Verma |
| Final Status | Closed |

## Defects

No Tester-specific defects were found during the executed MVP workflow.

## Summary

| Result | Count |
|---|---:|
| Passed | 11 |
| Failed | 0 |
| Pending | 0 |
| Total | 11 |

**Tester Testing: Completed**