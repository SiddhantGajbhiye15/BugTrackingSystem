import { useEffect, useMemo, useState } from "react";
import {
  ArrowLeft,
  Edit,
  KeyRound,
  Plus,
  Power,
  Search,
  Users,
  X,
} from "lucide-react";
import { useNavigate } from "react-router";
import api from "../../api/axios";

const roleNames = {
  1: "Admin",
  2: "Project Manager",
  3: "Developer",
  4: "Tester",
};

const emptyAddForm = {
  firstName: "",
  lastName: "",
  email: "",
  password: "",
  role: "3",
};

const emptyEditForm = {
  userId: 0,
  firstName: "",
  lastName: "",
  email: "",
  role: "3",
};

const emptyResetForm = {
  newPassword: "",
  confirmPassword: "",
};

function AllUsers() {
  const navigate = useNavigate();

  const [users, setUsers] = useState([]);
  const [search, setSearch] = useState("");
  const [roleFilter, setRoleFilter] = useState("");
  const [statusFilter, setStatusFilter] = useState("");

  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const [success, setSuccess] = useState("");

  const [showAddModal, setShowAddModal] = useState(false);
  const [addForm, setAddForm] = useState(emptyAddForm);
  const [addError, setAddError] = useState("");

  const [showEditModal, setShowEditModal] = useState(false);
  const [editForm, setEditForm] = useState(emptyEditForm);
  const [editError, setEditError] = useState("");

  const [showResetModal, setShowResetModal] = useState(false);
  const [resetUser, setResetUser] = useState(null);
  const [resetForm, setResetForm] = useState(emptyResetForm);
  const [resetError, setResetError] = useState("");

  const [submitting, setSubmitting] = useState(false);
  const [changingStatusId, setChangingStatusId] =
    useState(null);

  useEffect(() => {
    loadUsers();
  }, []);

  async function loadUsers() {
    try {
      setLoading(true);
      setError("");

      const response = await api.get("/api/Users");
      setUsers(response.data);
    } catch (requestError) {
      setError(
        requestError.response?.data?.detail ||
          requestError.response?.data?.message ||
          "Failed to load users."
      );
    } finally {
      setLoading(false);
    }
  }

  function handleAddInputChange(event) {
    const { name, value } = event.target;

    setAddForm((previousForm) => ({
      ...previousForm,
      [name]: value,
    }));
  }

  function handleEditInputChange(event) {
    const { name, value } = event.target;

    setEditForm((previousForm) => ({
      ...previousForm,
      [name]: value,
    }));
  }

  function handleResetInputChange(event) {
    const { name, value } = event.target;

    setResetForm((previousForm) => ({
      ...previousForm,
      [name]: value,
    }));
  }

  function closeAddModal() {
    setShowAddModal(false);
    setAddForm(emptyAddForm);
    setAddError("");
  }

  function openEditModal(user) {
    setEditForm({
      userId: user.userId,
      firstName: user.firstName,
      lastName: user.lastName,
      email: user.email,
      role: String(user.role),
    });

    setEditError("");
    setShowEditModal(true);
  }

  function closeEditModal() {
    setShowEditModal(false);
    setEditForm(emptyEditForm);
    setEditError("");
  }

  function openResetModal(user) {
    setResetUser(user);
    setResetForm(emptyResetForm);
    setResetError("");
    setShowResetModal(true);
  }

  function closeResetModal() {
    setShowResetModal(false);
    setResetUser(null);
    setResetForm(emptyResetForm);
    setResetError("");
  }

  async function handleAddUser(event) {
    event.preventDefault();

    try {
      setSubmitting(true);
      setAddError("");

      const response = await api.post("/api/Users", {
        firstName: addForm.firstName.trim(),
        lastName: addForm.lastName.trim(),
        email: addForm.email.trim(),
        password: addForm.password,
        role: Number(addForm.role),
      });

      setUsers((previousUsers) => [
        ...previousUsers,
        response.data,
      ]);

      closeAddModal();
    } catch (requestError) {
      setAddError(
        requestError.response?.data?.detail ||
          requestError.response?.data?.message ||
          "Failed to create user."
      );
    } finally {
      setSubmitting(false);
    }
  }

  async function handleEditUser(event) {
    event.preventDefault();

    try {
      setSubmitting(true);
      setEditError("");

      const response = await api.put(
        `/api/Users/${editForm.userId}`,
        {
          firstName: editForm.firstName.trim(),
          lastName: editForm.lastName.trim(),
          email: editForm.email.trim(),
          role: Number(editForm.role),
        }
      );

      setUsers((previousUsers) =>
        previousUsers.map((user) =>
          user.userId === editForm.userId
            ? response.data
            : user
        )
      );

      closeEditModal();
    } catch (requestError) {
      setEditError(
        requestError.response?.data?.detail ||
          requestError.response?.data?.message ||
          "Failed to update user."
      );
    } finally {
      setSubmitting(false);
    }
  }

  async function handleResetPassword(event) {
    event.preventDefault();

    if (resetForm.newPassword.length < 8) {
      setResetError(
        "Password must be at least 8 characters long."
      );
      return;
    }

    if (
      resetForm.newPassword !==
      resetForm.confirmPassword
    ) {
      setResetError("Passwords do not match.");
      return;
    }

    try {
      setSubmitting(true);
      setResetError("");
      setError("");
      setSuccess("");

      await api.patch(
        `/api/Users/${resetUser.userId}/reset-password`,
        {
          newPassword: resetForm.newPassword,
        }
      );

      const userName =
        `${resetUser.firstName} ${resetUser.lastName}`;

      closeResetModal();

      setSuccess(
        `Password reset successfully for ${userName}.`
      );
    } catch (requestError) {
      setResetError(
        requestError.response?.data?.detail ||
          requestError.response?.data?.message ||
          getValidationError(requestError) ||
          "Failed to reset password."
      );
    } finally {
      setSubmitting(false);
    }
  }

  async function handleStatusChange(user) {
  const action = user.isActive
    ? "deactivate"
    : "activate";

  const actionPastTense =
    action === "activate"
      ? "activated"
      : "deactivated";

  const confirmed = window.confirm(
    `Are you sure you want to ${action} ${user.firstName} ${user.lastName}?`
  );

  if (!confirmed) {
    return;
  }

  try {
    setChangingStatusId(user.userId);
    setError("");
    setSuccess("");

    await api.patch(
      `/api/Users/${user.userId}/${action}`
    );

    // Reload the real value saved in the database.
    await loadUsers();

    setSuccess(
      `${user.firstName} ${user.lastName} was ${actionPastTense} successfully.`
    );
  } catch (requestError) {
    setError(
      requestError.response?.data?.detail ||
        requestError.response?.data?.message ||
        `Failed to ${action} user.`
    );
  } finally {
    setChangingStatusId(null);
  }
}

  const filteredUsers = useMemo(() => {
    const normalizedSearch = search
      .trim()
      .toLowerCase();

    return users.filter((user) => {
      const fullName =
        `${user.firstName} ${user.lastName}`.toLowerCase();

      const matchesSearch =
        fullName.includes(normalizedSearch) ||
        user.email
          .toLowerCase()
          .includes(normalizedSearch);

      const matchesRole =
        roleFilter === "" ||
        user.role === Number(roleFilter);

      const matchesStatus =
        statusFilter === "" ||
        String(user.isActive) === statusFilter;

      return (
        matchesSearch &&
        matchesRole &&
        matchesStatus
      );
    });
  }, [users, search, roleFilter, statusFilter]);

  if (loading) {
    return (
      <div className="flex min-h-screen items-center justify-center bg-slate-950 text-white">
        Loading users...
      </div>
    );
  }

  return (
    <main className="min-h-screen bg-slate-950 p-6 text-white">
      <div className="mx-auto max-w-7xl">
        <div className="mb-8 flex flex-col gap-5 sm:flex-row sm:items-end sm:justify-between">
          <div>
            <button
              onClick={() =>
                navigate("/admin/dashboard")
              }
              className="mb-4 flex items-center gap-2 text-slate-400 hover:text-white"
            >
              <ArrowLeft size={18} />
              Back to dashboard
            </button>

            <h1 className="flex items-center gap-3 text-3xl font-bold">
              <Users className="text-blue-500" />
              All Users
            </h1>

            <p className="mt-2 text-slate-400">
              {filteredUsers.length} users found
            </p>
          </div>

          <button
            onClick={() => setShowAddModal(true)}
            className="flex items-center justify-center gap-2 rounded-lg bg-blue-600 px-5 py-3 font-semibold hover:bg-blue-500"
          >
            <Plus size={19} />
            Add User
          </button>
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
              placeholder="Search name or email"
              className="w-full rounded-lg border border-slate-700 bg-slate-950 py-3 pl-10 pr-4 outline-none focus:border-blue-500"
            />
          </div>

          <select
            value={roleFilter}
            onChange={(event) =>
              setRoleFilter(event.target.value)
            }
            className="rounded-lg border border-slate-700 bg-slate-950 px-4 py-3 outline-none focus:border-blue-500"
          >
            <option value="">All roles</option>
            <option value="1">Admin</option>
            <option value="2">
              Project Manager
            </option>
            <option value="3">Developer</option>
            <option value="4">Tester</option>
          </select>

          <select
            value={statusFilter}
            onChange={(event) =>
              setStatusFilter(event.target.value)
            }
            className="rounded-lg border border-slate-700 bg-slate-950 px-4 py-3 outline-none focus:border-blue-500"
          >
            <option value="">All statuses</option>
            <option value="true">Active</option>
            <option value="false">Inactive</option>
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
            <table className="w-full text-left">
              <thead className="bg-slate-800/50 text-sm text-slate-400">
                <tr>
                  <th className="p-4">Name</th>
                  <th className="p-4">Email</th>
                  <th className="p-4">Role</th>
                  <th className="p-4">Status</th>
                  <th className="p-4">Actions</th>
                </tr>
              </thead>

              <tbody>
                {filteredUsers.map((user) => (
                  <tr
                    key={user.userId}
                    className="border-t border-slate-800 hover:bg-slate-800/30"
                  >
                    <td className="p-4 font-medium">
                      {user.firstName} {user.lastName}
                    </td>

                    <td className="p-4 text-slate-400">
                      {user.email}
                    </td>

                    <td className="p-4">
                      {roleNames[user.role]}
                    </td>

                    <td className="p-4">
                      <span
                        className={
                          user.isActive
                            ? "rounded-full bg-green-500/10 px-3 py-1 text-sm text-green-400"
                            : "rounded-full bg-red-500/10 px-3 py-1 text-sm text-red-400"
                        }
                      >
                        {user.isActive
                          ? "Active"
                          : "Inactive"}
                      </span>
                    </td>

                    <td className="p-4">
                      <div className="flex flex-wrap gap-2">
                        <button
                          onClick={() =>
                            openEditModal(user)
                          }
                          className="flex items-center gap-2 rounded-lg bg-slate-700 px-3 py-2 text-sm hover:bg-slate-600"
                        >
                          <Edit size={16} />
                          Edit
                        </button>

                        <button
                          onClick={() =>
                            openResetModal(user)
                          }
                          className="flex items-center gap-2 rounded-lg bg-amber-500/10 px-3 py-2 text-sm text-amber-400 hover:bg-amber-500/20"
                        >
                          <KeyRound size={16} />
                          Reset Password
                        </button>

                        <button
                          onClick={() =>
                            handleStatusChange(user)
                          }
                          disabled={
                            changingStatusId ===
                            user.userId
                          }
                          className={
                            user.isActive
                              ? "flex items-center gap-2 rounded-lg bg-red-500/10 px-3 py-2 text-sm text-red-400 hover:bg-red-500/20 disabled:opacity-50"
                              : "flex items-center gap-2 rounded-lg bg-green-500/10 px-3 py-2 text-sm text-green-400 hover:bg-green-500/20 disabled:opacity-50"
                          }
                        >
                          <Power size={16} />

                          {changingStatusId ===
                          user.userId
                            ? "Updating..."
                            : user.isActive
                              ? "Deactivate"
                              : "Activate"}
                        </button>
                      </div>
                    </td>
                  </tr>
                ))}

                {filteredUsers.length === 0 && (
                  <tr>
                    <td
                      colSpan="5"
                      className="p-8 text-center text-slate-400"
                    >
                      No users found.
                    </td>
                  </tr>
                )}
              </tbody>
            </table>
          </div>
        </section>
      </div>

      {showAddModal && (
        <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/70 p-4">
          <div className="w-full max-w-lg rounded-2xl border border-slate-700 bg-slate-900 p-6">
            <div className="mb-6 flex items-center justify-between">
              <h2 className="text-2xl font-bold">
                Add New User
              </h2>

              <button
                type="button"
                onClick={closeAddModal}
                className="rounded-lg p-2 text-slate-400 hover:bg-slate-800 hover:text-white"
              >
                <X size={22} />
              </button>
            </div>

            <form
              onSubmit={handleAddUser}
              className="space-y-4"
            >
              <div className="grid gap-4 sm:grid-cols-2">
                <InputField
                  label="First Name"
                  name="firstName"
                  value={addForm.firstName}
                  onChange={handleAddInputChange}
                />

                <InputField
                  label="Last Name"
                  name="lastName"
                  value={addForm.lastName}
                  onChange={handleAddInputChange}
                />
              </div>

              <InputField
                label="Email"
                name="email"
                type="email"
                value={addForm.email}
                onChange={handleAddInputChange}
              />

              <InputField
                label="Password"
                name="password"
                type="password"
                value={addForm.password}
                onChange={handleAddInputChange}
                minLength={8}
              />

              <RoleField
                value={addForm.role}
                onChange={handleAddInputChange}
              />

              {addError && (
                <ErrorMessage message={addError} />
              )}

              <ModalButtons
                onCancel={closeAddModal}
                submitting={submitting}
                submitText="Create User"
                loadingText="Creating..."
              />
            </form>
          </div>
        </div>
      )}

      {showEditModal && (
        <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/70 p-4">
          <div className="w-full max-w-lg rounded-2xl border border-slate-700 bg-slate-900 p-6">
            <div className="mb-6 flex items-center justify-between">
              <h2 className="text-2xl font-bold">
                Edit User
              </h2>

              <button
                type="button"
                onClick={closeEditModal}
                className="rounded-lg p-2 text-slate-400 hover:bg-slate-800 hover:text-white"
              >
                <X size={22} />
              </button>
            </div>

            <form
              onSubmit={handleEditUser}
              className="space-y-4"
            >
              <div className="grid gap-4 sm:grid-cols-2">
                <InputField
                  label="First Name"
                  name="firstName"
                  value={editForm.firstName}
                  onChange={handleEditInputChange}
                />

                <InputField
                  label="Last Name"
                  name="lastName"
                  value={editForm.lastName}
                  onChange={handleEditInputChange}
                />
              </div>

              <InputField
                label="Email"
                name="email"
                type="email"
                value={editForm.email}
                onChange={handleEditInputChange}
              />

              <RoleField
                value={editForm.role}
                onChange={handleEditInputChange}
              />

              {editError && (
                <ErrorMessage message={editError} />
              )}

              <ModalButtons
                onCancel={closeEditModal}
                submitting={submitting}
                submitText="Save Changes"
                loadingText="Saving..."
              />
            </form>
          </div>
        </div>
      )}

      {showResetModal && resetUser && (
        <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/70 p-4">
          <div className="w-full max-w-lg rounded-2xl border border-slate-700 bg-slate-900 p-6">
            <div className="mb-6 flex items-center justify-between">
              <div>
                <h2 className="text-2xl font-bold">
                  Reset Password
                </h2>

                <p className="mt-2 text-sm text-slate-400">
                  Set a new password for{" "}
                  {resetUser.firstName}{" "}
                  {resetUser.lastName}.
                </p>
              </div>

              <button
                type="button"
                onClick={closeResetModal}
                disabled={submitting}
                className="rounded-lg p-2 text-slate-400 hover:bg-slate-800 hover:text-white disabled:opacity-50"
              >
                <X size={22} />
              </button>
            </div>

            <form
              onSubmit={handleResetPassword}
              className="space-y-4"
            >
              <InputField
                label="New Password"
                name="newPassword"
                type="password"
                value={resetForm.newPassword}
                onChange={handleResetInputChange}
                minLength={8}
              />

              <InputField
                label="Confirm Password"
                name="confirmPassword"
                type="password"
                value={resetForm.confirmPassword}
                onChange={handleResetInputChange}
                minLength={8}
              />

              <p className="text-sm text-slate-500">
                Password must contain at least 8 characters.
              </p>

              {resetError && (
                <ErrorMessage message={resetError} />
              )}

              <ModalButtons
                onCancel={closeResetModal}
                submitting={submitting}
                submitText="Reset Password"
                loadingText="Resetting..."
              />
            </form>
          </div>
        </div>
      )}
    </main>
  );
}

