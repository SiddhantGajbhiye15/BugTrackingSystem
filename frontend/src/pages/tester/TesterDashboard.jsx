import { useEffect, useState } from "react";
import {
  AlertCircle,
  Bug,
  CheckCircle2,
  ClipboardCheck,
  Eye,
  FolderKanban,
  LogOut,
  Pencil,
  Plus,
  RefreshCcw,
  Trash2,
  XCircle,
} from "lucide-react";
import { useNavigate } from "react-router";
import api from "../../api/axios";

function TesterDashboard() {
  const navigate = useNavigate();

  const [dashboard, setDashboard] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const [success, setSuccess] = useState("");
  const [updatingBugId, setUpdatingBugId] = useState(null);
  const [deletingBugId, setDeletingBugId] = useState(null);

  useEffect(() => {
    loadDashboard();
  }, []);

  async function loadDashboard() {
    try {
      setLoading(true);
      setError("");

      const response = await api.get(
        "/api/dashboard/tester"
      );

      setDashboard(response.data);
    } catch (requestError) {
      setError(
        requestError.response?.data?.detail ||
          requestError.response?.data?.message ||
          "Failed to load Tester dashboard."
      );
    } finally {
      setLoading(false);
    }
  }

  async function verifyBug(bugId, status) {
    try {
      setUpdatingBugId(bugId);
      setError("");
      setSuccess("");

      await api.patch(`/api/bugs/${bugId}/status`, {
        status,
      });

      setSuccess(
        status === 5
          ? "Bug closed successfully."
          : "Bug reopened and sent back for fixing."
      );

      await loadDashboard();
    } catch (requestError) {
      setError(
        requestError.response?.data?.detail ||
          requestError.response?.data?.message ||
          "Failed to update bug status."
      );
    } finally {
      setUpdatingBugId(null);
    }
  }

  async function deleteBug(bug) {
    const confirmed = window.confirm(
      `Delete "${bug.title}"? This action cannot be undone.`
    );

    if (!confirmed) {
      return;
    }

    try {
      setDeletingBugId(bug.bugId);
      setError("");
      setSuccess("");

      await api.delete(`/api/bugs/${bug.bugId}`);

      setSuccess("Bug deleted successfully.");

      await loadDashboard();
    } catch (requestError) {
      setError(
        requestError.response?.data?.detail ||
          requestError.response?.data?.message ||
          "Failed to delete bug."
      );
    } finally {
      setDeletingBugId(null);
    }
  }

  function canModifyBug(bug) {
    return (
      bug.status === "Open" &&
      bug.assignedDeveloperId == null
    );
  }

  function handleLogout() {
    localStorage.removeItem("token");
    localStorage.removeItem("user");
    navigate("/login");
  }

  function formatDate(date) {
    if (!date) {
      return "Not updated";
    }

    return new Date(date).toLocaleDateString();
  }

  function getStatusStyle(status) {
    const styles = {
      Open: "bg-sky-500/10 text-sky-400",
      Assigned: "bg-blue-500/10 text-blue-400",
      InProgress: "bg-amber-500/10 text-amber-400",
      Resolved: "bg-violet-500/10 text-violet-400",
      Closed: "bg-green-500/10 text-green-400",
      Reopened: "bg-red-500/10 text-red-400",
    };

    return (
      styles[status] ||
      "bg-slate-500/10 text-slate-400"
    );
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

  if (loading) {
    return (
      <div className="flex min-h-screen items-center justify-center bg-slate-950 text-white">
        Loading Tester dashboard...
      </div>
    );
  }

  return (
    <main className="min-h-screen bg-slate-950 p-6 text-white">
      <div className="mx-auto max-w-7xl">
        <header className="mb-8 flex flex-col justify-between gap-4 sm:flex-row sm:items-center">
          <div>
            <h1 className="text-3xl font-bold">
              Tester Dashboard
            </h1>

            <p className="mt-2 text-slate-400">
              Report bugs and verify resolved issues.
            </p>
          </div>

          <div className="flex flex-wrap gap-3">
            <button
              onClick={() =>
                navigate(
                  `/tester/projects/${dashboard?.currentProject?.projectId}/bugs/create`
                )
              }
              disabled={!dashboard?.currentProject}
              className="flex items-center gap-2 rounded-lg bg-blue-600 px-4 py-2 font-medium hover:bg-blue-500 disabled:cursor-not-allowed disabled:opacity-50"
            >
              <Plus size={17} />
              Create Bug
            </button>

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
          <div className="mb-6 flex items-center gap-3 rounded-lg bg-red-500/10 p-4 text-red-400">
            <AlertCircle size={20} />
            {error}
          </div>
        )}

        {success && (
          <div className="mb-6 rounded-lg bg-green-500/10 p-4 text-green-400">
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
            You are not currently assigned to a project.
          </section>
        )}

        <section className="mb-7 grid gap-4 sm:grid-cols-2 lg:grid-cols-4">
          <DashboardCard
            title="Reported Bugs"
            value={dashboard?.totalReportedBugs || 0}
            icon={<Bug size={23} />}
          />

          <DashboardCard
            title="Open Bugs"
            value={dashboard?.openBugs || 0}
            icon={<AlertCircle size={23} />}
          />

          <DashboardCard
            title="Awaiting Verification"
            value={dashboard?.awaitingVerification || 0}
            icon={<ClipboardCheck size={23} />}
          />

          <DashboardCard
            title="Reopened Bugs"
            value={dashboard?.reopenedBugs || 0}
            icon={<RefreshCcw size={23} />}
          />
        </section>

        <section className="mb-7 rounded-2xl border border-slate-800 bg-slate-900 p-5">
          <h2 className="mb-5 text-xl font-semibold">
            Bugs by Status
          </h2>

          <div className="grid gap-4 sm:grid-cols-2 md:grid-cols-3 lg:grid-cols-6">
            <StatusCard
              label="Open"
              value={dashboard?.bugsByStatus?.open || 0}
            />

            <StatusCard
              label="Assigned"
              value={
                dashboard?.bugsByStatus?.assigned || 0
              }
            />

            <StatusCard
              label="In Progress"
              value={
                dashboard?.bugsByStatus?.inProgress || 0
              }
            />

            <StatusCard
              label="Resolved"
              value={
                dashboard?.bugsByStatus?.resolved || 0
              }
            />

            <StatusCard
              label="Closed"
              value={
                dashboard?.bugsByStatus?.closed || 0
              }
            />

            <StatusCard
              label="Reopened"
              value={
                dashboard?.bugsByStatus?.reopened || 0
              }
            />
          </div>
        </section>

        <section className="mb-7 overflow-hidden rounded-2xl border border-slate-800 bg-slate-900">
          <div className="flex items-center gap-3 border-b border-slate-800 p-5">
            <ClipboardCheck className="text-violet-400" />

            <h2 className="text-xl font-semibold">
              Awaiting Verification
            </h2>
          </div>

          <div className="overflow-x-auto">
            <table className="w-full min-w-212.5 text-left">
              <thead className="bg-slate-800/50 text-sm text-slate-400">
                <tr>
                  <th className="p-4">Bug</th>
                  <th className="p-4">Priority</th>
                  <th className="p-4">Developer</th>
                  <th className="p-4">Status</th>
                  <th className="p-4">Updated</th>
                  <th className="p-4">Action</th>
                </tr>
              </thead>

              <tbody>
                {dashboard?.awaitingVerificationBugs?.map(
                  (bug) => (
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

                      <td className="p-4">
                        <span
                          className={`rounded-full px-3 py-1 text-sm ${getPriorityStyle(
                            bug.priority
                          )}`}
                        >
                          {bug.priority}
                        </span>
                      </td>

                      <td className="p-4 text-slate-300">
                        {bug.assignedDeveloperName ||
                          "Unassigned"}
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
                        {formatDate(
                          bug.updatedAt || bug.createdAt
                        )}
                      </td>

                      <td className="p-4">
                        <div className="flex flex-wrap gap-2">
                          <button
                            onClick={() =>
                              navigate(`/bugs/${bug.bugId}`)
                            }
                            className="flex items-center gap-2 rounded-lg bg-blue-600 px-3 py-2 text-sm font-medium hover:bg-blue-500"
                          >
                            <Eye size={16} />
                            View
                          </button>

                          <button
                            onClick={() =>
                              verifyBug(bug.bugId, 5)
                            }
                            disabled={
                              updatingBugId === bug.bugId
                            }
                            className="flex items-center gap-2 rounded-lg bg-green-600 px-3 py-2 text-sm font-medium hover:bg-green-500 disabled:cursor-not-allowed disabled:opacity-50"
                          >
                            <CheckCircle2 size={16} />
                            {updatingBugId === bug.bugId
                              ? "Updating..."
                              : "Close"}
                          </button>

                          <button
                            onClick={() =>
                              verifyBug(bug.bugId, 6)
                            }
                            disabled={
                              updatingBugId === bug.bugId
                            }
                            className="flex items-center gap-2 rounded-lg bg-red-600 px-3 py-2 text-sm font-medium hover:bg-red-500 disabled:cursor-not-allowed disabled:opacity-50"
                          >
                            <XCircle size={16} />
                            Reopen
                          </button>
                        </div>
                      </td>
                    </tr>
                  )
                )}

                {dashboard?.awaitingVerificationBugs
                  ?.length === 0 && (
                  <tr>
                    <td
                      colSpan="6"
                      className="p-8 text-center text-slate-400"
                    >
                      No resolved bugs are awaiting
                      verification.
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
              Recently Reported Bugs
            </h2>
          </div>

          <div className="overflow-x-auto">
            <table className="w-full min-w-225 text-left">
              <thead className="bg-slate-800/50 text-sm text-slate-400">
                <tr>
                  <th className="p-4">Bug</th>
                  <th className="p-4">Project</th>
                  <th className="p-4">Priority</th>
                  <th className="p-4">Status</th>
                  <th className="p-4">Developer</th>
                  <th className="p-4">Created</th>
                  <th className="p-4">Action</th>
                </tr>
              </thead>

              <tbody>
                {dashboard?.recentReportedBugs?.map(
                  (bug) => (
                    <tr
                      key={bug.bugId}
                      className="border-t border-slate-800"
                    >
                      <td className="p-4 font-medium">
                        {bug.title}
                      </td>

                      <td className="p-4 text-slate-400">
                        {bug.projectName}
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
                        {bug.assignedDeveloperName ||
                          "Unassigned"}
                      </td>

                      <td className="p-4 text-slate-400">
                        {formatDate(bug.createdAt)}
                      </td>

                      <td className="p-4">
                        <div className="flex flex-wrap gap-2">
                          <button
                            onClick={() =>
                              navigate(`/bugs/${bug.bugId}`)
                            }
                            className="flex items-center gap-2 rounded-lg bg-blue-600 px-3 py-2 text-sm font-medium hover:bg-blue-500"
                          >
                            <Eye size={16} />
                            View
                          </button>

                          {canModifyBug(bug) && (
                            <>
                              <button
                                onClick={() =>
                                  navigate(
                                    `/tester/bugs/${bug.bugId}/edit`
                                  )
                                }
                                className="flex items-center gap-2 rounded-lg bg-amber-600 px-3 py-2 text-sm font-medium hover:bg-amber-500"
                              >
                                <Pencil size={16} />
                                Edit
                              </button>

                              <button
                                onClick={() => deleteBug(bug)}
                                disabled={
                                  deletingBugId === bug.bugId
                                }
                                className="flex items-center gap-2 rounded-lg bg-red-600 px-3 py-2 text-sm font-medium hover:bg-red-500 disabled:cursor-not-allowed disabled:opacity-50"
                              >
                                <Trash2 size={16} />
                                {deletingBugId === bug.bugId
                                  ? "Deleting..."
                                  : "Delete"}
                              </button>
                            </>
                          )}
                        </div>
                      </td>
                    </tr>
                  )
                )}

                {dashboard?.recentReportedBugs?.length ===
                  0 && (
                  <tr>
                    <td
                      colSpan="7"
                      className="p-8 text-center text-slate-400"
                    >
                      You have not reported any bugs.
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

function DashboardCard({ title, value, icon }) {
  return (
    <div className="rounded-2xl border border-slate-800 bg-slate-900 p-5">
      <div className="mb-4 flex h-11 w-11 items-center justify-center rounded-xl bg-blue-500/10 text-blue-400">
        {icon}
      </div>

      <p className="text-sm text-slate-400">{title}</p>

      <p className="mt-1 text-3xl font-bold">{value}</p>
    </div>
  );
}

function StatusCard({ label, value }) {
  return (
    <div className="rounded-xl bg-slate-950 p-4">
      <p className="text-sm text-slate-400">{label}</p>

      <p className="mt-2 text-2xl font-bold">{value}</p>
    </div>
  );
}

export default TesterDashboard;
