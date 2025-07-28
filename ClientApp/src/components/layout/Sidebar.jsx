import { Link, useLocation, useNavigate } from 'react-router-dom';
import { useAuth } from '../../context/AuthContext';
import { useState, useEffect } from 'react';
import apiClient from '../../api/apiClient';
import { RxDashboard } from "react-icons/rx";
import { FiFileText, FiSettings, FiLogOut, FiUser, FiBook, FiUsers, FiUserPlus, FiEdit } from "react-icons/fi";

export default function Sidebar() {
  const location = useLocation();
  const navigate = useNavigate();
  const { user, logout } = useAuth();
  const [studentProfile, setStudentProfile] = useState(null);
  const [hasProposal, setHasProposal] = useState(false);

  const isActive = (path) => location.pathname === path;

  const handleLogout = () => {
    logout();
    navigate('/login');
  };

  // Check user roles
  const isAdmin = user?.roles?.includes('Admin');
  const canViewLecturers = user?.roles?.includes('Admin') || user?.roles?.includes('Committee');
  const isCommittee = user?.roles?.includes('Committee');
  const isStudent = user?.roles?.includes('Student');
  const isSupervisor = user?.roles?.includes('Supervisor');
  const isEvaluator = user?.roles?.includes('Evaluator');
  
  // Fetch student profile and proposal status if user is a student
  useEffect(() => {
    if (isStudent) {
      const fetchStudentData = async () => {
        try {
          // Fetch student profile
          const profileResponse = await apiClient.get('/api/students/my-profile');
          setStudentProfile(profileResponse.data);
          setHasProposal(profileResponse.data.hasProposal);
        } catch (error) {
          console.error('Error fetching student data:', error);
        }
      };
      
      fetchStudentData();
    }
  }, [isStudent]);

  const navLinks = [
    { path: '/dashboard', icon: <RxDashboard size={20} />, label: 'Dashboard' },
    // Student-specific links
    ...(isStudent ? [
      { path: '/student/select-supervisor', icon: <FiUserPlus size={20} />, label: 'Select Supervisor' },
      // Show Submit Proposal link if student has an approved supervisor and hasn't submitted a proposal yet
      ...(studentProfile?.approvalStatus === 'Approved' && !hasProposal ? [
        { path: '/student/submit-proposal', icon: <FiEdit size={20} />, label: 'Submit Proposal' }
      ] : []),
      // Show My Proposal link if student has submitted a proposal
      ...(hasProposal ? [
        { path: '/student/my-proposal', icon: <FiFileText size={20} />, label: 'My Proposal' }
      ] : [])
    ] : []),
    // Admin and Committee specific links
    ...(canViewLecturers ? [{ path: '/admin/lecturers', icon: <FiUsers size={20} />, label: 'Manage Lecturers' }] : []),
    // Committee-only link
    ...(isCommittee ? [
      { path: '/committee/approvals', icon: <FiFileText size={20} />, label: 'Supervisor Approvals' },
      { path: '/committee/proposals', icon: <FiFileText size={20} />, label: 'Manage Proposals' }
    ] : []),
    // Supervisor-only link
    ...(isSupervisor ? [
      { path: '/supervisor/my-students', icon: <FiUsers size={20} />, label: 'My Students' }
    ] : []),
    // Evaluator-only link
    ...(isEvaluator ? [
      { path: '/evaluator/my-assignments', icon: <FiFileText size={20} />, label: 'My Evaluation Assignments' }
    ] : []),
    // Admin-only links
    ...(isAdmin ? [
      { path: '/admin/programs', icon: <FiBook size={20} />, label: 'Academic Programs' },
      { path: '/admin/committee', icon: <FiUsers size={20} />, label: 'Manage Committee' }
    ] : []),
    { path: '/settings', icon: <FiSettings size={20} />, label: 'Settings' },
  ];

  return (
    <aside className="w-64 h-screen bg-white border-r border-gray-200 fixed left-0 top-0 flex flex-col">
      <div className="p-6">
        <h1 className="text-2xl font-bold text-gray-900 mb-8">FYP-Link</h1>
        
        {/* User Info */}
        <div className="mb-8 p-4 bg-gray-50 rounded-lg flex items-center">
          <FiUser size={20} className="text-gray-500 mr-3" />
          <div>
            <p className="text-sm text-gray-500">Logged in as:</p>
            {/* <p className="text-sm font-medium text-gray-900 truncate">{user?.email}</p> */}
            {isAdmin && (
              <p className="text-xs text-teal-600 mt-1">Administrator</p>
            )}
            {isStudent && (
              <p className="text-xs text-teal-600 mt-1">Student</p>
            )}
            {isSupervisor && (
              <p className="text-xs text-teal-600 mt-1">Supervisor</p>
            )}
          </div>
        </div>

        {/* Navigation Links */}
        <nav className="space-y-1">
          {navLinks.map((link) => (
            <Link
              key={link.path}
              to={link.path}
              className={`flex items-center px-4 py-2 text-sm rounded-lg transition-colors ${
                isActive(link.path)
                ? 'bg-teal-50 text-teal-600 font-semibold'
                : 'text-gray-600 hover:bg-gray-50 hover:text-gray-900'
              }`}
            >
              <span className="mr-3">{link.icon}</span>
              {link.label}
            </Link>
          ))}
        </nav>
      </div>

      {/* Logout Button */}
      <div className="mt-auto p-6 border-t border-gray-200">
        <button
          onClick={handleLogout}
          className="flex items-center w-full px-4 py-2 text-sm text-gray-600 rounded-lg hover:bg-gray-50 hover:text-gray-900 transition-colors"
        >
          <FiLogOut size={20} className="mr-3" />
          Logout
        </button>
      </div>
    </aside>
  );
}