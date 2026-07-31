import { useState } from "react";
import {
  ArrowLeft,
  Bug,
  Save,
} from "lucide-react";
import { useNavigate, useParams } from "react-router";
import api from "../../api/axios";

const initialForm = {
  title: "",
  description: "",
  type: "1",
  priority: "2",
  expectedOutput: "",
  actualOutput: "",
  stepsToReproduce: "",
  evidenceLink: "",
};

function CreateBug() {
  const navigate = useNavigate();
  const { projectId } = useParams();

  const [form, setForm] = useState(initialForm);
  const [submitting, setSubmitting] = useState(false);
  const [error, setError] = useState("");

  function handleChange(event) {
    const { name, value } = event.target;

    setForm((previousForm) => ({
      ...previousForm,
      [name]: value,
    }));
  }

  async function handleSubmit(event) {
    event.preventDefault();

    try {
      setSubmitting(true);
      setError("");

      await api.post(
        `/api/projects/${projectId}/bugs`,
        {
          title: form.title.trim(),
          description: form.description.trim(),
          type: Number(form.type),
          priority: Number(form.priority),
          expectedOutput: form.expectedOutput.trim(),
          actualOutput: form.actualOutput.trim(),
          stepsToReproduce:
            form.stepsToReproduce.trim(),
          evidenceLink:
            form.evidenceLink.trim() || null,
        }
      );

      navigate("/tester/dashboard");
    } catch (requestError) {
      setError(
        requestError.response?.data?.detail ||
          requestError.response?.data?.message ||
          "Failed to create bug."
      );
    } finally {
      setSubmitting(false);
    }
  }

  return (
    <main className="min-h-screen bg-slate-950 p-6 text-white">
      <div className="mx-auto max-w-4xl">
        <button
          onClick={() =>
            navigate("/tester/dashboard")
          }
          className="mb-6 flex items-center gap-2 text-slate-400 hover:text-white"
        >
          <ArrowLeft size={18} />
          Back to dashboard
        </button>

        <div className="mb-8">
          <h1 className="flex items-center gap-3 text-3xl font-bold">
            <Bug className="text-red-500" />
            Report New Bug
          </h1>

          <p className="mt-2 text-slate-400">
            Describe the problem clearly so the
            Developer can reproduce it.
          </p>
        </div>

        {error && (
          <div className="mb-5 rounded-lg bg-red-500/10 p-4 text-red-400">
            {error}
          </div>
        )}

        <form
          onSubmit={handleSubmit}
          className="space-y-6 rounded-2xl border border-slate-800 bg-slate-900 p-6"
        >
          <FormField label="Bug Title">
            <input
              name="title"
              value={form.title}
              onChange={handleChange}
              required
              placeholder="Example: Login button is not responding"
              className="form-input"
            />
          </FormField>

          <FormField label="Description">
            <textarea
              name="description"
              value={form.description}
              onChange={handleChange}
              required
              rows="4"
              placeholder="Explain what is happening."
              className="form-input resize-none"
            />
          </FormField>

          <div className="grid gap-5 md:grid-cols-2">
            <FormField label="Bug Type">
              <select
                name="type"
                value={form.type}
                onChange={handleChange}
                className="form-input"
              >
                <option value="1">UI</option>
                <option value="2">Functional</option>
                <option value="3">Performance</option>
                <option value="4">Security</option>
                <option value="5">Other</option>
              </select>
            </FormField>

            <FormField label="Priority">
              <select
                name="priority"
                value={form.priority}
                onChange={handleChange}
                className="form-input"
              >
                <option value="1">Low</option>
                <option value="2">Medium</option>
                <option value="3">High</option>
                <option value="4">Critical</option>
              </select>
            </FormField>
          </div>

          <FormField label="Steps to Reproduce">
            <textarea
              name="stepsToReproduce"
              value={form.stepsToReproduce}
              onChange={handleChange}
              required
              rows="4"
              placeholder="1. Open login page&#10;2. Enter credentials&#10;3. Click Login"
              className="form-input resize-none"
            />
          </FormField>

          <div className="grid gap-5 md:grid-cols-2">
            <FormField label="Expected Output">
              <textarea
                name="expectedOutput"
                value={form.expectedOutput}
                onChange={handleChange}
                required
                rows="4"
                placeholder="What should happen?"
                className="form-input resize-none"
              />
            </FormField>

            <FormField label="Actual Output">
              <textarea
                name="actualOutput"
                value={form.actualOutput}
                onChange={handleChange}
                required
                rows="4"
                placeholder="What actually happened?"
                className="form-input resize-none"
              />
            </FormField>
          </div>

          <FormField label="Evidence Link">
            <input
              name="evidenceLink"
              value={form.evidenceLink}
              onChange={handleChange}
              placeholder="Screenshot or video URL — optional"
              className="form-input"
            />
          </FormField>

          <div className="flex justify-end gap-3 border-t border-slate-800 pt-6">
            <button
              type="button"
              onClick={() =>
                navigate("/tester/dashboard")
              }
              className="rounded-lg border border-slate-700 px-5 py-3 text-slate-300 hover:bg-slate-800"
            >
              Cancel
            </button>

            <button
              type="submit"
              disabled={submitting}
              className="flex items-center gap-2 rounded-lg bg-blue-600 px-5 py-3 font-semibold hover:bg-blue-500 disabled:cursor-not-allowed disabled:opacity-50"
            >
              <Save size={18} />
              {submitting
                ? "Creating..."
                : "Create Bug"}
            </button>
          </div>
        </form>
      </div>
    </main>
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

export default CreateBug;