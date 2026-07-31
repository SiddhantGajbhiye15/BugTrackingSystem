import { ArrowLeft, Home, SearchX } from "lucide-react";
import { useNavigate } from "react-router";

function NotFound() {
  const navigate = useNavigate();

  return (
    <main className="flex min-h-screen items-center justify-center bg-slate-950 p-6 text-white">
      <section className="w-full max-w-xl rounded-2xl border border-slate-800 bg-slate-900 p-8 text-center shadow-xl">
        <div className="mx-auto flex h-20 w-20 items-center justify-center rounded-2xl bg-blue-500/10 text-blue-400">
          <SearchX size={42} />
        </div>

        <p className="mt-6 text-sm font-semibold uppercase tracking-[0.25em] text-blue-400">
          Error 404
        </p>

        <h1 className="mt-3 text-4xl font-bold">
          Page not found
        </h1>

        <p className="mt-4 leading-7 text-slate-400">
          The page may have been moved, deleted, or the
          address may be incorrect.
        </p>

        <div className="mt-8 flex flex-col justify-center gap-3 sm:flex-row">
          <button
            onClick={() => navigate(-1)}
            className="flex items-center justify-center gap-2 rounded-lg border border-slate-700 px-5 py-3 font-medium text-slate-300 hover:bg-slate-800"
          >
            <ArrowLeft size={18} />
            Go Back
          </button>

          <button
            onClick={() => navigate("/login")}
            className="flex items-center justify-center gap-2 rounded-lg bg-blue-600 px-5 py-3 font-semibold hover:bg-blue-500"
          >
            <Home size={18} />
            Go to Login
          </button>
        </div>
      </section>
    </main>
  );
}

export default NotFound;