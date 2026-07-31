import { Navigate, Route, Routes } from "react-router";
import Login from "./pages/Login";
import ProtectedRoute from "./components/ProtectedRoute";
import AdminDashboard from "./pages/admin/AdminDashboard";
import AllUsers from "./pages/admin/AllUsers";
import AllProjects from "./pages/admin/AllProjects";
import ProjectManagerDashboard from "./pages/manager/ProjectManagerDashboard";
import ManagerProjects from "./pages/manager/ManagerProjects";
import ManageProjectMembers from "./pages/manager/ManageProjectMembers";
import ManagerBugs from "./pages/manager/ManagerBugs";
import TesterDashboard from "./pages/tester/TesterDashboard";
import CreateBug from "./pages/tester/CreateBug";
import EditBug from "./pages/tester/EditBug";
import DeveloperDashboard from "./pages/developer/DeveloperDashboard";
import BugDetails from "./pages/bugs/BugDetails";

function App() {
  return (
    <Routes>
      <Route path="/login" element={<Login />} />

      <Route
        path="/admin/dashboard"
        element={
          <ProtectedRoute allowedRoles={[1]}>
            <AdminDashboard />
          </ProtectedRoute>
        }
      />

      <Route
        path="/admin/users"
        element={
          <ProtectedRoute allowedRoles={[1]}>
            <AllUsers />
          </ProtectedRoute>
        }
      />

      <Route
        path="/admin/projects"
        element={
          <ProtectedRoute allowedRoles={[1]}>
            <AllProjects />
          </ProtectedRoute>
        }
      />

      <Route
        path="/manager/dashboard"
        element={
          <ProtectedRoute allowedRoles={[2]}>
            <ProjectManagerDashboard />
          </ProtectedRoute>
        }
      />

      <Route
        path="/manager/projects"
        element={
          <ProtectedRoute allowedRoles={[2]}>
            <ManagerProjects />
          </ProtectedRoute>
        }
      />

      <Route
        path="/manager/projects/:projectId/members"
        element={
          <ProtectedRoute allowedRoles={[2]}>
            <ManageProjectMembers />
          </ProtectedRoute>
        }
      />

      <Route
        path="/manager/projects/:projectId/bugs"
        element={
          <ProtectedRoute allowedRoles={[2]}>
            <ManagerBugs />
          </ProtectedRoute>
        }
      />

      <Route
        path="/developer/dashboard"
        element={
          <ProtectedRoute allowedRoles={[3]}>
            <DeveloperDashboard />
          </ProtectedRoute>
        }
      />

      <Route
        path="/tester/dashboard"
        element={
          <ProtectedRoute allowedRoles={[4]}>
            <TesterDashboard />
          </ProtectedRoute>
        }
      />

      <Route
        path="/tester/projects/:projectId/bugs/create"
        element={
          <ProtectedRoute allowedRoles={[4]}>
            <CreateBug />
          </ProtectedRoute>
        }
      />

      <Route
        path="/tester/bugs/:bugId/edit"
        element={
          <ProtectedRoute allowedRoles={[4]}>
            <EditBug />
          </ProtectedRoute>
        }
      />

      <Route
        path="/bugs/:bugId"
        element={
          <ProtectedRoute allowedRoles={[2, 3, 4]}>
            <BugDetails />
          </ProtectedRoute>
        }
      />

      <Route path="*" element={<Navigate to="/login" replace />} />
    </Routes>
  );
}

export default App;