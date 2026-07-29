import { useEffect, useMemo, useState } from "react";
import {
  ArrowLeft,
  FolderKanban,
  Search,
  UserRoundCog,
  X,
} from "lucide-react";
import { useNavigate } from "react-router";
import api from "../../api/axios";

const statusNames = {
  1: "Active",
  2: "Completed",
  3: "Archived",
};

function AllProjects() {
  const navigate = useNavigate();

  const [projects, setProjects] = useState([]);
  const [managers, setManagers] = useState([]);
  const [search, setSearch] = useState("");
  const [statusFilter, setStatusFilter] = useState("");

  const [selectedProject, setSelectedProject] =
    useState(null);
  const [selectedManagerId, setSelectedManagerId] =
    useState("");

  const [loading, setLoading] = useState(true);
  const [submitting, setSubmitting] = useState(false);
  const [error, setError] = useState("");
  const [modalError, setModalError] = useState("");

  useEffect(() => {
    loadData();
  }, []);

  async function loadData() {
    try {
      setLoading(true);
      setError("");

      const [projectsResponse, usersResponse] =
        await Promise.all([
          api.get("/api/Projects"),
          api.get("/api/Users"),
        ]);

      setProjects(projectsResponse.data);

      const activeManagers = usersResponse.data.filter(
        (user) =>
          user.role === 2 &&
          user.isActive
      );

      setManagers(activeManagers);
    } catch (requestError) {
      setError(
        requestError.response?.data?.detail ||
          "Failed to load projects."
      );
    } finally {
      setLoading(false);
    }
  }

  function openManagerModal(project) {
    setSelectedProject(project);
    setSelectedManagerId(
      project.projectManagerId
        ? String(project.projectManagerId)
        : ""
    );
    setModalError("");
  }

  function closeManagerModal() {
    setSelectedProject(null);
    setSelectedManagerId("");
    setModalError("");
  }

  async function handleChangeManager(event) {
    event.preventDefault();

    if (!selectedManagerId) {
      setModalError("Select a Project Manager.");
      return;
    }

    try {
      setSubmitting(true);
      setModalError("");

      const response = await api.patch(
        `/api/Projects/${selectedProject.projectId}/manager`,
        {
          projectManagerId: Number(selectedManagerId),
        }
      );

      setProjects((previousProjects) =>
        previousProjects.map((project) =>
          project.projectId === selectedProject.projectId
            ? response.data
            : project
        )
      );

      closeManagerModal();
    } catch (requestError) {
      setModalError(
        requestError.response?.data?.detail ||
          requestError.response?.data?.message ||
          "Failed to change manager."
      );
    } finally {
      setSubmitting(false);
    }
  }

  const filteredProjects = useMemo(() => {
    const normalizedSearch = search
      .trim()
      .toLowerCase();

    return projects.filter((project) => {
      const matchesSearch =
        project.projectName
          .toLowerCase()
          .includes(normalizedSearch) ||
        project.projectCode
          .toLowerCase()
          .includes(normalizedSearch);

      const matchesStatus =
        statusFilter === "" ||
        project.status === Number(statusFilter);

      return matchesSearch && matchesStatus;
    });
  }, [projects, search, statusFilter]);

  if (loading) {
    return (
      <div className="flex min-h-screen items-center justify-center bg-slate-950 text-white">
        Loading projects...
      </div>
    );
  }

  return (
    <main className="min-h-screen bg-slate-950 p-6 text-white">
      <div className="mx-auto max-w-7xl">
        <button
          onClick={() => navigate("/admin/dashboard")}
          className="mb-4 flex items-center gap-2 text-slate-400 hover:text-white"
        >
          <ArrowLeft size={18} />
          Back to dashboard
        </button>

        <div className="mb-8">
          <h1 className="flex items-center gap-3 text-3xl font-bold">
            <FolderKanban className="text-violet-500" />
            All Projects
          </h1>

          <p className="mt-2 text-slate-400">
            {filteredProjects.length} projects found
          </p>
        </div>

        <section className="mb-6 grid gap-4 rounded-2xl border border-slate-800 bg-slate-900 p-5 md:grid-cols-2">
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
              placeholder="Search project name or code"
              className="w-full rounded-lg border border-slate-700 bg-slate-950 py-3 pl-10 pr-4 outline-none focus:border-blue-500"
            />
          </div>

          <select
            value={statusFilter}
            onChange={(event) =>
              setStatusFilter(event.target.value)
            }
            className="rounded-lg border border-slate-700 bg-slate-950 px-4 py-3 outline-none focus:border-blue-500"
          >
            <option value="">All statuses</option>
            <option value="1">Active</option>
            <option value="2">Completed</option>
            <option value="3">Archived</option>
          </select>
        </section>

        {error && (
          <p className="mb-5 rounded-lg bg-red-500/10 p-4 text-red-400">
            {error}
          </p>
        )}

        <section className="overflow-hidden rounded-2xl border border-slate-800 bg-slate-900">
          <div className="overflow-x-auto">
            <table className="w-full text-left">
              <thead className="bg-slate-800/50 text-sm text-slate-400">
                <tr>
                  <th className="p-4">Project</th>
                  <th className="p-4">Code</th>
                  <th className="p-4">Current Manager</th>
                  <th className="p-4">Created By</th>
                  <th className="p-4">Status</th>
                  <th className="p-4">Action</th>
                </tr>
              </thead>

              <tbody>
                {filteredProjects.map((project) => (
                  <tr
                    key={project.projectId}
                    className="border-t border-slate-800 hover:bg-slate-800/30"
                  >
                    <td className="p-4">
                      <p className="font-medium">
                        {project.projectName}
                      </p>

                      <p className="mt-1 max-w-xs text-sm text-slate-400">
                        {project.description}
                      </p>
                    </td>

                    <td className="p-4 text-slate-400">
                      {project.projectCode}
                    </td>

                    <td className="p-4">
                      {project.projectManagerName ||
                        "Not Assigned"}
                    </td>

                    <td className="p-4 text-slate-400">
                      {project.createdByName}
                    </td>

                    <td className="p-4">
                      <span className="rounded-full bg-blue-500/10 px-3 py-1 text-sm text-blue-400">
                        {statusNames[project.status]}
                      </span>
                    </td>

                    <td className="p-4">
                      <button
                        onClick={() =>
                          openManagerModal(project)
                        }
                        className="flex items-center gap-2 rounded-lg bg-violet-600 px-3 py-2 text-sm font-medium hover:bg-violet-500"
                      >
                        <UserRoundCog size={16} />
                        {project.projectManagerId
                          ? "Change Manager"
                          : "Assign Manager"}
                      </button>
                    </td>
                  </tr>
                ))}

                {filteredProjects.length === 0 && (
                  <tr>
                    <td
                      colSpan="6"
                      className="p-8 text-center text-slate-400"
                    >
                      No projects found.
                    </td>
                  </tr>
                )}
              </tbody>
            </table>
          </div>
        </section>
      </div>

      {selectedProject && (
        <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/70 p-4">
          <div className="w-full max-w-md rounded-2xl border border-slate-700 bg-slate-900 p-6">
            <div className="mb-6 flex items-start justify-between">
              <div>
                <h2 className="text-2xl font-bold">
                  Change Manager
                </h2>

                <p className="mt-1 text-sm text-slate-400">
                  {selectedProject.projectName}
                </p>
              </div>

              <button
                onClick={closeManagerModal}
                className="rounded-lg p-2 text-slate-400 hover:bg-slate-800 hover:text-white"
              >
                <X size={22} />
              </button>
            </div>

            <form
              onSubmit={handleChangeManager}
              className="space-y-5"
            >
              <div>
                <p className="mb-2 text-sm text-slate-400">
                  Current Manager
                </p>

                <p className="font-medium">
                  {selectedProject.projectManagerName ||
                    "Not Assigned"}
                </p>
              </div>

              <div>
                <label className="mb-2 block text-sm text-slate-300">
                  New Project Manager
                </label>

                <select
                  value={selectedManagerId}
                  onChange={(event) =>
                    setSelectedManagerId(
                      event.target.value
                    )
                  }
                  required
                  className="w-full rounded-lg border border-slate-700 bg-slate-950 px-4 py-3 outline-none focus:border-violet-500"
                >
                  <option value="">
                    Select manager
                  </option>

                  {managers.map((manager) => (
                    <option
                      key={manager.userId}
                      value={manager.userId}
                    >
                      {manager.firstName}{" "}
                      {manager.lastName}
                    </option>
                  ))}
                </select>
              </div>

              {modalError && (
                <p className="rounded-lg bg-red-500/10 p-3 text-sm text-red-400">
                  {modalError}
                </p>
              )}

              <div className="flex justify-end gap-3">
                <button
                  type="button"
                  onClick={closeManagerModal}
                  className="rounded-lg border border-slate-700 px-5 py-3 hover:bg-slate-800"
                >
                  Cancel
                </button>

                <button
                  type="submit"
                  disabled={submitting}
                  className="rounded-lg bg-violet-600 px-5 py-3 font-semibold hover:bg-violet-500 disabled:opacity-60"
                >
                  {submitting
                    ? "Saving..."
                    : "Save Manager"}
                </button>
              </div>
            </form>
          </div>
        </div>
      )}
    </main>
  );
}

export default AllProjects;