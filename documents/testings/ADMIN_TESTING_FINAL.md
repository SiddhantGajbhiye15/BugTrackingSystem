# Admin Testing Report

## Test Information

| Field | Value |
|---|---|
| Project | Bug Tracking System |
| Role | Admin |
| Tester | Siddhant Gajbhiye |
| Environment | Local Development |
| Test Date | 2026-08-01 |
| Status | Completed |

## Test Cases

| Test ID | Area | Test Scenario | Expected Result | Actual Result | Status |
|---|---|---|---|---|---|
| ADMIN-001 | Authentication | Login using valid Admin credentials | Admin Dashboard opens and session remains after refresh | Dashboard opened successfully and remained active after refresh | Passed |
| ADMIN-002 | Users | Create Amit Patil as Project Manager | PM account is created once with Active status | Account created correctly | Passed |
| ADMIN-003 | Users | Create Neha Sharma as Project Manager | PM account is created once with Active status | Account created correctly | Passed |
| ADMIN-004 | Users | Create Rahul Verma as Developer | Developer account is created correctly | Account created correctly | Passed |
| ADMIN-005 | Users | Create Siddhant Gajbhiye as Developer | Developer account is created correctly | Account created correctly | Passed |
| ADMIN-006 | Users | Create Pranay Patil as Tester | Tester account is created correctly | Account created correctly | Passed |
| ADMIN-007 | Users | Create Anjali Deshmukh as Tester | Tester account is created correctly | Account created correctly | Passed |
| ADMIN-008 | Validation | Create another user using an existing email | Request is rejected and no duplicate user is created | Duplicate email was rejected | Passed |
| ADMIN-009 | Users | Edit Rahul Verma's name and restore it | Name changes without changing email, role or status | Update and restore succeeded | Passed |
| ADMIN-010 | Password | Reset Rahul Verma's password | Old password stops working and new password works | Password reset worked correctly | Passed |
| ADMIN-011 | Status | Deactivate Siddhant Gajbhiye | Database status becomes inactive and login is blocked | Initial frontend issue was fixed; retest passed | Retest Passed |
| ADMIN-012 | Status | Reactivate Siddhant Gajbhiye | User becomes active and can log in again | Reactivation and login succeeded | Passed |

## Test Accounts Created

| Name | Email | Role |
|---|---|---|
| Amit Patil | `amit.qa.pm@example.com` | Project Manager |
| Neha Sharma | `neha.qa.pm@example.com` | Project Manager |
| Rahul Verma | `rahul.qa.dev@example.com` | Developer |
| Siddhant Gajbhiye | `siddhant.qa.dev@example.com` | Developer |
| Pranay Patil | `pranay.qa.tester@example.com` | Tester |
| Anjali Deshmukh | `anjali.qa.tester@example.com` | Tester |

## Defects

| Defect ID | Test ID | Severity | Description | Resolution | Status |
|---|---|---|---|---|---|
| DEF-001 | ADMIN-011 | High | Frontend showed the user as inactive without reliably loading the saved backend state, allowing login | Reload users from the backend after activate/deactivate operations in `AllUsers.jsx` | Closed — Retest Passed |

## Summary

| Result | Count |
|---|---:|
| Passed | 11 |
| Retest Passed | 1 |
| Failed | 0 |
| Pending | 0 |
| Total | 12 |

**Admin Testing: Completed**