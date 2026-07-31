import { useEffect, useState } from "react";
import {
  ArrowLeft,
  Bug,
  CalendarDays,
  Edit3,
  ExternalLink,
  MessageSquare,
  Save,
  Send,
  Trash2,
  User,
  UserCheck,
  X,
} from "lucide-react";
import { useNavigate, useParams } from "react-router";
import api from "../../api/axios";

function BugDetails() {
  const navigate = useNavigate();
  const { bugId } = useParams();

  const [bug, setBug] = useState(null);
  const [comments, setComments] = useState([]);
  const [newComment, setNewComment] = useState("");
  const [editingCommentId, setEditingCommentId] =
    useState(null);
  const [editingContent, setEditingContent] =
    useState("");

  const [loading, setLoading] = useState(true);
  const [submittingComment, setSubmittingComment] =
    useState(false);
  const [savingCommentId, setSavingCommentId] =
    useState(null);
  const [deletingCommentId, setDeletingCommentId] =
    useState(null);
  const [error, setError] = useState("");
  const [success, setSuccess] = useState("");

  const storedUser = localStorage.getItem("user");
  const currentUser = storedUser
    ? JSON.parse(storedUser)
    : null;

  useEffect(() => {
    loadBugDetails();
  }, [bugId]);

  async function loadBugDetails() {
    try {
      setLoading(true);
      setError("");

      const [bugResponse, commentsResponse] =
        await Promise.all([
          api.get(`/api/bugs/${bugId}`),
          api.get(`/api/bugs/${bugId}/comments`),
        ]);

      setBug(bugResponse.data);
      setComments(commentsResponse.data);
    } catch (requestError) {
      setError(
        getErrorMessage(
          requestError,
          "Failed to load bug details."
        )
      );
    } finally {
      setLoading(false);
    }
  }

  async function handleAddComment(event) {
    event.preventDefault();

    const content = newComment.trim();

    if (!content) {
      setError("Write a comment before submitting.");
      return;
    }

    try {
      setSubmittingComment(true);
      setError("");
      setSuccess("");

      const response = await api.post(
        `/api/bugs/${bugId}/comments`,
        {
          commentText: content,
        }
      );

      setComments((previousComments) => [
        ...previousComments,
        response.data,
      ]);

      setNewComment("");
      setSuccess("Comment added successfully.");
    } catch (requestError) {
      setError(
        getErrorMessage(
          requestError,
          "Failed to add comment."
        )
      );
    } finally {
      setSubmittingComment(false);
    }
  }

  function startEditing(comment) {
    setEditingCommentId(comment.commentId);
    setEditingContent(comment.commentText);
    setError("");
    setSuccess("");
  }

  function cancelEditing() {
    setEditingCommentId(null);
    setEditingContent("");
  }

  async function handleUpdateComment(commentId) {
    const content = editingContent.trim();

    if (!content) {
      setError("Comment cannot be empty.");
      return;
    }

    try {
      setSavingCommentId(commentId);
      setError("");
      setSuccess("");

      const response = await api.put(
        `/api/comments/${commentId}`,
        {
          commentText: content,
        }
      );

      setComments((previousComments) =>
        previousComments.map((comment) =>
          comment.commentId === commentId
            ? response.data
            : comment
        )
      );

      cancelEditing();
      setSuccess("Comment updated successfully.");
    } catch (requestError) {
      setError(
        getErrorMessage(
          requestError,
          "Failed to update comment."
        )
      );
    } finally {
      setSavingCommentId(null);
    }
  }

  async function handleDeleteComment(comment) {
    const confirmed = window.confirm(
      "Delete this comment permanently?"
    );

    if (!confirmed) {
      return;
    }

    try {
      setDeletingCommentId(comment.commentId);
      setError("");
      setSuccess("");

      await api.delete(
        `/api/comments/${comment.commentId}`
      );

      setComments((previousComments) =>
        previousComments.filter(
          (existingComment) =>
            existingComment.commentId !==
            comment.commentId
        )
      );

      if (
        editingCommentId === comment.commentId
      ) {
        cancelEditing();
      }

      setSuccess("Comment deleted successfully.");
    } catch (requestError) {
      setError(
        getErrorMessage(
          requestError,
          "Failed to delete comment."
        )
      );
    } finally {
      setDeletingCommentId(null);
    }
  }

  function handleBack() {
    const role = Number(currentUser?.role);

    if (role === 2 && bug?.projectId) {
      navigate(
        `/manager/projects/${bug.projectId}/bugs`
      );
      return;
    }

    if (role === 3) {
      navigate("/developer/dashboard");
      return;
    }

    if (role === 4) {
      navigate("/tester/dashboard");
      return;
    }

    navigate("/login");
  }

  function ownsComment(comment) {
    return (
      Number(comment.userId) ===
      Number(currentUser?.userId)
    );
  }

  if (loading) {
    return (
      <div className="flex min-h-screen items-center justify-center bg-slate-950 text-white">
        Loading bug details...
      </div>
    );
  }

  if (!bug) {
    return (
      <main className="flex min-h-screen items-center justify-center bg-slate-950 p-6 text-white">
        <div className="text-center">
          <p className="mb-4 text-slate-300">
            Bug details could not be loaded.
          </p>

          <button
            onClick={handleBack}
            className="rounded-lg bg-blue-600 px-4 py-2 hover:bg-blue-500"
          >
            Go Back
          </button>
        </div>
      </main>
    );
  }

  const evidence = bug.evidenceLink?.trim();
  const hasEvidence =
    evidence &&
    !["NA", "N/A", "NONE", "NULL"].includes(
      evidence.toUpperCase()
    );
  const evidenceIsLink =
    hasEvidence &&
    /^https?:\/\//i.test(evidence);

  return (
    <main className="min-h-screen bg-slate-950 p-6 text-white">
      <div className="mx-auto max-w-6xl">
        <button
          onClick={handleBack}
          className="mb-6 flex items-center gap-2 text-slate-400 hover:text-white"
        >
          <ArrowLeft size={18} />
          Back
        </button>

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

        <section className="mb-7 rounded-2xl border border-slate-800 bg-slate-900 p-6">
          <div className="flex flex-col justify-between gap-5 lg:flex-row lg:items-start">
            <div>
              <p className="mb-2 text-sm font-medium text-blue-400">
                {bug.projectCode} · {bug.projectName}
              </p>

              <h1 className="flex items-start gap-3 text-3xl font-bold">
                <Bug
                  className="mt-1 shrink-0 text-red-500"
                  size={30}
                />
                {bug.title}
              </h1>

              <p className="mt-4 max-w-4xl whitespace-pre-wrap leading-7 text-slate-300">
                {bug.description}
              </p>
            </div>

            <div className="flex shrink-0 flex-wrap gap-2">
              <span
                className={`rounded-full px-3 py-1 text-sm ${getPriorityStyle(
                  bug.priority
                )}`}
              >
                {bug.priority}
              </span>

              <span
                className={`rounded-full px-3 py-1 text-sm ${getStatusStyle(
                  bug.status
                )}`}
              >
                {formatStatus(bug.status)}
              </span>

              <span className="rounded-full bg-slate-800 px-3 py-1 text-sm text-slate-300">
                {bug.type}
              </span>
            </div>
          </div>
        </section>

        <div className="mb-7 grid gap-6 lg:grid-cols-[2fr_1fr]">
          <div className="space-y-6">
            <DetailSection title="Steps to Reproduce">
              {bug.stepsToReproduce}
            </DetailSection>

            <div className="grid gap-6 md:grid-cols-2">
              <DetailSection title="Expected Output">
                {bug.expectedOutput}
              </DetailSection>

              <DetailSection title="Actual Output">
                {bug.actualOutput}
              </DetailSection>
            </div>

            <section className="rounded-2xl border border-slate-800 bg-slate-900 p-5">
              <h2 className="mb-3 text-lg font-semibold">
                Evidence
              </h2>

              {!hasEvidence && (
                <p className="text-slate-400">
                  No evidence link was provided.
                </p>
              )}

              {hasEvidence && evidenceIsLink && (
                <a
                  href={evidence}
                  target="_blank"
                  rel="noreferrer"
                  className="inline-flex items-center gap-2 break-all text-blue-400 hover:text-blue-300"
                >
                  Open evidence
                  <ExternalLink size={16} />
                </a>
              )}

              {hasEvidence && !evidenceIsLink && (
                <p className="break-all text-slate-300">
                  {evidence}
                </p>
              )}
            </section>
          </div>

          <aside className="h-fit rounded-2xl border border-slate-800 bg-slate-900 p-5">
            <h2 className="mb-5 text-lg font-semibold">
              Bug Information
            </h2>

            <InfoRow
              icon={<User size={18} />}
              label="Reported by"
              value={bug.reporterName}
            />

            <InfoRow
              icon={<UserCheck size={18} />}
              label="Assigned Developer"
              value={
                bug.assignedDeveloperName ||
                "Unassigned"
              }
            />

            <InfoRow
              icon={<CalendarDays size={18} />}
              label="Created"
              value={formatDateTime(bug.createdAt)}
            />

            <InfoRow
              icon={<CalendarDays size={18} />}
              label="Last Updated"
              value={
                bug.updatedAt
                  ? formatDateTime(bug.updatedAt)
                  : "Not updated"
              }
              last
            />
          </aside>
        </div>

        <section className="rounded-2xl border border-slate-800 bg-slate-900">
          <div className="flex items-center gap-3 border-b border-slate-800 p-5">
            <MessageSquare className="text-blue-400" />
            <div>
              <h2 className="text-xl font-semibold">
                Comments
              </h2>
              <p className="text-sm text-slate-400">
                {comments.length} comment
                {comments.length === 1 ? "" : "s"}
              </p>
            </div>
          </div>

          <form
            onSubmit={handleAddComment}
            className="border-b border-slate-800 p-5"
          >
            <label className="mb-2 block text-sm font-medium text-slate-300">
              Add Comment
            </label>

            <textarea
              value={newComment}
              onChange={(event) =>
                setNewComment(event.target.value)
              }
              rows="4"
              placeholder="Write an update, question, or testing note..."
              className="form-input resize-none"
            />

            <div className="mt-3 flex justify-end">
              <button
                type="submit"
                disabled={
                  submittingComment ||
                  !newComment.trim()
                }
                className="flex items-center gap-2 rounded-lg bg-blue-600 px-4 py-2 font-medium hover:bg-blue-500 disabled:cursor-not-allowed disabled:opacity-50"
              >
                <Send size={17} />
                {submittingComment
                  ? "Posting..."
                  : "Post Comment"}
              </button>
            </div>
          </form>

          <div className="divide-y divide-slate-800">
            {comments.map((comment) => {
              const isOwner = ownsComment(comment);
              const isEditing =
                editingCommentId ===
                comment.commentId;

              return (
                <article
                  key={comment.commentId}
                  className="p-5"
                >
                  <div className="mb-3 flex flex-col justify-between gap-3 sm:flex-row sm:items-start">
                    <div>
                      <p className="font-semibold">
                        {comment.userName}
                      </p>

                      <p className="mt-1 text-xs text-slate-500">
                        {formatDateTime(
                          comment.updatedAt ||
                            comment.createdAt
                        )}
                        {comment.isEdited
                          ? " · Edited"
                          : ""}
                      </p>
                    </div>

                    {isOwner && !isEditing && (
                      <div className="flex gap-2">
                        <button
                          onClick={() =>
                            startEditing(comment)
                          }
                          className="flex items-center gap-2 rounded-lg bg-amber-500/10 px-3 py-2 text-sm text-amber-400 hover:bg-amber-500/20"
                        >
                          <Edit3 size={15} />
                          Edit
                        </button>

                        <button
                          onClick={() =>
                            handleDeleteComment(comment)
                          }
                          disabled={
                            deletingCommentId ===
                            comment.commentId
                          }
                          className="flex items-center gap-2 rounded-lg bg-red-500/10 px-3 py-2 text-sm text-red-400 hover:bg-red-500/20 disabled:opacity-50"
                        >
                          <Trash2 size={15} />
                          {deletingCommentId ===
                          comment.commentId
                            ? "Deleting..."
                            : "Delete"}
                        </button>
                      </div>
                    )}
                  </div>

                  {!isEditing && (
                    <p className="whitespace-pre-wrap leading-7 text-slate-300">
                      {comment.commentText}
                    </p>
                  )}

                  {isEditing && (
                    <div>
                      <textarea
                        value={editingContent}
                        onChange={(event) =>
                          setEditingContent(
                            event.target.value
                          )
                        }
                        rows="4"
                        className="form-input resize-none"
                      />

                      <div className="mt-3 flex justify-end gap-2">
                        <button
                          type="button"
                          onClick={cancelEditing}
                          className="flex items-center gap-2 rounded-lg border border-slate-700 px-3 py-2 text-sm text-slate-300 hover:bg-slate-800"
                        >
                          <X size={15} />
                          Cancel
                        </button>

                        <button
                          type="button"
                          onClick={() =>
                            handleUpdateComment(
                              comment.commentId
                            )
                          }
                          disabled={
                            savingCommentId ===
                              comment.commentId ||
                            !editingContent.trim()
                          }
                          className="flex items-center gap-2 rounded-lg bg-blue-600 px-3 py-2 text-sm font-medium hover:bg-blue-500 disabled:opacity-50"
                        >
                          <Save size={15} />
                          {savingCommentId ===
                          comment.commentId
                            ? "Saving..."
                            : "Save"}
                        </button>
                      </div>
                    </div>
                  )}
                </article>
              );
            })}

            {comments.length === 0 && (
              <div className="p-10 text-center text-slate-400">
                No comments yet. Start the discussion.
              </div>
            )}
          </div>
        </section>
      </div>
    </main>
  );
}

function DetailSection({ title, children }) {
  return (
    <section className="rounded-2xl border border-slate-800 bg-slate-900 p-5">
      <h2 className="mb-3 text-lg font-semibold">
        {title}
      </h2>

      <p className="whitespace-pre-wrap leading-7 text-slate-300">
        {children || "Not provided"}
      </p>
    </section>
  );
}

function InfoRow({
  icon,
  label,
  value,
  last = false,
}) {
  return (
    <div
      className={`flex gap-3 py-4 ${
        last ? "" : "border-b border-slate-800"
      }`}
    >
      <div className="mt-0.5 text-blue-400">
        {icon}
      </div>

      <div>
        <p className="text-xs uppercase tracking-wide text-slate-500">
          {label}
        </p>

        <p className="mt-1 text-sm text-slate-200">
          {value}
        </p>
      </div>
    </div>
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

function formatStatus(status) {
  return status.replace(/([A-Z])/g, " $1").trim();
}

function formatDateTime(date) {
  if (!date) {
    return "Not available";
  }

  return new Date(date).toLocaleString();
}

function getErrorMessage(error, fallbackMessage) {
  return (
    error.response?.data?.detail ||
    error.response?.data?.message ||
    fallbackMessage
  );
}

export default BugDetails;