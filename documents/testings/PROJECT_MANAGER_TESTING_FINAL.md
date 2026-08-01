# Project Manager Testing Report

## Test Information

| Field | Value |
|---|---|
| Project | Bug Tracking System |
| Role | Project Manager |
| Tester | Siddhant Gajbhiye |
| Environment | Local Development |
| Test Date | 2026-08-01 |
| Status | Completed |

## Test Accounts

| Code | Name | Email |
|---|---|---|
| PM1 | Amit Patil | `amit.qa.pm@example.com` |
| PM2 | Neha Sharma | `neha.qa.pm@example.com` |
| DEV1 | Rahul Verma | `rahul.qa.dev@example.com` |
| DEV2 | Siddhant Gajbhiye | `siddhant.qa.dev@example.com` |
| TEST1 | Pranay Patil | `pranay.qa.tester@example.com` |
| TEST2 | Anjali Deshmukh | `anjali.qa.tester@example.com` |

## Test Projects

| Project | Code | Final Manager | Final Status |
|---|---|---|---|
| Bug Tracking System QA Updated | `QA-BTS-001` | Amit Patil | Completed |
| Customer Portal QA | `QA-PORTAL-001` | Neha Sharma | Active |
| Temporary Deletion Project | `QA-TEMP-001` | Amit Patil | Deleted |

## Authentication and Access Tests

| Test ID | Test Scenario | Expected Result | Actual Result | Status |
|---|---|---|---|---|
| PM-001 | Login as Amit Patil | Manager Dashboard opens | Login and refresh succeeded | Passed |
| PM-007 | PM2 opens Manage Projects before transfer | PM1 projects must not appear | PM1 projects were hidden | Passed |
| PM-008 | PM2 directly requests PM1 project through API | API returns `403 Forbidden` | Access was rejected | Passed |
| PM-009 | PM opens Admin Users route and API | Admin data must remain inaccessible | Access was blocked, but frontend redirected to Login | Passed with Defect |

## Project Tests

| Test ID | Test Scenario | Expected Result | Actual Result | Status |
|---|---|---|---|---|
| PM-002 | Create project `QA-BTS-001` | Project is created once | Project created correctly | Passed |
| PM-003 | Create project `QA-PORTAL-001` | Project is created once | Project created correctly | Passed |
| PM-004 | Create project using duplicate project code | Duplicate is rejected and message appears in modal | Backend rejected it; modal error placement was fixed | Retest Passed |
| PM-005 | Edit project name and description | Changes persist after refresh | Changes persisted | Passed |
| PM-006 | Create and delete empty temporary project | Empty project can be deleted | Project was created and deleted | Passed |
| PM-029 | Delete project containing members and bugs | Deletion must be blocked | Deletion was blocked | Passed |
| PM-030 | Transfer P2 from Amit to Neha | Manager changes while project data remains | Transfer succeeded, but no success message appeared | Passed with Defect |
| PM-031 | Amit opens P2 after transfer | Access must be removed | P2 disappeared and API access was blocked | Passed |
| PM-032 | Neha opens P2 after transfer | Access must be granted | P2 appeared and could be managed | Passed |
| PM-033 | Mark P1 as Completed after closing bugs | Project becomes Completed | Status changed and persisted | Passed |
| PM-034 | Final Manager regression | Dashboard, projects, members and bugs continue working | All important pages worked correctly | Passed |

## Project Member Tests

| Test ID | Test Scenario | Expected Result | Actual Result | Status |
|---|---|---|---|---|
| PM-010 | Add Pranay, Rahul and Siddhant to P1 | Three unique members are added | Members were added correctly | Passed |
| PM-011 | Add Anjali to P2 | Anjali is added once | Member added correctly | Passed |
| PM-012 | Add Rahul to another active project | Rahul must not be available | Rahul could not be added | Passed |
| PM-013 | Add Pranay to another active project | Pranay must not be available | Pranay did not appear in available users | Passed |
| PM-014 | Add an existing P1 member again | Duplicate membership must be prevented | Existing members were excluded | Passed |
| PM-015 | Remove Anjali from P2 | Member is removed and count becomes zero | Removal succeeded | Passed |
| PM-016 | Add Anjali back to P2 | Member becomes available again | Re-addition succeeded | Passed |

## Bug Assignment and Workflow Tests

| Test ID | Test Scenario | Expected Result | Actual Result | Status |
|---|---|---|---|---|
| PM-017 | View Bug A and Bug B in P1 | Both bugs appear as Open and Unassigned | Both appeared correctly | Passed |
| PM-018 | Change Bug A priority before assignment | Priority changes from Medium to High | Priority changed and persisted | Passed |
| PM-019 | Assign Bug A to Rahul | Assignee becomes Rahul and status becomes Assigned | Assignment succeeded | Passed |
| PM-020 | Change Bug A priority after assignment | Priority change must be blocked | Change was blocked | Passed |
| PM-021 | Assign Bug B to Siddhant | Assignee becomes Siddhant | Assignment succeeded | Passed |
| PM-022 | Assign Bug B to Anjali Tester through API | Invalid assignee must be rejected | API returned `400 Bad Request` | Passed |
| PM-023 | Reassign reopened Bug B from Siddhant to Rahul | Rahul becomes the new assignee | Reassignment succeeded | Passed |
| PM-024 | PM performs Developer-only status transition | API must return `403 Forbidden` | API returned `403 Forbidden` | Passed |

## Comment Tests

| Test ID | Test Scenario | Expected Result | Actual Result | Status |
|---|---|---|---|---|
| PM-025 | Add Manager comment to Bug A | Comment appears under Amit Patil | Comment created successfully | Passed |
| PM-026 | Edit Manager's own comment | Updated comment persists | Comment updated successfully | Passed |
| PM-027 | Delete Manager's own comment | Comment is removed | Comment deleted successfully | Passed |
| PM-028 | Edit or delete Rahul's comment | Manager must not modify another user's comment | Unauthorized changes were blocked | Passed |

## Defects

| Defect ID | Test ID | Severity | Description | Status |
|---|---|---|---|---|
| DEF-002 | PM-004 | Medium | Duplicate project-code error appeared behind the Create Project modal | Closed — Retest Passed |
| DEF-003 | PM-009 | Low | Wrong-role route redirects the logged-in Manager to Login instead of Manager Dashboard or Access Denied | Open |
| DEF-004 | PM-030 | Low | Project Manager transfer succeeds without displaying a success message | Open |

## Summary

| Result | Count |
|---|---:|
| Passed | 31 |
| Retest Passed | 1 |
| Passed with Defect | 2 |
| Failed | 0 |
| Pending | 0 |
| Total | 34 |

**Project Manager Testing: Completed**