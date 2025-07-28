import React, { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import { Card, Row, Col, Button, Spin, Alert, Tag } from 'antd';
import { 
  UserOutlined, 
  TeamOutlined, 
  FileTextOutlined, 
  ClockCircleOutlined,
  CheckCircleOutlined,
  ExclamationCircleOutlined,
  ArrowRightOutlined
} from '@ant-design/icons';
import { useAuth } from '../../context/AuthContext';
import apiClient from '../../api/apiClient';
import toast from 'react-hot-toast';

const LecturerDashboard = () => {
  const { user } = useAuth();
  const [summary, setSummary] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    fetchLecturerSummary();
  }, []);

  const fetchLecturerSummary = async () => {
    try {
      setLoading(true);
      const response = await apiClient.get('/api/dashboard/lecturer-summary');
      setSummary(response.data);
      setError(null);
    } catch (err) {
      console.error('Error fetching lecturer summary:', err);
      setError('Failed to load dashboard data');
      toast.error('Failed to load dashboard data');
    } finally {
      setLoading(false);
    }
  };

  // Check user roles
  const isSupervisor = user?.roles?.includes('Supervisor');
  const isCommittee = user?.roles?.includes('Committee');
  const isEvaluator = user?.roles?.includes('Evaluator');

  // Helper function to get role display name
  const getRoleDisplayName = () => {
    const roles = [];
    if (isSupervisor) roles.push('Supervisor');
    if (isCommittee) roles.push('Committee');
    if (isEvaluator) roles.push('Evaluator');
    
    if (roles.length === 0) return 'Lecturer';
    if (roles.length === 1) return roles[0];
    return roles.join(', ');
  };

  // Helper function to get status color
  const getStatusColor = (count) => {
    if (count === 0) return 'green';
    if (count <= 3) return 'orange';
    return 'red';
  };

  if (loading) {
    return (
      <div className="flex justify-center items-center h-64">
        <Spin size="large" />
      </div>
    );
  }

  if (error) {
    return (
      <div className="container mx-auto px-4 py-8">
        <Alert
          message="Error"
          description={error}
          type="error"
          showIcon
          action={
            <Button size="small" onClick={fetchLecturerSummary}>
              Retry
            </Button>
          }
        />
      </div>
    );
  }

  return (
    <div className="container mx-auto px-4 py-8">
      <div className="mb-8">
        <h1 className="text-3xl font-bold text-gray-800 mb-2">Lecturer Dashboard</h1>
        <p className="text-gray-600">
          Welcome back! You are logged in as a <Tag color="blue">{getRoleDisplayName()}</Tag>
        </p>
      </div>

      <Row gutter={[16, 16]}>
        {/* Supervisor Widget */}
        {isSupervisor && (
          <Col xs={24} md={8}>
            <Card 
              title={
                <div className="flex items-center">
                  <TeamOutlined className="mr-2 text-blue-600" />
                  My Supervised Students
                </div>
              }
              className="h-full shadow-sm hover:shadow-md transition-shadow"
            >
              <div className="text-center">
                <div className="text-3xl font-bold text-gray-800 mb-2">
                  {summary?.supervisedStudentsCount || 0}
                </div>
                <p className="text-gray-600 mb-4">Active Students</p>
                
                {summary?.supervisedStudentsCount > 0 ? (
                  <div className="mb-4">
                    <Tag color={getStatusColor(summary.supervisedStudentsCount)} className="mb-2">
                      {summary.supervisedStudentsCount === 1 ? '1 Student' : `${summary.supervisedStudentsCount} Students`}
                    </Tag>
                    <p className="text-sm text-gray-500">
                      You are currently supervising {summary.supervisedStudentsCount} student{summary.supervisedStudentsCount !== 1 ? 's' : ''}.
                    </p>
                  </div>
                ) : (
                  <div className="mb-4">
                    <div className="text-gray-400 mb-2">
                      <UserOutlined style={{ fontSize: '32px' }} />
                    </div>
                    <p className="text-sm text-gray-500">No students assigned yet.</p>
                  </div>
                )}
                
                <Link to="/supervisor/my-students">
                  <button
                    type="submit"
                    className="w-full mt-2 px-4 py-2 bg-blue-600 text-white rounded-md hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:ring-offset-2 disabled:opacity-50 disabled:cursor-not-allowed"
                  >
                    View My Students
                  </button>
                </Link>
              </div>
            </Card>
          </Col>
        )}

        {/* Committee Widget */}
        {isCommittee && (
          <Col xs={24} md={8}>
            <Card 
              title={
                <div className="flex items-center">
                  <CheckCircleOutlined className="mr-2 text-green-600" />
                  Committee Tasks
                </div>
              }
              className="h-full shadow-sm hover:shadow-md transition-shadow"
            >
              <div className="text-center">
                <div className="text-3xl font-bold text-gray-800 mb-2">
                  {summary?.pendingSupervisorRequestsCount || 0}
                </div>
                <p className="text-gray-600 mb-4">Pending Approvals</p>
                
                {summary?.pendingSupervisorRequestsCount > 0 ? (
                  <div className="mb-4">
                    <Tag color={getStatusColor(summary.pendingSupervisorRequestsCount)} className="mb-2">
                      {summary.pendingSupervisorRequestsCount === 1 ? '1 Request' : `${summary.pendingSupervisorRequestsCount} Requests`}
                    </Tag>
                    <p className="text-sm text-gray-500">
                      {summary.pendingSupervisorRequestsCount} supervisor request{summary.pendingSupervisorRequestsCount !== 1 ? 's' : ''} awaiting approval.
                    </p>
                  </div>
                ) : (
                  <div className="mb-4">
                    <div className="text-gray-400 mb-2">
                      <CheckCircleOutlined style={{ fontSize: '32px' }} />
                    </div>
                    <p className="text-sm text-gray-500">No pending approvals.</p>
                  </div>
                )}
                
                <Link to="/committee/approvals">
                  <button
                    type="submit"
                    className="w-full px-4 py-2 bg-green-600 text-white rounded-md hover:bg-green-700 focus:outline-none focus:ring-2 focus:ring-green-500 focus:ring-offset-2 disabled:opacity-50 disabled:cursor-not-allowed"
                  >
                    Review Approvals
                  </button>
                </Link>
              </div>
            </Card>
          </Col>
        )}

        {/* Evaluator Widget */}
        {isEvaluator && (
          <Col xs={24} md={8}>
            <Card 
              title={
                <div className="flex items-center">
                  <FileTextOutlined className="mr-2 text-purple-600" />
                  Evaluation Assignments
                </div>
              }
              className="h-full shadow-sm hover:shadow-md transition-shadow"
            >
              <div className="text-center">
                <div className="text-3xl font-bold text-gray-800 mb-2">
                  {summary?.pendingEvaluationAssignmentsCount || 0}
                </div>
                <p className="text-gray-600 mb-4">Pending Evaluations</p>
                
                {summary?.pendingEvaluationAssignmentsCount > 0 ? (
                  <div className="mb-4">
                    <Tag color={getStatusColor(summary.pendingEvaluationAssignmentsCount)} className="mb-2">
                      {summary.pendingEvaluationAssignmentsCount === 1 ? '1 Assignment' : `${summary.pendingEvaluationAssignmentsCount} Assignments`}
                    </Tag>
                    <p className="text-sm text-gray-500">
                      {summary.pendingEvaluationAssignmentsCount} proposal{summary.pendingEvaluationAssignmentsCount !== 1 ? 's' : ''} awaiting your evaluation.
                    </p>
                  </div>
                ) : (
                  <div className="mb-4">
                    <div className="text-gray-400 mb-2">
                      <FileTextOutlined style={{ fontSize: '32px' }} />
                    </div>
                    <p className="text-sm text-gray-500">No pending evaluations.</p>
                  </div>
                )}
                
                <Link to="/evaluator/my-assignments">
                  <button
                    type="submit"
                    className="w-full px-4 py-2 bg-purple-600 text-white rounded-md hover:bg-purple-700 focus:outline-none focus:ring-2 focus:ring-purple-500 focus:ring-offset-2 disabled:opacity-50 disabled:cursor-not-allowed"
                  >
                    View Assignments
                  </button>
                </Link>
              </div>
            </Card>
          </Col>
        )}
      </Row>

      {/* Quick Actions Section */}
      <div className="mt-8">
        <Card title="Quick Actions" className="shadow-sm">
          <Row gutter={[16, 16]}>
            {isSupervisor && (
              <Col xs={24} sm={12} md={8}>
                <Link to="/supervisor/my-students" className="block">
                  <div className="p-4 border border-gray-200 rounded-lg hover:border-blue-300 hover:bg-blue-50 transition-colors">
                    <div className="flex items-center">
                      <TeamOutlined className="text-blue-600 mr-3" />
                      <div>
                        <h3 className="font-medium text-gray-800">Manage Students</h3>
                        <p className="text-sm text-gray-600">View and manage your supervised students</p>
                      </div>
                      <ArrowRightOutlined className="ml-auto text-gray-400" />
                    </div>
                  </div>
                </Link>
              </Col>
            )}
            
            {isCommittee && (
              <Col xs={24} sm={12} md={8}>
                <Link to="/committee/approvals" className="block">
                  <div className="p-4 border border-gray-200 rounded-lg hover:border-green-300 hover:bg-green-50 transition-colors">
                    <div className="flex items-center">
                      <CheckCircleOutlined className="text-green-600 mr-3" />
                      <div>
                        <h3 className="font-medium text-gray-800">Review Approvals</h3>
                        <p className="text-sm text-gray-600">Approve or reject supervisor requests</p>
                      </div>
                      <ArrowRightOutlined className="ml-auto text-gray-400" />
                    </div>
                  </div>
                </Link>
              </Col>
            )}
            
            {isEvaluator && (
              <Col xs={24} sm={12} md={8}>
                <Link to="/evaluator/my-assignments" className="block">
                  <div className="p-4 border border-gray-200 rounded-lg hover:border-purple-300 hover:bg-purple-50 transition-colors">
                    <div className="flex items-center">
                      <FileTextOutlined className="text-purple-600 mr-3" />
                      <div>
                        <h3 className="font-medium text-gray-800">Evaluate Proposals</h3>
                        <p className="text-sm text-gray-600">Review and evaluate assigned proposals</p>
                      </div>
                      <ArrowRightOutlined className="ml-auto text-gray-400" />
                    </div>
                  </div>
                </Link>
              </Col>
            )}
          </Row>
        </Card>
      </div>

      {/* System Information */}
      <div className="mt-8">
        <Card title="System Information" className="shadow-sm">
          <div className="grid grid-cols-1 md:grid-cols-3 gap-4 text-sm text-gray-600">
            <div>
              <p><strong>Last Updated:</strong> {new Date().toLocaleString()}</p>
              <p><strong>System Status:</strong> <span className="text-green-600">Operational</span></p>
            </div>
            <div>
              <p><strong>Your Roles:</strong> {getRoleDisplayName()}</p>
              <p><strong>Active Students:</strong> {summary?.supervisedStudentsCount || 0}</p>
            </div>
            <div>
              <p><strong>Pending Tasks:</strong> {
                (summary?.pendingSupervisorRequestsCount || 0) + 
                (summary?.pendingEvaluationAssignmentsCount || 0)
              }</p>
              <p><strong>Total Assignments:</strong> {summary?.pendingEvaluationAssignmentsCount || 0}</p>
            </div>
          </div>
        </Card>
      </div>
    </div>
  );
};

export default LecturerDashboard; 