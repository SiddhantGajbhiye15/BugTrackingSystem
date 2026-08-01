import { useEffect, useMemo, useState } from "react";
import {
  ArrowLeft,
  Bug,
  FolderKanban,
  Pencil,
  Plus,
  Search,
  Trash2,
  Users,
  X,
} from "lucide-react";
import { useNavigate } from "react-router";
import api from "../../api/axios";

const emptyCreateForm = {
  projectCode: "",
  projectName: "",
  description: "",
};

function ManagerProjects() {
  const navigate = useNavigate();

  const [projects, setProjects] = useState([]);
  const [search, setSearch] = useState("");
  const [createForm, setCreateForm] = useState(emptyCreateForm);
  const [editingProject, setEditingProject] = useState(null);

  const [showCreateModal, setShowCreateModal] = useState(false);
  const [loading, setLoading] = useState(true);
  const [submitting, setSubmitting] = useState(false);
  const [deletingProjectId, setDeletingProjectId] = useState(null);
  const [error, setError] = useState("");
  const [createError, setCreateError] = useState("");
  const [success, setSuccess] = useState("");

  const storedUser = localStorage.getItem("user");
  const currentUser = storedUser ? JSON.parse(storedUser) : null;

  useEffect(() => {
    loadProjects();
  }, []);

  async function loadProjects() {
    try {
      setLoading(true);
      setError("");

      const response = await api.get("/api/Projects");

      const managedProjects = response.data.filter(
        (project) =>
          Number(project.projectManagerId) ===
          Number(currentUser?.userId)
      );

      setProjects(managedProjects);
    } catch (requestError) {
      setError(getErrorMessage(requestError, "Failed to load projects."));
    } finally {
      setLoading(false);
    }
  }

  function handleCreateInputChange(event) {
    const { name, value } = event.target;

    setCreateForm((previousForm) => ({
      ...previousForm,
      [name]: value,
    }));

    if (createError) {
      setCreateError("");
    }
  }

  function closeCreateModal() {
    if (submitting) {
      return;
    }

    setShowCreateModal(false);
    setCreateForm(emptyCreateForm);
    setCreateError("");
  }

  function handleEditInputChange(event) {
    const { name, value } = event.target;

    setEditingProject((previousProject) => ({
      ...previousProject,
      [name]: value,
    }));
  }

  async function handleCreateProject(event) {
    event.preventDefault();

    try {
      setSubmitting(true);
      setCreateError("");
      setError("");
      setSuccess("");

      const response = await api.post("/api/Projects", {
        projectCode: createForm.projectCode.trim(),
        projectName: createForm.projectName.trim(),
        description: createForm.description.trim(),
      });

      setProjects((previousProjects) => [
        response.data,
        ...previousProjects,
      ]);

      setCreateForm(emptyCreateForm);
      setCreateError("");
      setShowCreateModal(false);
      setSuccess("Project created successfully.");
    } catch (requestError) {
      setCreateError(
        getErrorMessage(requestError, "Failed to create project.")
      );
    } finally {
      setSubmitting(false);
    }
  }

  async function handleUpdateProject(event) {
    event.preventDefault();

    try {
      setSubmitting(true);
      setError("");
      setSuccess("");

      const response = await api.put(
        `/api/Projects/${editingProject.projectId}`,
        {
          projectName: editingProject.projectName.trim(),
          description: editingProject.description.trim(),
          status: Number(editingProject.status),
        }
      );

      setProjects((previousProjects) =>
        previousProjects.map((project) =>
          project.projectId === response.data.projectId
            ? response.data
            : project
        )
      );

      setEditingProject(null);
      setSuccess("Project updated successfully.");
    } catch (requestError) {
      setError(getErrorMessage(requestError, "Failed to update project."));
    } finally {
      setSubmitting(false);
    }
  }

  async function handleDeleteProject(project) {
    const confirmed = window.confirm(
      `Delete "${project.projectName}"? This action cannot be undone.`
    );

    if (!confirmed) {
      return;
    }

    try {
      setDeletingProjectId(project.projectId);
      setError("");
      setSuccess("");

      await api.delete(`/api/Projects/${project.projectId}`);

      setProjects((previousProjects) =>
        previousProjects.filter(
          (existingProject) =>
            existingProject.projectId !== project.projectId
        )
      );

      setSuccess("Project deleted successfully.");
    } catch (requestError) {
      setError(getErrorMessage(requestError, "Failed to delete project."));
    } finally {
      setDeletingProjectId(null);
    }
  }

  const filteredProjects = useMemo(() => {
    const value = search.trim().toLowerCase();

    if (!value) {
      return projects;
    }

    return projects.filter((project) => {
      return (
        project.projectName.toLowerCase().includes(value) ||
        project.projectCode.toLowerCase().includes(value) ||
        project.description?.toLowerCase().includes(value)
      );
    });
  }, [projects, search]);

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
          onClick={() => navigate("/manager/dashboard")}
          className="mb-6 flex items-center gap-2 text-slate-400 hover:text-white"
        >
          <ArrowLeft size={18} />
          Back to dashboard
        </button>

        <header className="mb-8 flex flex-col justify-between gap-4 sm:flex-row sm:items-center">
          <div>
            <h1 className="flex items-center gap-3 text-3xl font-bold">
              <FolderKanban className="text-blue-500" />
              My Projects
            </h1>

            <p className="mt-2 text-slate-400">
              Create and manage the projects assigned to you.
            </p>
          </div>

          <button
            onClick={() => {
              setError("");
              setCreateError("");
              setSuccess("");
              setCreateForm(emptyCreateForm);
              setShowCreateModal(true);
            }}
            className="flex items-center justify-center gap-2 rounded-lg bg-blue-600 px-5 py-3 font-semibold hover:bg-blue-500"
          >
            <Plus size={18} />
            Create Project
          </button>
        </header>

        {error && (
          <div className="mb-5 rounded-lg bg-red-500/10 p-4 text-red-400">
            {error}
          </div>
        )}

        {success && (
          <div className="mb-5 rounded-lg bg-green-500/10 p-4 text-green-400">
            {success}
          </div>
        )}

        <section className="mb-6 rounded-2xl border border-slate-800 bg-slate-900 p-5">
          <div className="relative max-w-xl">
            <Search
              size={18}
              className="absolute left-3 top-1/2 -translate-y-1/2 text-slate-500"
            />

            <input
              value={search}
              onChange={(event) => setSearch(event.target.value)}
              placeholder="Search by project name, code, or description"
              className="w-full rounded-lg border border-slate-700 bg-slate-950 py-3 pl-10 pr-4 outline-none focus:border-blue-500"
            />
          </div>
        </section>

        <section className="overflow-hidden rounded-2xl border border-slate-800 bg-slate-900">
          <div className="border-b border-slate-800 p-5">
            <h2 className="text-xl font-semibold">
              Projects ({filteredProjects.length})
            </h2>
          </div>

          <div className="overflow-x-auto">
            <table className="w-full min-w-275 text-left">
              <thead className="bg-slate-800/50 text-sm text-slate-400">
                <tr>
                  <th className="p-4">Project</th>
                  <th className="p-4">Code</th>
                  <th className="p-4">Description</th>
                  <th className="p-4">Status</th>
                  <th className="p-4">Created</th>
                  <th className="p-4">Actions</th>
                </tr>
              </thead>

              <tbody>
                {filteredProjects.map((project) => (
                  <tr
                    key={project.projectId}
                    className="border-t border-slate-800 align-top"
                  >
                    <td className="p-4 font-medium">
                      {project.projectName}
                    </td>

                    <td className="p-4 text-slate-400">
                      {project.projectCode}
                    </td>

                    <td className="max-w-sm p-4 text-slate-400">
                      {project.description || "No description"}
                    </td>

                    <td className="p-4">
                      <span className={getStatusStyle(project.status)}>
                        {getStatusLabel(project.status)}
                      </span>
                    </td>

                    <td className="p-4 text-slate-400">
                      {new Date(project.createdAt).toLocaleDateString()}
                    </td>

                    <td className="p-4">
                      <div className="flex flex-wrap gap-2">
                        <button
                          onClick={() =>
                            navigate(
                              `/manager/projects/${project.projectId}/members`
                            )
                          }
                          className="flex items-center gap-2 rounded-lg bg-blue-500/10 px-3 py-2 text-sm text-blue-400 hover:bg-blue-500/20"
                        >
                          <Users size={16} />
                          Members
                        </button>

                        <button
                          onClick={() =>
                            navigate(
                              `/manager/projects/${project.projectId}/bugs`
                            )
                          }
                          className="flex items-center gap-2 rounded-lg bg-red-500/10 px-3 py-2 text-sm text-red-400 hover:bg-red-500/20"
                        >
                          <Bug size={16} />
                          Bugs
                        </button>

                        <button
                          onClick={() => {
                            setError("");
                            setSuccess("");
                            setEditingProject({
                              ...project,
                              status: String(project.status),
                            });
                          }}
                          className="flex items-center gap-2 rounded-lg bg-amber-500/10 px-3 py-2 text-sm text-amber-400 hover:bg-amber-500/20"
                        >
                          <Pencil size={16} />
                          Edit
                        </button>

                        <button
                          onClick={() => handleDeleteProject(project)}
                          disabled={
                            deletingProjectId === project.projectId
                          }
                          className="flex items-center gap-2 rounded-lg bg-red-600 px-3 py-2 text-sm font-medium hover:bg-red-500 disabled:opacity-50"
                        >
                          <Trash2 size={16} />
                          {deletingProjectId === project.projectId
                            ? "Deleting..."
                            : "Delete"}
                        </button>
                      </div>
                    </td>
                  </tr>
                ))}

                {filteredProjects.length === 0 && (
                  <tr>
                    <td
                      colSpan="6"
                      className="p-10 text-center text-slate-400"
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

      {showCreateModal && (
        <Modal
          title="Create Project"
          onClose={closeCreateModal}
        >
          <form onSubmit={handleCreateProject} className="space-y-5">
            {createError && (
              <div className="rounded-lg border border-red-500/30 bg-red-500/10 p-4 text-sm text-red-400">
                {createError}
              </div>
            )}
            <FormField label="Project Code">
              <input
                name="projectCode"
                value={createForm.projectCode}
                onChange={handleCreateInputChange}
                required
                placeholder="Example: BTS-001"
                className="form-input"
              />
            </FormField>

            <FormField label="Project Name">
              <input
                name="projectName"
                value={createForm.projectName}
                onChange={handleCreateInputChange}
                required
                placeholder="Example: Bug Tracking System"
                className="form-input"
              />
            </FormField>

            <FormField label="Description">
              <textarea
                name="description"
                value={createForm.description}
                onChange={handleCreateInputChange}
                required
                rows="4"
                placeholder="Describe the project."
                className="form-input resize-none"
              />
            </FormField>

            <ModalActions
              submitting={submitting}
              submitLabel="Create Project"
              onCancel={closeCreateModal}
            />
          </form>
        </Modal>
      )}

      {editingProject && (
        <Modal
          title="Edit Project"
          onClose={() => setEditingProject(null)}
        >
          <form onSubmit={handleUpdateProject} className="space-y-5">
            <FormField label="Project Code">
              <input
                value={editingProject.projectCode}
                disabled
                className="form-input cursor-not-allowed opacity-60"
              />
            </FormField>

            <FormField label="Project Name">
              <input
                name="projectName"
                value={editingProject.projectName}
                onChange={handleEditInputChange}
                required
                className="form-input"
              />
            </FormField>

            <FormField label="Description">
              <textarea
                name="description"
                value={editingProject.description || ""}
                onChange={handleEditInputChange}
                required
                rows="4"
                className="form-input resize-none"
              />
            </FormField>

            <FormField label="Status">
              <select
                name="status"
                value={editingProject.status}
                onChange={handleEditInputChange}
                className="form-input"
              >
                <option value="1">Active</option>
                <option value="2">Completed</option>
                <option value="3">Archived</option>
              </select>
            </FormField>

            <ModalActions
              submitting={submitting}
              submitLabel="Save Changes"
              onCancel={() => setEditingProject(null)}
            />
          </form>
        </Modal>
      )}
    </main>
  );
}

function Modal({ title, onClose, children }) {
  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/70 p-4">
      <div className="w-full max-w-xl rounded-2xl border border-slate-700 bg-slate-900 p-6 text-white shadow-2xl">
        <div className="mb-6 flex items-center justify-between">
          <h2 className="text-2xl font-bold">{title}</h2>

          <button
            type="button"
            onClick={onClose}
            className="rounded-lg p-2 text-slate-400 hover:bg-slate-800 hover:text-white"
          >
            <X size={20} />
          </button>
        </div>

        {children}
      </div>
    </div>
  );
}

