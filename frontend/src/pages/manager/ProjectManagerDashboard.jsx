import { useEffect, useState } from "react";
import {
  AlertTriangle,
  Bug,
  FolderKanban,
  LogOut,
  Settings,
  Users,
} from "lucide-react";
import { useNavigate } from "react-router";
import api from "../../api/axios";

function ProjectManagerDashboard() {
  const navigate = useNavigate();

  const [dashboard, setDashboard] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  const storedUser = localStorage.getItem("user");
  const user = storedUser ? JSON.parse(storedUser) : null;

  useEffect(() => {
    async function loadDashboard() {
      try {
        setLoading(true);
        setError("");

        const response = await api.get(
          "/api/dashboard/project-manager"
        );

        setDashboard(response.data);
      } catch (requestError) {
        setError(
          requestError.response?.data?.detail ||
            "Failed to load Project Manager dashboard."
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
              Project Manager Dashboard
            </h1>

            <p className="text-sm text-slate-400">
              Welcome, {user?.firstName} {user?.lastName}
            </p>
          </div>

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
        <section className="grid gap-5 sm:grid-cols-2 xl:grid-cols-4">
          <DashboardCard
            title="Total Projects"
            value={dashboard.totalProjects}
            icon={<FolderKanban size={34} />}
          />

          <DashboardCard
            title="Open Bugs"
            value={dashboard.openBugs}
            icon={<Bug size={34} />}
          />

          <DashboardCard
            title="Unassigned Bugs"
            value={dashboard.unassignedBugs}
            icon={<Users size={34} />}
          />

          <DashboardCard
            title="Critical Bugs"
            value={dashboard.criticalBugs}
            icon={<AlertTriangle size={34} />}
          />
        </section>

        <section className="rounded-2xl border border-slate-800 bg-slate-900 p-5">
          <h2 className="mb-5 text-xl font-semibold">
            Bugs by Status
          </h2>

          <div className="grid gap-4 sm:grid-cols-2 md:grid-cols-3 xl:grid-cols-6">
            {Object.entries(dashboard.bugsByStatus).map(
              ([status, count]) => (
                <div
                  key={status}
                  className="rounded-xl border border-slate-800 bg-slate-950 p-4"
                >
                  <p className="capitalize text-slate-400">
                    {formatStatus(status)}
                  </p>

                  <p className="mt-2 text-2xl font-bold">
                    {count}
                  </p>
                </div>
              )
            )}
          </div>
        </section>

        <DataTable
          title="Projects Overview"
          emptyMessage="No projects are currently assigned to you."
         headers={[
            "Project",
            "Code",
            "Status",
            "Members",
            "Open Bugs",
            "Critical Bugs",
            "Action",
          ]}
        >
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

              <td className="p-4">{project.status}</td>

              <td className="p-4">
                {project.activeMemberCount}
              </td>

              <td className="p-4">
                {project.openBugCount}
              </td>

              <td className="p-4">
                <button
                  onClick={() =>
                    navigate(
                      `/manager/projects/${project.projectId}/members`
                    )
                  }
                  className="flex items-center gap-2 rounded-lg bg-blue-600 px-3 py-2 text-sm font-medium hover:bg-blue-500"
                >
                  <Settings size={16} />
                  Manage Members
                </button>
              </td>
            </tr>
          ))}
        </DataTable>

        <DataTable
          title="Unassigned and Urgent Bugs"
          emptyMessage="No urgent or unassigned bugs."
          headers={[
            "Bug",
            "Project",
            "Priority",
            "Status",
            "Reporter",
            "Developer",
          ]}
        >
          {dashboard.unassignedAndUrgentBugs.map((bug) => (
            <BugRow key={bug.bugId} bug={bug} />
          ))}
        </DataTable>

        <DataTable
          title="Developer Workload"
          emptyMessage="No developers are assigned."
          headers={[
            "Developer",
            "Project",
            "Assigned",
            "In Progress",
            "Resolved",
          ]}
        >
          {dashboard.developerWorkload.map((developer) => (
            <tr
              key={`${developer.projectId}-${developer.developerId}`}
              className="border-t border-slate-800"
            >
              <td className="p-4 font-medium">
                {developer.developerName}
              </td>

              <td className="p-4 text-slate-400">
                {developer.projectName}
              </td>

              <td className="p-4">
                {developer.assignedCount}
              </td>

              <td className="p-4">
                {developer.inProgressCount}
              </td>

              <td className="p-4">
                {developer.resolvedCount}
              </td>
            </tr>
          ))}
        </DataTable>

        <DataTable
          title="Recent Bugs"
          emptyMessage="No bugs have been reported."
          headers={[
            "Bug",
            "Project",
            "Priority",
            "Status",
            "Reporter",
            "Developer",
          ]}
        >
          {dashboard.recentBugs.map((bug) => (
            <BugRow key={bug.bugId} bug={bug} />
          ))}
        </DataTable>
      </div>
    </main>
  );
}

function DashboardCard({ title, value, icon }) {
  return (
    <div className="rounded-2xl border border-slate-800 bg-slate-900 p-6">
      <div className="flex items-center justify-between">
        <div>
          <p className="text-sm text-slate-400">{title}</p>
          <p className="mt-2 text-4xl font-bold">{value}</p>
        </div>

        <div className="text-blue-500">{icon}</div>
      </div>
    </div>
  );
}

function DataTable({
  title,
  headers,
  emptyMessage,
  children,
}) {
  const rows = Array.isArray(children)
    ? children.filter(Boolean)
    : children
      ? [children]
      : [];

  return (
    <section className="overflow-hidden rounded-2xl border border-slate-800 bg-slate-900">
      <div className="border-b border-slate-800 p-5">
        <h2 className="text-xl font-semibold">{title}</h2>
      </div>

      <div className="overflow-x-auto">
        <table className="w-full text-left">
          <thead className="bg-slate-800/50 text-sm text-slate-400">
            <tr>
              {headers.map((header) => (
                <th key={header} className="p-4">
                  {header}
                </th>
              ))}
            </tr>
          </thead>

          <tbody>
            {rows.length > 0 ? (
              children
            ) : (
              <tr>
                <td
                  colSpan={headers.length}
                  className="p-8 text-center text-slate-400"
                >
                  {emptyMessage}
                </td>
              </tr>
            )}
          </tbody>
        </table>
      </div>
    </section>
  );
}

function BugRow({ bug }) {
  return (
    <tr className="border-t border-slate-800">
      <td className="p-4 font-medium">{bug.title}</td>

      <td className="p-4 text-slate-400">
        {bug.projectName}
      </td>

      <td className="p-4">{bug.priority}</td>

      <td className="p-4">{bug.status}</td>

      <td className="p-4 text-slate-400">
        {bug.reporterName}
      </td>

      <td className="p-4">
        {bug.assignedDeveloperName || "Unassigned"}
      </td>
    </tr>
  );
}

function formatStatus(status) {
  return status.replace(/([A-Z])/g, " $1");
}

export default ProjectManagerDashboard;