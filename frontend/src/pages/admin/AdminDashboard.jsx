import { useEffect, useState } from "react";
import { FolderKanban, LogOut, Users } from "lucide-react";
import { useNavigate } from "react-router";
import api from "../../api/axios";

function AdminDashboard() {
  const navigate = useNavigate();

  const [dashboard, setDashboard] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  useEffect(() => {
    async function loadDashboard() {
      try {
        setLoading(true);
        setError("");

        const response = await api.get("/api/dashboard/admin");

        setDashboard(response.data);
      } catch (error) {
        setError(
          error.response?.data?.detail ||
            "Failed to load admin dashboard."
        );
      } finally {
        setLoading(false);
      }
    }

    loadDashboard();
  }, []);

  function handleLogout() {
    localStorage.removeItem("token");
    localStorage.removeItem("user");
    navigate("/login");
  }

  if (loading) {
    return (
      <div className="flex min-h-screen items-center justify-center bg-slate-950 text-white">
        Loading dashboard...
      </div>
    );
  }

  if (error) {
    return (
      <div className="flex min-h-screen items-center justify-center bg-slate-950">
        <p className="rounded-xl bg-red-500/10 p-4 text-red-400">
          {error}
        </p>
      </div>
    );
  }

  return (
    <main className="min-h-screen bg-slate-950 text-white">
      <header className="border-b border-slate-800 bg-slate-900 px-6 py-4">
        <div className="mx-auto flex max-w-7xl items-center justify-between">
          <div>
            <h1 className="text-2xl font-bold">
              Admin Dashboard
            </h1>

            <p className="text-sm text-slate-400">
              Manage users and projects
            </p>
          </div>
          <button
            onClick={() => navigate("/admin/users")}
            className="rounded-lg bg-blue-600 px-4 py-2 font-medium hover:bg-blue-500"
          >
            All Users
          </button>
          
          <button
            onClick={() => navigate("/admin/projects")}
            className="rounded-lg bg-violet-600 px-4 py-2 font-medium hover:bg-violet-500"
          >
            All Projects
          </button>

          <button
            onClick={handleLogout}
            className="flex items-center gap-2 rounded-lg bg-red-600 px-4 py-2 font-medium hover:bg-red-500"
          >

            <LogOut size={18} />
            Logout
          </button>
        </div>
      </header>

      <div className="mx-auto max-w-7xl space-y-8 p-6">
        <section className="grid gap-5 sm:grid-cols-2">
          <div className="rounded-2xl border border-slate-800 bg-slate-900 p-6">
            <div className="flex items-center justify-between">
              <div>
                <p className="text-sm text-slate-400">
                  Total Users
                </p>

                <p className="mt-2 text-4xl font-bold">
                  {dashboard.totalUsers}
                </p>
              </div>

              <Users
                size={38}
                className="text-blue-500"
              />
            </div>
          </div>

          <div className="rounded-2xl border border-slate-800 bg-slate-900 p-6">
            <div className="flex items-center justify-between">
              <div>
                <p className="text-sm text-slate-400">
                  Total Projects
                </p>

                <p className="mt-2 text-4xl font-bold">
                  {dashboard.totalProjects}
                </p>
              </div>

              <FolderKanban
                size={38}
                className="text-violet-500"
              />
            </div>
          </div>
        </section>

        <section className="rounded-2xl border border-slate-800 bg-slate-900">
          <div className="border-b border-slate-800 p-5">
            <h2 className="text-xl font-semibold">
              Recent Users
            </h2>
          </div>

          <div className="overflow-x-auto">
            <table className="w-full text-left">
              <thead className="bg-slate-800/50 text-sm text-slate-400">
                <tr>
                  <th className="p-4">Name</th>
                  <th className="p-4">Email</th>
                  <th className="p-4">Role</th>
                  <th className="p-4">Current Project</th>
                  <th className="p-4">Status</th>
                </tr>
              </thead>

              <tbody>
                {dashboard.recentUsers.map((user) => (
                  <tr
                    key={user.userId}
                    className="border-t border-slate-800"
                  >
                    <td className="p-4 font-medium">
                      {user.fullName}
                    </td>

                    <td className="p-4 text-slate-400">
                      {user.email}
                    </td>

                    <td className="p-4">
                      {user.role}
                    </td>

                    <td className="p-4 text-slate-400">
                      {user.currentProject}
                    </td>

                    <td className="p-4">
                      <span
                        className={
                          user.isActive
                            ? "rounded-full bg-green-500/10 px-3 py-1 text-sm text-green-400"
                            : "rounded-full bg-red-500/10 px-3 py-1 text-sm text-red-400"
                        }
                      >
                        {user.isActive ? "Active" : "Inactive"}
                      </span>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </section>

        <section className="rounded-2xl border border-slate-800 bg-slate-900">
          <div className="border-b border-slate-800 p-5">
            <h2 className="text-xl font-semibold">
              Projects Overview
            </h2>
          </div>

          <div className="overflow-x-auto">
            <table className="w-full text-left">
              <thead className="bg-slate-800/50 text-sm text-slate-400">
                <tr>
                  <th className="p-4">Project</th>
                  <th className="p-4">Code</th>
                  <th className="p-4">Manager</th>
                  <th className="p-4">Members</th>
                  <th className="p-4">Status</th>
                </tr>
              </thead>

              <tbody>
                {dashboard.projectsOverview.map((project) => (
                  <tr
                    key={project.projectId}
                    className="border-t border-slate-800"
                  >
                    <td className="p-4 font-medium">
                      {project.projectName}
                    </td>

                    <td className="p-4 text-slate-400">
                      {project.projectCode}
                    </td>

                    <td className="p-4">
                      {project.projectManagerName}
                    </td>

                    <td className="p-4">
                      {project.activeMemberCount}
                    </td>

                    <td className="p-4">
                      <span className="rounded-full bg-blue-500/10 px-3 py-1 text-sm text-blue-400">
                        {project.status}
                      </span>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </section>
      </div>
    </main>
  );
}

export default AdminDashboard;