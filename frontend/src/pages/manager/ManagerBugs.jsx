import { useEffect, useMemo, useState } from "react";
import {
  ArrowLeft,
  Bug,
  Search,
  UserCheck,
} from "lucide-react";
import { useNavigate, useParams } from "react-router";
import api from "../../api/axios";

const priorities = {
  Low: 1,
  Medium: 2,
  High: 3,
  Critical: 4,
};

function ManagerBugs() {
  const navigate = useNavigate();
  const { projectId } = useParams();

  const [project, setProject] = useState(null);
  const [bugs, setBugs] = useState([]);
  const [developers, setDevelopers] = useState([]);

  const [search, setSearch] = useState("");
  const [statusFilter, setStatusFilter] = useState("");
  const [priorityFilter, setPriorityFilter] = useState("");

  const [selectedDevelopers, setSelectedDevelopers] =
    useState({});

  const [loading, setLoading] = useState(true);
  const [updatingBugId, setUpdatingBugId] = useState(null);
  const [error, setError] = useState("");
  const [success, setSuccess] = useState("");

  useEffect(() => {
    loadData();
  }, [projectId]);

  async function loadData() {
    try {
      setLoading(true);
      setError("");

      const [
        projectResponse,
        bugsResponse,
        membersResponse,
      ] = await Promise.all([
        api.get(`/api/Projects/${projectId}`),
        api.get(`/api/projects/${projectId}/bugs`),
        api.get(`/api/projects/${projectId}/members`),
      ]);

      setProject(projectResponse.data);
      setBugs(bugsResponse.data);

      const projectDevelopers =
        membersResponse.data.filter(
          (member) => member.role === "Developer"
        );

      setDevelopers(projectDevelopers);

      const initialSelections = {};

      bugsResponse.data.forEach((bug) => {
        initialSelections[bug.bugId] =
          bug.assignedDeveloperId
            ? String(bug.assignedDeveloperId)
            : "";
      });

      setSelectedDevelopers(initialSelections);
    } catch (requestError) {
      setError(
        requestError.response?.data?.detail ||
          requestError.response?.data?.message ||
          "Failed to load bugs."
      );
    } finally {
      setLoading(false);
    }
  }

  async function handlePriorityChange(
    bugId,
    priorityName
  ) {
    try {
      setUpdatingBugId(bugId);
      setError("");
      setSuccess("");

      const response = await api.patch(
        `/api/bugs/${bugId}/priority`,
        {
          priority: priorities[priorityName],
        }
      );

      updateBug(response.data);
      setSuccess("Bug priority updated.");
    } catch (requestError) {
      setError(
        requestError.response?.data?.detail ||
          requestError.response?.data?.message ||
          "Failed to change priority."
      );
    } finally {
      setUpdatingBugId(null);
    }
  }

  async function handleAssignDeveloper(bugId) {
    const developerId =
      selectedDevelopers[bugId];

    if (!developerId) {
      setError("Select a Developer first.");
      return;
    }

    try {
      setUpdatingBugId(bugId);
      setError("");
      setSuccess("");

      const response = await api.patch(
        `/api/bugs/${bugId}/assign`,
        {
          developerId: Number(developerId),
        }
      );

      updateBug(response.data);
      setSuccess("Developer assigned successfully.");
    } catch (requestError) {
      setError(
        requestError.response?.data?.detail ||
          requestError.response?.data?.message ||
          "Failed to assign Developer."
      );
    } finally {
      setUpdatingBugId(null);
    }
  }

  function updateBug(updatedBug) {
    setBugs((previousBugs) =>
      previousBugs.map((bug) =>
        bug.bugId === updatedBug.bugId
          ? updatedBug
          : bug
      )
    );
  }

  const filteredBugs = useMemo(() => {
    const normalizedSearch =
      search.trim().toLowerCase();

    return bugs.filter((bug) => {
      const matchesSearch =
        bug.title
          .toLowerCase()
          .includes(normalizedSearch) ||
        bug.reporterName
          .toLowerCase()
          .includes(normalizedSearch);

      const matchesStatus =
        statusFilter === "" ||
        bug.status === statusFilter;

      const matchesPriority =
        priorityFilter === "" ||
        bug.priority === priorityFilter;

      return (
        matchesSearch &&
        matchesStatus &&
        matchesPriority
      );
    });
  }, [
    bugs,
    search,
    statusFilter,
    priorityFilter,
  ]);

  if (loading) {
    return (
      <div className="flex min-h-screen items-center justify-center bg-slate-950 text-white">
        Loading bugs...
      </div>
    );
  }

  return (
    <main className="min-h-screen bg-slate-950 p-6 text-white">
      <div className="mx-auto max-w-375">
        <button
          onClick={() =>
            navigate("/manager/dashboard")
          }
          className="mb-5 flex items-center gap-2 text-slate-400 hover:text-white"
        >
          <ArrowLeft size={18} />
          Back to dashboard
        </button>

        <div className="mb-8">
          <h1 className="flex items-center gap-3 text-3xl font-bold">
            <Bug className="text-red-500" />
            Manage Bugs
          </h1>

          <p className="mt-2 text-slate-400">
            {project?.projectName} ·{" "}
            {project?.projectCode}
          </p>
        </div>

        <section className="mb-6 grid gap-4 rounded-2xl border border-slate-800 bg-slate-900 p-5 md:grid-cols-3">
          <div className="relative">
            <Search
              size={18}
              className="absolute left-3 top-1/2 -translate-y-1/2 text-slate-500"
            />

            <input
              value={search}
              onChange={(event) =>
                setSearch(event.target.value)
              }
              placeholder="Search bug or reporter"
              className="w-full rounded-lg border border-slate-700 bg-slate-950 py-3 pl-10 pr-4 outline-none focus:border-blue-500"
            />
          </div>

          <select
            value={statusFilter}
            onChange={(event) =>
              setStatusFilter(event.target.value)
            }
            className="rounded-lg border border-slate-700 bg-slate-950 px-4 py-3 outline-none"
          >
            <option value="">All statuses</option>
            <option value="Open">Open</option>
            <option value="Assigned">Assigned</option>
            <option value="InProgress">
              In Progress
            </option>
            <option value="Resolved">Resolved</option>
            <option value="Closed">Closed</option>
            <option value="Reopened">Reopened</option>
          </select>

          <select
            value={priorityFilter}
            onChange={(event) =>
              setPriorityFilter(event.target.value)
            }
            className="rounded-lg border border-slate-700 bg-slate-950 px-4 py-3 outline-none"
          >
            <option value="">All priorities</option>
            <option value="Low">Low</option>
            <option value="Medium">Medium</option>
            <option value="High">High</option>
            <option value="Critical">Critical</option>
          </select>
        </section>

        {error && (
          <p className="mb-5 rounded-lg bg-red-500/10 p-4 text-red-400">
            {error}
          </p>
        )}

        {success && (
          <p className="mb-5 rounded-lg bg-green-500/10 p-4 text-green-400">
            {success}
          </p>
        )}

        <section className="overflow-hidden rounded-2xl border border-slate-800 bg-slate-900">
          <div className="overflow-x-auto">
            <table className="w-full min-w-375 text-left">
              <thead className="bg-slate-800/50 text-sm text-slate-400">
                <tr>
                  <th className="p-4">Bug</th>
                  <th className="p-4">Reporter</th>
                  <th className="p-4">Type</th>
                  <th className="p-4">Status</th>
                  <th className="p-4">Priority</th>
                  <th className="p-4">
                    Assigned Developer
                  </th>
                  <th className="p-4">Assign</th>
                </tr>
              </thead>

              <tbody>
                {filteredBugs.map((bug) => {
                  const canChangePriority =
                    bug.status === "Open" &&
                    bug.assignedDeveloperId === null;

                  const canAssign =
                    bug.status !== "Resolved" &&
                    bug.status !== "Closed";

                  return (
                    <tr
                      key={bug.bugId}
                      className="border-t border-slate-800 align-top"
                    >
                      <td className="p-4">
                        <p className="font-medium">
                          {bug.title}
                        </p>

                        <p className="mt-1 max-w-sm text-sm text-slate-400">
                          {bug.description}
                        </p>
                      </td>

                      <td className="p-4">
                        {bug.reporterName}
                      </td>

                      <td className="p-4">
                        {bug.type}
                      </td>

                      <td className="p-4">
                        <span className="rounded-full bg-blue-500/10 px-3 py-1 text-sm text-blue-400">
                          {bug.status}
                        </span>
                      </td>

                      <td className="p-4">
                        <select
                          value={bug.priority}
                          disabled={
                            !canChangePriority ||
                            updatingBugId === bug.bugId
                          }
                          onChange={(event) =>
                            handlePriorityChange(
                              bug.bugId,
                              event.target.value
                            )
                          }
                          className="rounded-lg border border-slate-700 bg-slate-950 px-3 py-2 disabled:cursor-not-allowed disabled:opacity-50"
                        >
                          <option value="Low">Low</option>
                          <option value="Medium">
                            Medium
                          </option>
                          <option value="High">
                            High
                          </option>
                          <option value="Critical">
                            Critical
                          </option>
                        </select>
                      </td>

                      <td className="p-4">
                        {bug.assignedDeveloperName ||
                          "Unassigned"}
                      </td>

                      <td className="p-4">
                        <div className="flex min-w-64 gap-2">
                          <select
                            value={
                              selectedDevelopers[
                                bug.bugId
                              ] || ""
                            }
                            disabled={!canAssign}
                            onChange={(event) =>
                              setSelectedDevelopers(
                                (previous) => ({
                                  ...previous,
                                  [bug.bugId]:
                                    event.target.value,
                                })
                              )
                            }
                            className="flex-1 rounded-lg border border-slate-700 bg-slate-950 px-3 py-2 disabled:opacity-50"
                          >
                            <option value="">
                              Select Developer
                            </option>

                            {developers.map(
                              (developer) => (
                                <option
                                  key={
                                    developer.userId
                                  }
                                  value={
                                    developer.userId
                                  }
                                >
                                  {
                                    developer.fullName
                                  }
                                </option>
                              )
                            )}
                          </select>

                          <button
                            onClick={() =>
                              handleAssignDeveloper(
                                bug.bugId
                              )
                            }
                            disabled={
                              !canAssign ||
                              updatingBugId === bug.bugId
                            }
                            className="flex items-center gap-2 rounded-lg bg-blue-600 px-3 py-2 text-sm hover:bg-blue-500 disabled:opacity-50"
                          >
                            <UserCheck size={16} />
                            Assign
                          </button>
                        </div>
                      </td>
                    </tr>
                  );
                })}

                {filteredBugs.length === 0 && (
                  <tr>
                    <td
                      colSpan="7"
                      className="p-8 text-center text-slate-400"
                    >
                      No bugs found.
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

export default ManagerBugs;