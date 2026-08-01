# Developer Testing Report

## Test Information

| Field | Value |
|---|---|
| Project | Bug Tracking System |
| Role | Developer |
| Developer 1 | Rahul Verma |
| Developer 2 | Siddhant Gajbhiye |
| Project | `QA-BTS-001` |
| Environment | Local Development |
| Test Date | 2026-08-01 |
| Status | Completed |

## Scope

This report contains only the Developer functionality executed during the MVP workflow:

- Developer login
- viewing assigned bugs
- changing valid Developer statuses
- resolving bugs
- adding investigation and resolution comments
- handling a reopened and reassigned bug

Unexecuted and repetitive permission combinations are excluded.

## Authentication and Assigned Bug Access

| Test ID | Test Scenario | Expected Result | Actual Result | Status |
|---|---|---|---|---|
| DEV-001 | Login as Rahul using reset password | Developer Dashboard opens | Login succeeded using `Rahul@2026` | Passed |
| DEV-002 | Login as Siddhant | Developer Dashboard opens | Login succeeded | Passed |
| DEV-003 | Rahul opens assigned Bug A | Assigned bug details are visible | Bug A opened correctly | Passed |
| DEV-004 | Siddhant opens assigned Bug B | Assigned bug details are visible | Bug B opened correctly | Passed |

## Bug Status Workflow Tests

| Test ID | Test Scenario | Expected Result | Actual Result | Status |
|---|---|---|---|---|
| DEV-005 | Rahul changes Bug A from Assigned to In Progress | Status becomes In Progress | Status changed successfully | Passed |
| DEV-006 | Rahul changes Bug A from In Progress to Resolved | Status becomes Resolved | Status changed successfully | Passed |
| DEV-007 | Siddhant changes Bug B from Assigned to In Progress | Status becomes In Progress | Status changed successfully | Passed |
| DEV-008 | Siddhant changes Bug B from In Progress to Resolved | Status becomes Resolved | Status changed successfully | Passed |
| DEV-009 | Rahul receives reopened Bug B through reassignment | Bug appears under Rahul as assigned work | Reassignment succeeded | Passed |
| DEV-010 | Rahul changes reassigned Bug B to In Progress and Resolved | Bug completes the Developer workflow again | Status transitions succeeded | Passed |

## Comment Tests

| Test ID | Test Scenario | Expected Result | Actual Result | Status |
|---|---|---|---|---|
| DEV-011 | Rahul adds investigation comment to Bug A | Comment appears under Rahul Verma | Comment was created successfully | Passed |
| DEV-012 | Siddhant adds implementation comment to Bug B | Comment appears under Siddhant Gajbhiye | Comment was created successfully | Passed |
| DEV-013 | Rahul adds final fix comment to reassigned Bug B | Comment appears and remains after refresh | Comment was created successfully | Passed |

## Comments Used

### Rahul — Bug A

```text
Root cause identified in the login submit handler.