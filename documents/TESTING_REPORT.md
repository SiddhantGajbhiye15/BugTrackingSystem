# Bug Tracking System — Testing Report

## Test Information

| Field | Value |
|---|---|
| Project | Bug Tracking System |
| Test Start Date | 2026-08-01 |
| Tester | Siddhant Gajbhiye |
| Frontend | React + Vite |
| Backend | ASP.NET Core Web API |
| Database | SQL Server |
| Frontend URL | `http://localhost:5173` |
| Backend URL | `https://localhost:7294` |

## Pre-Test Baseline

| Check | Status |
|---|---|
| Database cleaned | Passed |
| Users table contains only Admin | Passed |
| Projects table empty | Passed |
| ProjectMembers table empty | Passed |
| Bugs table empty | Passed |
| Comments table empty | Passed |
| Frontend production build | Passed |
| Backend build | Passed |

## Fixed Test Accounts

| Code | Name | Email | Initial Password | Role |
|---|---|---|---|---|
| PM1 | Amit Patil | `amit.qa.pm@example.com` | `Pm@12345` | Project Manager |
| PM2 | Neha Sharma | `neha.qa.pm@example.com` | `Pm@12345` | Project Manager |
| DEV1 | Rahul Verma | `rahul.qa.dev@example.com` | `Dev@12345` | Developer |
| DEV2 | Siddhant Gajbhiye | `siddhant.qa.dev@example.com` | `Dev@12345` | Developer |
| TEST1 | Pranay Patil | `pranay.qa.tester@example.com` | `Test@12345` | Tester |
| TEST2 | Anjali Deshmukh | `anjali.qa.tester@example.com` | `Test@12345` | Tester |

> After password reset testing, DEV1's password becomes `Rahul@2026`.

# Phase 1 — Admin Authentication and User Management

## ADMIN-001 — Admin Login

**Steps**
1. Open `http://localhost:5173/login`.
2. Enter the existing Admin email and password.
3. Click Login.

**Expected Result**
- Login succeeds.
- Admin dashboard opens.
- Admin navigation is visible.

**Actual Result:**  
Admin login succeeded. The Admin Dashboard opened at `/admin/dashboard`.  
The dashboard showed 1 user and 0 projects.  
The Admin account appeared as `System Administrator` with role `Admin`.  
The page continued working correctly after refresh.

**Status:** Passed  

**Notes:**  
No issue found. The Admin name and role are visible in the Recent Users table.

---

## ADMIN-002 — Create PM1

**Input**
- First Name: `Amit`
- Last Name: `Patil`
- Email: `amit.qa.pm@example.com`
- Password: `Pm@12345`
- Role: `Project Manager`

**Expected Result**
- User is created.
- User appears once in the table.
- Role is Project Manager.
- Status is Active.

**Actual Result:**  
PM1 was created successfully. Amit Patil appeared once in the users table with email `amit.qa.pm@example.com`, role `Project Manager`, and status `Active`. The total user count increased from 1 to 2.

**Status:** Passed  

**Notes:**  
No issue found.

---

## ADMIN-003 — Create PM2

**Input**
- First Name: `Neha`
- Last Name: `Sharma`
- Email: `neha.qa.pm@example.com`
- Password: `Pm@12345`
- Role: `Project Manager`

**Expected Result**
- User is created once.
- Role and active status are correct.

**Actual Result:**  
PM2 was created successfully. Neha Sharma appeared exactly once in the users table with email `neha.qa.pm@example.com`, role `Project Manager`, and status `Active`. The total user count increased from 2 to 3.

**Status:** Passed  

**Notes:**  
No issue found.

---

## ADMIN-004 — Create DEV1

**Input**
- First Name: `Rahul`
- Last Name: `Verma`
- Email: `rahul.qa.dev@example.com`
- Password: `Dev@12345`
- Role: `Developer`

**Expected Result**
- User is created once.
- Role is Developer.
- Status is Active.

**Actual Result:**  
DEV1 was created successfully. Rahul Verma appeared exactly once in the users table with email `rahul.qa.dev@example.com`, role `Developer`, and status `Active`. The total user count increased from 3 to 4.

**Status:** Passed  

**Notes:**  
No issue found.

---

## ADMIN-005 — Create DEV2

**Input**
- First Name: `Siddhant`
- Last Name: `Gajbhiye`
- Email: `siddhant.qa.dev@example.com`
- Password: `Dev@12345`
- Role: `Developer`

**Expected Result**
- User is created once.
- Role is Developer.
- Status is Active.

**Actual Result:**  
DEV2 was created successfully. Siddhant Gajbhiye appeared exactly once in the users table with email `siddhant.qa.dev@example.com`, role `Developer`, and status `Active`. The total user count increased from 4 to 5.

**Status:** Passed  

**Notes:**  
No issue found.

---

## ADMIN-006 — Create TEST1