function InputField({
  label,
  name,
  value,
  onChange,
  type = "text",
  minLength,
}) {
  return (
    <div>
      <label className="mb-2 block text-sm text-slate-300">
        {label}
      </label>

      <input
        name={name}
        type={type}
        value={value}
        onChange={onChange}
        minLength={minLength}
        required
        className="w-full rounded-lg border border-slate-700 bg-slate-950 px-4 py-3 outline-none focus:border-blue-500"
      />
    </div>
  );
}

function RoleField({ value, onChange }) {
  return (
    <div>
      <label className="mb-2 block text-sm text-slate-300">
        Role
      </label>

      <select
        name="role"
        value={value}
        onChange={onChange}
        className="w-full rounded-lg border border-slate-700 bg-slate-950 px-4 py-3 outline-none focus:border-blue-500"
      >
        <option value="1">Admin</option>
        <option value="2">Project Manager</option>
        <option value="3">Developer</option>
        <option value="4">Tester</option>
      </select>
    </div>
  );
}

function getValidationError(error) {
  const validationErrors = error.response?.data?.errors;

  if (!validationErrors) {
    return "";
  }

  return Object.values(validationErrors)
    .flat()
    .join(" ");
}

function ErrorMessage({ message }) {
  return (
    <p className="rounded-lg bg-red-500/10 p-3 text-sm text-red-400">
      {message}
    </p>
  );
}

function ModalButtons({
  onCancel,
  submitting,
  submitText,
  loadingText,
}) {
  return (
    <div className="flex justify-end gap-3 pt-2">
      <button
        type="button"
        onClick={onCancel}
        className="rounded-lg border border-slate-700 px-5 py-3 font-medium hover:bg-slate-800"
      >
        Cancel
      </button>

      <button
        type="submit"
        disabled={submitting}
        className="rounded-lg bg-blue-600 px-5 py-3 font-semibold hover:bg-blue-500 disabled:cursor-not-allowed disabled:opacity-60"
      >
        {submitting ? loadingText : submitText}
      </button>
    </div>
  );
}

export default AllUsers;