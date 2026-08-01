import { Navigate } from "react-router";

const dashboardRoutes = {
  1: "/admin/dashboard",
  2: "/manager/dashboard",
  3: "/developer/dashboard",
  4: "/tester/dashboard",
};

function ProtectedRoute({ allowedRoles = [], children }) {
  const token = localStorage.getItem("token");
  const storedUser = localStorage.getItem("user");

  if (!token || !storedUser) {
    return <Navigate to="/login" replace />;
  }

  let user;

  try {
    user = JSON.parse(storedUser);
  } catch {
    localStorage.removeItem("token");
    localStorage.removeItem("user");
    return <Navigate to="/login" replace />;
  }

  const userRole = Number(user?.role);
  const userDashboard = dashboardRoutes[userRole];

  if (!userDashboard) {
    localStorage.removeItem("token");
    localStorage.removeItem("user");
    return <Navigate to="/login" replace />;
  }

  if (!allowedRoles.includes(userRole)) {
    return <Navigate to={userDashboard} replace />;
  }

  return children;
}

export default ProtectedRoute;