**Input**
- First Name: `Pranay`
- Last Name: `Patil`
- Email: `pranay.qa.tester@example.com`
- Password: `Test@12345`
- Role: `Tester`

**Expected Result**
- User is created once.
- Role is Tester.
- Status is Active.

**Actual Result:**  
TEST1 was created successfully. Pranay Patil appeared exactly once in the users table with email `pranay.qa.tester@example.com`, role `Tester`, and status `Active`. The total user count increased from 5 to 6.

**Status:** Passed  

**Notes:**  
No issue found.

---

## ADMIN-007 — Create TEST2

**Input**
- First Name: `Anjali`
- Last Name: `Deshmukh`
- Email: `anjali.qa.tester@example.com`
- Password: `Test@12345`
- Role: `Tester`

**Expected Result**
- User is created once.
- Role is Tester.
- Status is Active.

**Actual Result:**  
TEST2 was created successfully. Anjali Deshmukh appeared exactly once in the users table with email `anjali.qa.tester@example.com`, role `Tester`, and status `Active`. The total user count increased from 6 to 7.

**Status:** Passed  

**Notes:**  
No issue found.

---

## ADMIN-008 — Duplicate Email Validation

**Input**
- First Name: `Duplicate`
- Last Name: `Manager`
- Email: `amit.qa.pm@example.com`
- Password: `Duplicate@123`
- Role: `Developer`

**Expected Result**
- Creation is rejected.
- Duplicate-email message appears.
- Only one PM1 account remains.

**Actual Result:**  
The duplicate user creation was rejected. The system displayed a duplicate-email error, no additional user was created, the total user count remained 7, and only one account with email `amit.qa.pm@example.com` existed.

**Status:** Passed  

**Notes:**  
Duplicate-email validation worked correctly.

---

## ADMIN-009 — Edit DEV1

**Input**
- Change first name from `Rahul` to `Rahul Updated`.

**Expected Result**
- Name is updated.
- Email and role remain unchanged.
- No duplicate user is created.

**Cleanup**
- Change the first name back to `Rahul`.

**Actual Result:**  
DEV1 was edited successfully. The first name changed from `Rahul` to `Rahul Updated` while the email, role, active status, and total user count remained unchanged. No duplicate user was created. The first name was then changed back to `Rahul` for later test cases.

**Status:** Passed  

**Notes:**  
Edit-user functionality worked correctly.

---

## ADMIN-010 — Reset DEV1 Password

**Input**
- New Password: `Rahul@2026`
- Confirm Password: `Rahul@2026`

**Expected Result**
- Reset succeeds.
- Old password `Dev@12345` fails.
- New password `Rahul@2026` works.

**Actual Result:**  
DEV1's password was reset successfully. Login with the old password `Dev@12345` was rejected, while login with the new password `Rahul@2026` succeeded and opened the Developer Dashboard.

**Status:** Passed  

**Notes:**  
Password reset and login validation worked correctly. DEV1 will use `Rahul@2026` for the remaining tests.

---

## ADMIN-011 — Deactivate DEV2

**Login Input**
- Email: `siddhant.qa.dev@example.com`
- Password: `Dev@12345`

**Expected Result**
- Login is rejected while DEV2 is inactive.

**Actual Result:**  
During the initial test, DEV2 appeared inactive in the frontend, but the database value remained `IsActive = 1`, so the user could still log in.

The frontend status-change logic was corrected to reload users from the backend after the activate/deactivate request. After the fix, deactivating DEV2 changed the database value to `IsActive = 0`. Login using `siddhant.qa.dev@example.com` and `Dev@12345` was rejected.

**Status:** Retest Passed  

**Notes:**  
Initial test failed because the frontend changed only its local displayed state. The backend endpoint worked correctly. The frontend now reloads the saved user status from the backend.

---

## ADMIN-012 — Reactivate DEV2

**Expected Result**
- DEV2 becomes Active.
- DEV2 can log in.
- Developer dashboard opens.

**Actual Result:** Not recorded  
**Status:** Pending  
**Notes:** —

# Remaining Test Phases

| Phase | Scope | Status |
|---|---|---|
| 2 | Project creation, editing, deletion, manager ownership | Pending |
| 3 | Project-member management and one-active-project rule | Pending |
| 4 | Tester bug creation, editing, deletion | Pending |
| 5 | PM priority and assignment workflow | Pending |
| 6 | Developer In Progress and Resolve workflow | Pending |
| 7 | Tester Close and Reopen workflow | Pending |
| 8 | Comments and ownership permissions | Pending |
| 9 | Project Manager transfer | Pending |
| 10 | Authentication, 401 logout, role routes, 404 | Pending |
| 11 | Final regression and production builds | Pending |

# Defect Log

| Defect ID | Test Case ID | Severity | Description | Fix Status | Retest Status |
|---|---|---|---|---|---|
| — | — | — | No defects recorded yet | — | — |

# Result Summary

| Passed | 11 |
| Failed | 0 |
| Blocked | 0 |
| Pending | 1 |
