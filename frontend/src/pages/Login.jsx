import { useState } from "react";
import { useNavigate } from "react-router";
import { Bug, LockKeyhole, Mail } from "lucide-react";
import api from "../api/axios";
function LoginPage() {
  async function handleSubmit(event) {
  event.preventDefault();

  try {
    setLoading(true);
    setError("");

    const response = await api.post("/api/Auth/login", {
      email,
      password,
    });

    const { token, user } = response.data;

    if (!user.isActive) {
      setError("Your account is inactive. Contact the administrator.");
      return;
    }

    localStorage.setItem("token", token);
    localStorage.setItem("user", JSON.stringify(user));

    switch (user.role) {
      case 1:
        navigate("/admin/dashboard");
        break;

      case 2:
        navigate("/manager/dashboard");
        break;

      case 3:
        navigate("/developer/dashboard");
        break;

      case 4:
        navigate("/tester/dashboard");
        break;

      default:
        setError("Invalid user role.");
    }
  } catch (error) {
    setError(
      error.response?.data?.detail ||
      error.response?.data?.message ||
      "Invalid email or password."
    );
  } finally {
    setLoading(false);
  }
}

  const navigate = useNavigate();
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState("");
  const [loading, setLoading] = useState(false);

  return (
    <main className="flex min-h-screen bg-slate-950">
      <section className="hidden w-1/2 flex-col justify-between bg-blue-600 p-12 text-white lg:flex">
        <div className="flex items-center gap-3">
          <Bug size={36} />
          <span className="text-2xl font-bold">BugTracker</span>
        </div>

        <div>
          <h1 className="max-w-lg text-5xl font-bold leading-tight">
            Track bugs. Collaborate better. Ship confidently.
          </h1>

          <p className="mt-6 max-w-md text-lg text-blue-100">
            Manage projects, assign issues and monitor development progress
            from one place.
          </p>
        </div>

        <p className="text-sm text-blue-200">
          Bug Tracking System
        </p>
      </section>

      <section className="flex w-full items-center justify-center p-6 lg:w-1/2">
        <div className="w-full max-w-md">
          <div className="mb-8 lg:hidden">
            <div className="flex items-center gap-2 text-white">
              <Bug size={32} className="text-blue-500" />
              <span className="text-2xl font-bold">BugTracker</span>
            </div>
          </div>

          <h2 className="text-3xl font-bold text-white">
            Welcome back
          </h2>

          <p className="mt-2 text-slate-400">
            Sign in to continue to your dashboard.
          </p>

          <form
            onSubmit={handleSubmit}
            className="mt-8 space-y-5"
          >
            <div>
              <label
                htmlFor="email"
                className="mb-2 block text-sm font-medium text-slate-300"
              >
                Email address
              </label>

              <div className="relative">
                <Mail
                  size={19}
                  className="absolute left-3 top-1/2 -translate-y-1/2 text-slate-500"
                />

                <input
              id="email"
              type="email"
              value={email}
              onChange={(event) => setEmail(event.target.value)}
              placeholder="manager@company.com"
              required
              className="w-full rounded-xl border border-slate-700 bg-slate-900 py-3 pl-11 pr-4 text-white outline-none focus:border-blue-500"
            />
              </div>
            </div>

            <div>
              <label
                htmlFor="password"
                className="mb-2 block text-sm font-medium text-slate-300"
              >
                Password
              </label>

              <div className="relative">
                <LockKeyhole
                  size={19}
                  className="absolute left-3 top-1/2 -translate-y-1/2 text-slate-500"
                />

                <input
                  id="password"
                  type="password"
                  value={password}
                  onChange={(event) => setPassword(event.target.value)}
                  placeholder="Enter your password"
                  required
                  className="w-full rounded-xl border border-slate-700 bg-slate-900 py-3 pl-11 pr-4 text-white outline-none focus:border-blue-500"
                />
              </div>
            </div>
            {error && (
              <p className="rounded-lg bg-red-500/10 p-3 text-sm text-red-400">
                {error}
              </p>
            )}

            <button
              type="submit"
              disabled={loading}
              className="w-full rounded-xl bg-blue-600 py-3 font-semibold text-white hover:bg-blue-500 disabled:cursor-not-allowed disabled:opacity-60"
            >
              {loading ? "Signing in..." : "Sign In"}
            </button>
          </form>
        </div>
      </section>
    </main>
  );
}

export default LoginPage;