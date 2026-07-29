import { useNavigate } from "react-router";

function DashboardPage({ title }) {
  const navigate = useNavigate();

  const user = JSON.parse(localStorage.getItem("user"));

  function handleLogout() {
    localStorage.removeItem("token");
    localStorage.removeItem("user");
    navigate("/login");
  }

  return (
    <main className="min-h-screen bg-slate-950 p-8 text-white">
      <div className="mx-auto max-w-6xl">
        <div className="flex items-center justify-between">
          <div>
            <h1 className="text-3xl font-bold">{title}</h1>

            <p className="mt-2 text-slate-400">
              Welcome, {user?.firstName} {user?.lastName}
            </p>
          </div>

          <button
            onClick={handleLogout}
            className="rounded-lg bg-red-600 px-4 py-2 font-medium hover:bg-red-500"
          >
            Logout
          </button>
        </div>
      </div>
    </main>
  );
}

export default DashboardPage;