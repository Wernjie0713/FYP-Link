import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import { AuthProvider } from './context/AuthContext';
import { useAuth } from './context/AuthContext';
import { Toaster } from 'react-hot-toast';
import Login from './components/auth/Login';
import Register from './components/auth/Register';
import MainLayout from './components/layout/MainLayout';
import ProtectedRoute from './components/auth/ProtectedRoute';
import ManagePrograms from './pages/admin/ManagePrograms';
import ManageLecturers from './pages/admin/ManageLecturers';
import ManageCommittee from './pages/admin/ManageCommittee';
import ConfirmEmail from './pages/auth/ConfirmEmail';
import RegistrationSuccess from './pages/auth/RegistrationSuccess';
import Settings from './pages/Settings';
import SupervisorApprovals from './pages/committee/SupervisorApprovals';
import ManageProposals from './pages/committee/ManageProposals';
import SelectSupervisor from './pages/student/SelectSupervisor';
import SubmitProposal from './pages/student/SubmitProposal';
import MyProposal from './pages/student/MyProposal';
import EditProposal from './pages/student/EditProposal';

// Placeholder components for dashboard pages
const Dashboard = () => (
  <div>
    <h1 className="text-3xl font-bold mb-4">Dashboard</h1>
    <p className="text-gray-600">Welcome to your FYP-Link dashboard!</p>
  </div>
);

const Proposals = () => (
  <div>
    <h1 className="text-3xl font-bold mb-4">Proposals</h1>
    <p className="text-gray-600">Manage your project proposals here.</p>
  </div>
);

// Admin role check component
const AdminRoute = ({ children }) => {
  const { user } = useAuth();
  
  if (!user?.roles?.includes('Admin')) {
    console.log('Access denied: User is not an admin', user);
    return <Navigate to="/dashboard" replace />;
  }

  return children;
};

function App() {
  return (
    <div className="min-h-screen bg-slate-50">
      <Toaster position="top-right" />
      <AuthProvider>
        <Router>
          <Routes>
            {/* Public routes */}
            <Route path="/login" element={<Login />} />
            <Route path="/register" element={<Register />} />
            <Route path="/register-success" element={<RegistrationSuccess />} />
            <Route path="/confirm-email" element={<ConfirmEmail />} />

            {/* Protected routes */}
            <Route
              path="/"
              element={
                <ProtectedRoute>
                  <MainLayout />
                </ProtectedRoute>
              }
            >
              <Route index element={<Navigate to="/dashboard" replace />} />
              <Route path="dashboard" element={<Dashboard />} />
              <Route path="proposals" element={<Proposals />} />
              <Route path="settings" element={<Settings />} />
              
              {/* Student routes */}
              <Route
                path="student/select-supervisor"
                element={
                  <ProtectedRoute allowedRoles={['Student']}>
                    <SelectSupervisor />
                  </ProtectedRoute>
                }
              />
              <Route
                path="student/submit-proposal"
                element={
                  <ProtectedRoute allowedRoles={['Student']}>
                    <SubmitProposal />
                  </ProtectedRoute>
                }
              />
              <Route
                path="student/my-proposal"
                element={
                  <ProtectedRoute allowedRoles={['Student']}>
                    <MyProposal />
                  </ProtectedRoute>
                }
              />\
              <Route
                path="/student/proposal/edit/:proposalId"
                element={
                  <ProtectedRoute allowedRoles={['Student']}>
                    <EditProposal />
                  </ProtectedRoute>
                }
              />

              {/* Admin routes */}
              <Route
                path="admin/programs"
                element={
                  <AdminRoute>
                    <ManagePrograms />
                  </AdminRoute>
                }
              />
              <Route
                path="admin/lecturers"
                element={
                  <ProtectedRoute allowedRoles={['Admin', 'Committee']}>
                    <ManageLecturers />
                  </ProtectedRoute>
                }
              />
              <Route
                path="admin/committee"
                element={
                  <AdminRoute>
                    <ManageCommittee />
                  </AdminRoute>
                }
              />

              {/* Committee routes */}
              <Route
                path="committee/approvals"
                element={
                  <ProtectedRoute allowedRoles={['Committee']}>
                    <SupervisorApprovals />
                  </ProtectedRoute>
                }
              />
              <Route
                path="committee/proposals"
                element={
                  <ProtectedRoute allowedRoles={['Committee']}>
                    <ManageProposals />
                  </ProtectedRoute>
                }
              />
            </Route>
          </Routes>
        </Router>
      </AuthProvider>
    </div>
  );
}

export default App;
