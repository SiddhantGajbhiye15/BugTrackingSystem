import { useEffect, useState } from "react";
import {
  AlertCircle,
  Bug,
  CheckCircle2,
  FolderKanban,
  LogOut,
  Play,
  RefreshCcw,
} from "lucide-react";
import { useNavigate } from "react-router";
import api from "../../api/axios";

function DeveloperDashboard() {
  const navigate = useNavigate();

  const [dashboard, setDashboard] = useState(null);
  const [loading, setLoading] = useState(true);
  const [updatingBugId, setUpdatingBugId] = useState(null);
  const [error, setError] = useState("");
  const [success, setSuccess] = useState("");

  useEffect(() => {
    loadDashboard();
  }, []);

  async function loadDashboard() {
    try {
      setLoading(true);
      setError("");

      const response = await api.get(
        "/api/dashboard/developer"
      );

      setDashboard(response.data);
    } catch (requestError) {
      setError(
        requestError.response?.data?.detail ||
          requestError.response?.data?.message ||
          "Failed to load Developer dashboard."
      );
    } finally {
      setLoading(false);
    }
  }

  async function changeBugStatus(bugId, status) {
    try {
      setUpdatingBugId(bugId);
      setError("");
      setSuccess("");

      await api.patch(`/api/bugs/${bugId}/status`, {
        status,
      });

      setSuccess(
        status === 3
          ? "Bug moved to In Progress."
          : "Bug marked as Resolved."
      );

      await loadDashboard();
    } catch (requestError) {
      setError(
        requestError.response?.data?.detail ||
          requestError.response?.data?.message ||
          "Failed to change bug status."
      );
    } finally {
      setUpdatingBugId(null);
    }
  }

  function handleLogout() {
    localStorage.removeItem("token");
    localStorage.removeItem("user");
    navigate("/login");
  }

  function getPriorityStyle(priority) {
    const styles = {
      Low: "bg-green-500/10 text-green-400",
      Medium: "bg-yellow-500/10 text-yellow-400",
      High: "bg-orange-500/10 text-orange-400",
      Critical: "bg-red-500/10 text-red-400",
    };

    return (
      styles[priority] ||
      "bg-slate-500/10 text-slate-400"
    );
  }

  function getStatusStyle(status) {
    const styles = {
      Assigned: "bg-blue-500/10 text-blue-400",
      InProgress: "bg-amber-500/10 text-amber-400",
      Resolved: "bg-violet-500/10 text-violet-400",
      Closed: "bg-green-500/10 text-green-400",
    };

    return (
      styles[status] ||
      "bg-slate-500/10 text-slate-400"
    );
  }

  if (loading) {
    return (
      <div className="flex min-h-screen items-center justify-center bg-slate-950 text-white">
        Loading Developer dashboard...
      </div>
    );
  }

  return (
    <main className="min-h-screen bg-slate-950 p-6 text-white">
      <div className="mx-auto max-w-7xl">
        <header className="mb-8 flex flex-col justify-between gap-4 sm:flex-row sm:items-center">
          <div>
            <h1 className="text-3xl font-bold">
              Developer Dashboard
            </h1>

            <p className="mt-2 text-slate-400">
              Manage your assigned bugs and development work.
            </p>
          </div>

          <div className="flex gap-3">
            <button
              onClick={loadDashboard}
              className="flex items-center gap-2 rounded-lg border border-slate-700 px-4 py-2 text-slate-300 hover:bg-slate-800"
            >
              <RefreshCcw size={17} />
              Refresh
            </button>

            <button
              onClick={handleLogout}
              className="flex items-center gap-2 rounded-lg bg-red-600 px-4 py-2 font-medium hover:bg-red-500"
            >
              <LogOut size={17} />
              Logout
            </button>
          </div>
        </header>

        {error && (
          <div className="mb-5 flex items-center gap-3 rounded-lg bg-red-500/10 p-4 text-red-400">
            <AlertCircle size={20} />
            {error}
          </div>
        )}

        {success && (
          <div className="mb-5 rounded-lg bg-green-500/10 p-4 text-green-400">
            {success}
          </div>
        )}

        {dashboard?.currentProject ? (
          <section className="mb-7 rounded-2xl border border-slate-800 bg-slate-900 p-5">
            <div className="flex items-start gap-4">
              <div className="rounded-xl bg-blue-500/10 p-3 text-blue-400">
                <FolderKanban size={25} />
              </div>

              <div>
                <p className="text-sm text-slate-400">
                  Current Project
                </p>

                <h2 className="mt-1 text-xl font-semibold">
                  {dashboard.currentProject.projectName}
                </h2>

                <div className="mt-2 flex flex-wrap gap-4 text-sm text-slate-400">
                  <span>
                    Code:{" "}
                    {dashboard.currentProject.projectCode}
                  </span>

                  <span>
                    Manager:{" "}
                    {
                      dashboard.currentProject
                        .projectManagerName
                    }
                  </span>

                  <span>
                    Status:{" "}
                    {dashboard.currentProject.status}
                  </span>
                </div>
              </div>
            </div>
          </section>
        ) : (
          <section className="mb-7 rounded-2xl border border-yellow-500/20 bg-yellow-500/10 p-5 text-yellow-300">
            You are not assigned to an active project.
          </section>
        )}

        <section className="mb-7 grid gap-4 sm:grid-cols-2 lg:grid-cols-4">
          <DashboardCard
            title="Assigned Bugs"
            value={dashboard?.assignedBugs || 0}
          />

          <DashboardCard
            title="In Progress"
            value={dashboard?.inProgressBugs || 0}
          />

          <DashboardCard
            title="Resolved"
            value={dashboard?.resolvedBugs || 0}
          />

          <DashboardCard
            title="Critical Active"
            value={dashboard?.criticalActiveBugs || 0}
          />
        </section>

        <section className="mb-7 rounded-2xl border border-slate-800 bg-slate-900 p-5">
          <h2 className="mb-5 text-xl font-semibold">
            Active Bugs by Priority
          </h2>

          <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-4">
            <PriorityCard
              label="Low"
              value={
                dashboard?.activeBugsByPriority?.low || 0
              }
            />

            <PriorityCard
              label="Medium"
              value={
                dashboard?.activeBugsByPriority?.medium || 0
              }
            />

            <PriorityCard
              label="High"
              value={
                dashboard?.activeBugsByPriority?.high || 0
              }
            />

            <PriorityCard
              label="Critical"
              value={
                dashboard?.activeBugsByPriority?.critical || 0
              }
            />
          </div>
        </section>

        <section className="mb-7 overflow-hidden rounded-2xl border border-slate-800 bg-slate-900">
          <div className="flex items-center gap-3 border-b border-slate-800 p-5">
            <Bug className="text-red-400" />

            <h2 className="text-xl font-semibold">
              Active Bugs
            </h2>
          </div>

          <div className="overflow-x-auto">
            <table className="w-full min-w-[1000px] text-left">
              <thead className="bg-slate-800/50 text-sm text-slate-400">
                <tr>
                  <th className="p-4">Bug</th>
                  <th className="p-4">Reporter</th>
                  <th className="p-4">Priority</th>
                  <th className="p-4">Status</th>
                  <th className="p-4">Created</th>
                  <th className="p-4">Action</th>
                </tr>
              </thead>

              <tbody>
                {dashboard?.activeBugs?.map((bug) => (
                  <tr
                    key={bug.bugId}
                    className="border-t border-slate-800"
                  >
                    <td className="p-4">
                      <p className="font-medium">
                        {bug.title}
                      </p>

                      <p className="mt-1 text-sm text-slate-500">
                        {bug.projectName}
                      </p>
                    </td>

                    <td className="p-4 text-slate-300">
                      {bug.reporterName}
                    </td>

                    <td className="p-4">
                      <span
                        className={`rounded-full px-3 py-1 text-sm ${getPriorityStyle(
                          bug.priority
                        )}`}
                      >
                        {bug.priority}
                      </span>
                    </td>

                    <td className="p-4">
                      <span
                        className={`rounded-full px-3 py-1 text-sm ${getStatusStyle(
                          bug.status
                        )}`}
                      >
                        {bug.status}
                      </span>
                    </td>

                    <td className="p-4 text-slate-400">
                      {new Date(
                        bug.createdAt
                      ).toLocaleDateString()}
                    </td>

                    <td className="p-4">
                      {bug.status === "Assigned" && (
                        <button
                          onClick={() =>
                            changeBugStatus(
                              bug.bugId,
                              3
                            )
                          }
                          disabled={
                            updatingBugId === bug.bugId
                          }
                          className="flex items-center gap-2 rounded-lg bg-amber-600 px-4 py-2 text-sm font-medium hover:bg-amber-500 disabled:opacity-50"
                        >
                          <Play size={16} />

                          {updatingBugId === bug.bugId
                            ? "Updating..."
                            : "Start Work"}
                        </button>
                      )}

                      {bug.status === "InProgress" && (
                        <button
                          onClick={() =>
                            changeBugStatus(
                              bug.bugId,
                              4
                            )
                          }
                          disabled={
                            updatingBugId === bug.bugId
                          }
                          className="flex items-center gap-2 rounded-lg bg-green-600 px-4 py-2 text-sm font-medium hover:bg-green-500 disabled:opacity-50"
                        >
                          <CheckCircle2 size={16} />

                          {updatingBugId === bug.bugId
                            ? "Updating..."
                            : "Mark Resolved"}
                        </button>
                      )}
                    </td>
                  </tr>
                ))}

                {dashboard?.activeBugs?.length === 0 && (
                  <tr>
                    <td
                      colSpan="6"
                      className="p-8 text-center text-slate-400"
                    >
                      No active bugs assigned to you.
                    </td>
                  </tr>
                )}
              </tbody>
            </table>
          </div>
        </section>

        <section className="overflow-hidden rounded-2xl border border-slate-800 bg-slate-900">
          <div className="flex items-center gap-3 border-b border-slate-800 p-5">
            <CheckCircle2 className="text-green-400" />

            <h2 className="text-xl font-semibold">
              Recently Resolved Bugs
            </h2>
          </div>

          <div className="overflow-x-auto">
            <table className="w-full min-w-[700px] text-left">
              <thead className="bg-slate-800/50 text-sm text-slate-400">
                <tr>
                  <th className="p-4">Bug</th>
                  <th className="p-4">Priority</th>
                  <th className="p-4">Status</th>
                  <th className="p-4">Updated</th>
                </tr>
              </thead>

              <tbody>
                {dashboard?.recentlyResolvedBugs?.map(
                  (bug) => (
                    <tr
                      key={bug.bugId}
                      className="border-t border-slate-800"
                    >
                      <td className="p-4 font-medium">
                        {bug.title}
                      </td>

                      <td className="p-4">
                        {bug.priority}
                      </td>

                      <td className="p-4 text-green-400">
                        {bug.status}
                      </td>

                      <td className="p-4 text-slate-400">
                        {new Date(
                          bug.updatedAt || bug.createdAt
                        ).toLocaleDateString()}
                      </td>
                    </tr>
                  )
                )}

                {dashboard?.recentlyResolvedBugs
                  ?.length === 0 && (
                  <tr>
                    <td
                      colSpan="4"
                      className="p-8 text-center text-slate-400"
                    >
                      No bugs resolved recently.
                    </td>
                  </tr>
                )}
              </tbody>
            </table>
          </div>
        </section>
      </div>
    </main>
  );
}

function DashboardCard({ title, value }) {
  return (
    <div className="rounded-2xl border border-slate-800 bg-slate-900 p-5">
      <p className="text-sm text-slate-400">{title}</p>
      <p className="mt-2 text-3xl font-bold">{value}</p>
    </div>
  );
}

function PriorityCard({ label, value }) {
  return (
    <div className="rounded-xl bg-slate-950 p-4">
      <p className="text-sm text-slate-400">{label}</p>
      <p className="mt-2 text-2xl font-bold">{value}</p>
    </div>
  );
}

export default DeveloperDashboard;