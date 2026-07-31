import { useEffect, useState } from "react";
import {
  ArrowLeft,
  Plus,
  Trash2,
  UserPlus,
  Users,
} from "lucide-react";
import { useNavigate, useParams } from "react-router";
import api from "../../api/axios";

function ManageProjectMembers() {
  const navigate = useNavigate();
  const { projectId } = useParams();

  const [project, setProject] = useState(null);
  const [members, setMembers] = useState([]);
  const [availableUsers, setAvailableUsers] = useState([]);
  const [selectedUserId, setSelectedUserId] = useState("");

  const [loading, setLoading] = useState(true);
  const [submitting, setSubmitting] = useState(false);
  const [removingId, setRemovingId] = useState(null);
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
        membersResponse,
        availableUsersResponse,
      ] = await Promise.all([
        api.get(`/api/Projects/${projectId}`),
        api.get(`/api/projects/${projectId}/members`),
        api.get(
          `/api/projects/${projectId}/members/available-users`
        ),
      ]);

      setProject(projectResponse.data);
      setMembers(membersResponse.data);
      setAvailableUsers(availableUsersResponse.data);
    } catch (requestError) {
      setError(
        requestError.response?.data?.detail ||
          requestError.response?.data?.message ||
          "Failed to load project members."
      );
    } finally {
      setLoading(false);
    }
  }

  async function handleAddMember(event) {
    event.preventDefault();

    if (!selectedUserId) {
      setError("Select a user.");
      return;
    }

    try {
      setSubmitting(true);
      setError("");
      setSuccess("");

      const response = await api.post(
        `/api/projects/${projectId}/members`,
        {
          userId: Number(selectedUserId),
        }
      );

      setMembers((previousMembers) => [
        ...previousMembers,
        response.data,
      ]);

      setAvailableUsers((previousUsers) =>
        previousUsers.filter(
          (user) => user.userId !== Number(selectedUserId)
        )
      );

      setSelectedUserId("");
      setSuccess("Member added successfully.");
    } catch (requestError) {
      setError(
        requestError.response?.data?.detail ||
          requestError.response?.data?.message ||
          "Failed to add member."
      );
    } finally {
      setSubmitting(false);
    }
  }

  async function handleRemoveMember(member) {
    const confirmed = window.confirm(
      `Remove ${member.fullName} from this project?`
    );

    if (!confirmed) {
      return;
    }

    try {
      setRemovingId(member.projectMemberId);
      setError("");
      setSuccess("");

      await api.delete(
        `/api/projects/${projectId}/members/${member.projectMemberId}`
      );

      setMembers((previousMembers) =>
        previousMembers.filter(
          (existingMember) =>
            existingMember.projectMemberId !==
            member.projectMemberId
        )
      );

      setAvailableUsers((previousUsers) => [
        ...previousUsers,
        {
          userId: member.userId,
          fullName: member.fullName,
          email: member.email,
          role: member.role,
        },
      ]);

      setSuccess("Member removed successfully.");
    } catch (requestError) {
      setError(
        requestError.response?.data?.detail ||
          requestError.response?.data?.message ||
          "Failed to remove member."
      );
    } finally {
      setRemovingId(null);
    }
  }

  if (loading) {
    return (
      <div className="flex min-h-screen items-center justify-center bg-slate-950 text-white">
        Loading members...
      </div>
    );
  }

  return (
    <main className="min-h-screen bg-slate-950 p-6 text-white">
      <div className="mx-auto max-w-6xl">
        <button
          onClick={() => navigate("/manager/dashboard")}
          className="mb-5 flex items-center gap-2 text-slate-400 hover:text-white"
        >
          <ArrowLeft size={18} />
          Back to dashboard
        </button>

        <div className="mb-8">
          <h1 className="flex items-center gap-3 text-3xl font-bold">
            <Users className="text-blue-500" />
            Manage Project Members
          </h1>

          <p className="mt-2 text-slate-400">
            {project?.projectName} · {project?.projectCode}
          </p>
        </div>

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

        <section className="mb-8 rounded-2xl border border-slate-800 bg-slate-900 p-5">
          <h2 className="mb-4 flex items-center gap-2 text-xl font-semibold">
            <UserPlus size={21} />
            Add Member
          </h2>

          <form
            onSubmit={handleAddMember}
            className="flex flex-col gap-4 sm:flex-row"
          >
            <select
              value={selectedUserId}
              onChange={(event) =>
                setSelectedUserId(event.target.value)
              }
              className="flex-1 rounded-lg border border-slate-700 bg-slate-950 px-4 py-3 outline-none focus:border-blue-500"
            >
              <option value="">
                Select available Developer or Tester
              </option>

              {availableUsers.map((user) => (
                <option
                  key={user.userId}
                  value={user.userId}
                >
                  {user.fullName} — {user.role}
                </option>
              ))}
            </select>

            <button
              type="submit"
              disabled={submitting || availableUsers.length === 0}
              className="flex items-center justify-center gap-2 rounded-lg bg-blue-600 px-5 py-3 font-semibold hover:bg-blue-500 disabled:cursor-not-allowed disabled:opacity-50"
            >
              <Plus size={18} />
              {submitting ? "Adding..." : "Add Member"}
            </button>
          </form>

          {availableUsers.length === 0 && (
            <p className="mt-3 text-sm text-slate-400">
              No available users found.
            </p>
          )}
        </section>

        <section className="overflow-hidden rounded-2xl border border-slate-800 bg-slate-900">
          <div className="border-b border-slate-800 p-5">
            <h2 className="text-xl font-semibold">
              Current Members ({members.length})
            </h2>
          </div>

          <div className="overflow-x-auto">
            <table className="w-full text-left">
              <thead className="bg-slate-800/50 text-sm text-slate-400">
                <tr>
                  <th className="p-4">Name</th>
                  <th className="p-4">Email</th>
                  <th className="p-4">Role</th>
                  <th className="p-4">Joined</th>
                  <th className="p-4">Action</th>
                </tr>
              </thead>

              <tbody>
                {members.map((member) => (
                  <tr
                    key={member.projectMemberId}
                    className="border-t border-slate-800"
                  >
                    <td className="p-4 font-medium">
                      {member.fullName}
                    </td>

                    <td className="p-4 text-slate-400">
                      {member.email}
                    </td>

                    <td className="p-4">
                      <span
                        className={
                          member.role === "Developer"
                            ? "rounded-full bg-blue-500/10 px-3 py-1 text-sm text-blue-400"
                            : "rounded-full bg-violet-500/10 px-3 py-1 text-sm text-violet-400"
                        }
                      >
                        {member.role}
                      </span>
                    </td>

                    <td className="p-4 text-slate-400">
                      {new Date(
                        member.joinedDate
                      ).toLocaleDateString()}
                    </td>

                    <td className="p-4">
                      <button
                        onClick={() =>
                          handleRemoveMember(member)
                        }
                        disabled={
                          removingId ===
                          member.projectMemberId
                        }
                        className="flex items-center gap-2 rounded-lg bg-red-500/10 px-3 py-2 text-sm text-red-400 hover:bg-red-500/20 disabled:opacity-50"
                      >
                        <Trash2 size={16} />

                        {removingId === member.projectMemberId
                          ? "Removing..."
                          : "Remove"}
                      </button>
                    </td>
                  </tr>
                ))}

                {members.length === 0 && (
                  <tr>
                    <td
                      colSpan="5"
                      className="p-8 text-center text-slate-400"
                    >
                      No members assigned.
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

export default ManageProjectMembers;