function FormField({ label, children }) {
  return (
    <label className="block">
      <span className="mb-2 block text-sm font-medium text-slate-300">
        {label}
      </span>
      {children}
    </label>
  );
}

function ModalActions({
  submitting,
  submitLabel,
  onCancel,
}) {
  return (
    <div className="flex justify-end gap-3 border-t border-slate-800 pt-5">
      <button
        type="button"
        onClick={onCancel}
        className="rounded-lg border border-slate-700 px-4 py-2 text-slate-300 hover:bg-slate-800"
      >
        Cancel
      </button>

      <button
        type="submit"
        disabled={submitting}
        className="rounded-lg bg-blue-600 px-5 py-2 font-semibold hover:bg-blue-500 disabled:opacity-50"
      >
        {submitting ? "Saving..." : submitLabel}
      </button>
    </div>
  );
}

function getStatusLabel(status) {
  const labels = {
    1: "Active",
    2: "Completed",
    3: "Archived",
  };

  return labels[Number(status)] || `Status ${status}`;
}

function getStatusStyle(status) {
  const styles = {
    1: "rounded-full bg-green-500/10 px-3 py-1 text-sm text-green-400",
    2: "rounded-full bg-blue-500/10 px-3 py-1 text-sm text-blue-400",
    3: "rounded-full bg-slate-500/10 px-3 py-1 text-sm text-slate-400",
  };

  return (
    styles[Number(status)] ||
    "rounded-full bg-slate-500/10 px-3 py-1 text-sm text-slate-400"
  );
}

function getErrorMessage(error, fallbackMessage) {
  return (
    error.response?.data?.detail ||
    error.response?.data?.message ||
    fallbackMessage
  );
}

export default ManagerProjects;