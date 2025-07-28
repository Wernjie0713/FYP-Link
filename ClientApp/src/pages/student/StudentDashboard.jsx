import React, { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import { Card, Row, Col, Button, Spin, Alert, Tag, Progress } from 'antd';
import { 
  UserOutlined, 
  FileTextOutlined, 
  CheckCircleOutlined,
  ClockCircleOutlined,
  ExclamationCircleOutlined,
  ArrowRightOutlined,
  BookOutlined,
  TeamOutlined
} from '@ant-design/icons';
import apiClient from '../../api/apiClient';
import toast from 'react-hot-toast';

const StudentDashboard = () => {
  const [profile, setProfile] = useState(null);
  const [proposal, setProposal] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    fetchStudentData();
  }, []);

  const fetchStudentData = async () => {
    try {
      setLoading(true);
      
      // Fetch student profile
      const profileResponse = await apiClient.get('/api/students/my-profile');
      setProfile(profileResponse.data);
      
      // Fetch proposal if it exists
      if (profileResponse.data.hasProposal) {
        try {
          const proposalResponse = await apiClient.get('/api/proposals/my-proposal');
          setProposal(proposalResponse.data);
        } catch (proposalError) {
          console.error('Error fetching proposal:', proposalError);
          // Don't set error here as proposal might not exist
        }
      }
      
      setError(null);
    } catch (err) {
      console.error('Error fetching student data:', err);
      setError('Failed to load dashboard data');
      toast.error('Failed to load dashboard data');
    } finally {
      setLoading(false);
    }
  };

  // Helper function to get status color
  const getStatusColor = (status) => {
    switch (status) {
      case 'Approved':
        return 'green';
      case 'Pending':
        return 'orange';
      case 'Rejected':
        return 'red';
      default:
        return 'blue';
    }
  };

  // Helper function to get proposal status
  const getProposalStatus = () => {
    if (!proposal) return null;
    
    const currentEvaluations = proposal.evaluations.filter(e => e.isCurrent);
    
    if (!currentEvaluations || currentEvaluations.length === 0) return 'Pending Review';
    
    if (currentEvaluations.some(e => !e.result)) return 'Under Evaluation';
    if (currentEvaluations.some(e => e.result === 'Rejected')) return 'Rejected';
    if (currentEvaluations.some(e => e.result === 'Accepted with Conditions')) return 'Accepted with Conditions';
    if (currentEvaluations.every(e => e.result === 'Accepted')) return 'Accepted';
    
    return 'Pending Review';
  };

  // Helper function to get progress percentage
  const getProgressPercentage = () => {
    if (!profile) return 0;
    
    let progress = 0;
    
    // Profile completed
    progress += 25;
    
    // Supervisor selected
    if (profile.supervisorId) progress += 25;
    
    // Supervisor approved
    if (profile.approvalStatus === 'Approved') progress += 25;
    
    // Proposal submitted
    if (profile.hasProposal) progress += 25;
    
    return progress;
  };

  // Helper function to get next step guidance
  const getNextStepGuidance = () => {
    if (!profile) return 'Please complete your profile setup.';
    
    if (!profile.supervisorId) {
      return 'You need to select a supervisor to proceed with your FYP.';
    }
    
    if (profile.approvalStatus === 'Pending') {
      return 'Your supervisor selection is pending approval. You will be notified once approved.';
    }
    
    if (profile.approvalStatus === 'Rejected') {
      return 'Your supervisor selection was rejected. Please select a different supervisor.';
    }
    
    if (profile.approvalStatus === 'Approved' && !profile.hasProposal) {
      return 'Your supervisor is approved! The next step is to submit your proposal.';
    }
    
    if (profile.hasProposal) {
      const proposalStatus = getProposalStatus();
      switch (proposalStatus) {
        case 'Under Evaluation':
          return 'Your proposal is currently being evaluated by the committee.';
        case 'Accepted':
          return 'Congratulations! Your proposal has been accepted. You can now proceed with your project.';
        case 'Accepted with Conditions':
          return 'Your proposal has been accepted with conditions. Please review the feedback and make necessary adjustments.';
        case 'Rejected':
          return 'Your proposal was not accepted. Please review the feedback and resubmit.';
        default:
          return 'Your proposal is pending review.';
      }
    }
    
    return 'Please complete your profile setup.';
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
            <Button size="small" onClick={fetchStudentData}>
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
        <h1 className="text-3xl font-bold text-gray-800 mb-2">Student Dashboard</h1>
        <p className="text-gray-600">Welcome back, {profile?.name}! Here's your FYP progress overview.</p>
      </div>

      {/* Progress Overview */}
      <div className="mb-8">
        <Card className="shadow-sm">
          <div className="mb-4">
            <h2 className="text-lg font-semibold text-gray-700 mb-2">Your Progress</h2>
            <Progress 
              percent={getProgressPercentage()} 
              status="active"
              strokeColor={{
                '0%': '#108ee9',
                '100%': '#87d068',
              }}
            />
          </div>
          <div className="grid grid-cols-2 md:grid-cols-4 gap-4 text-sm text-gray-600">
            <div className="text-center">
              <div className="w-8 h-8 bg-blue-100 rounded-full flex items-center justify-center mx-auto mb-1">
                <UserOutlined className="text-blue-600" />
              </div>
              <p>Profile</p>
            </div>
            <div className="text-center">
              <div className={`w-8 h-8 rounded-full flex items-center justify-center mx-auto mb-1 ${
                profile?.supervisorId ? 'bg-green-100' : 'bg-gray-100'
              }`}>
                <TeamOutlined className={profile?.supervisorId ? 'text-green-600' : 'text-gray-400'} />
              </div>
              <p>Supervisor</p>
            </div>
            <div className="text-center">
              <div className={`w-8 h-8 rounded-full flex items-center justify-center mx-auto mb-1 ${
                profile?.approvalStatus === 'Approved' ? 'bg-green-100' : 'bg-gray-100'
              }`}>
                <CheckCircleOutlined className={profile?.approvalStatus === 'Approved' ? 'text-green-600' : 'text-gray-400'} />
              </div>
              <p>Approval</p>
            </div>
            <div className="text-center">
              <div className={`w-8 h-8 rounded-full flex items-center justify-center mx-auto mb-1 ${
                profile?.hasProposal ? 'bg-green-100' : 'bg-gray-100'
              }`}>
                <FileTextOutlined className={profile?.hasProposal ? 'text-green-600' : 'text-gray-400'} />
              </div>
              <p>Proposal</p>
            </div>
          </div>
        </Card>
      </div>

      <Row gutter={[16, 16]}>
        {/* Supervisor Status Panel */}
        <Col xs={24} lg={12}>
          <Card 
            title={
              <div className="flex items-center">
                <TeamOutlined className="mr-2 text-blue-600" />
                Supervisor Status
              </div>
            }
            className="h-full shadow-sm"
          >
            {profile?.supervisorId ? (
              <div>
                <div className="mb-4">
                  <Tag color={getStatusColor(profile.approvalStatus)} className="mb-2">
                    {profile.approvalStatus}
                  </Tag>
                  <p className="text-gray-600">
                    Supervisor: <span className="font-medium">{profile.supervisorName || 'Not assigned'}</span>
                  </p>
                </div>
                
                {profile.approvalStatus === 'Pending' && (
                  <div className="bg-yellow-50 p-3 rounded-md mb-4">
                    <div className="flex items-center">
                      <ClockCircleOutlined className="text-yellow-600 mr-2" />
                      <span className="text-yellow-800">Awaiting supervisor approval</span>
                    </div>
                  </div>
                )}
                
                {profile.approvalStatus === 'Rejected' && (
                  <div className="bg-red-50 p-3 rounded-md mb-4">
                    <div className="flex items-center">
                      <ExclamationCircleOutlined className="text-red-600 mr-2" />
                      <span className="text-red-800">Supervisor selection was rejected</span>
                    </div>
                  </div>
                )}
                
                {profile.approvalStatus === 'Rejected' && (
                  <Link to="/student/select-supervisor">
                    <button
                      type="submit"
                      className="w-1/8 px-4 py-2 bg-teal-600 text-white rounded-md hover:bg-teal-700 focus:outline-none focus:ring-2 focus:ring-teal-500 focus:ring-offset-2 disabled:opacity-50 disabled:cursor-not-allowed"
                      >
                      Select New Supervisor
                  </button>
                  </Link>
                )}
              </div>
            ) : (
              <div className="text-center">
                <div className="text-gray-400 mb-4">
                  <TeamOutlined style={{ fontSize: '48px' }} />
                </div>
                <p className="text-gray-600 mb-4">You haven't selected a supervisor yet.</p>
                <Link to="/student/select-supervisor">
                  <button
                      type="submit"
                      className="w-1/8 px-4 py-2 bg-teal-600 text-white rounded-md hover:bg-teal-700 focus:outline-none focus:ring-2 focus:ring-teal-500 focus:ring-offset-2 disabled:opacity-50 disabled:cursor-not-allowed"
                      >
                      Select Supervisor
                  </button>
                </Link>
              </div>
            )}
          </Card>
        </Col>

        {/* Proposal Status Panel */}
        <Col xs={24} lg={12}>
          <Card 
            title={
              <div className="flex items-center">
                <FileTextOutlined className="mr-2 text-purple-600" />
                Proposal Status
              </div>
            }
            className="h-full shadow-sm"
          >
            {profile?.hasProposal && proposal ? (
              <div>
                <div className="mb-4">
                  <h3 className="font-medium text-gray-800 mb-2">{proposal.title}</h3>
                  <Tag color={getStatusColor(getProposalStatus())} className="mb-2">
                    {getProposalStatus()}
                  </Tag>
                  <p className="text-sm text-gray-600">
                    Submitted: {new Date(proposal.createdAt || Date.now()).toLocaleDateString()}
                  </p>
                </div>
                
                {proposal.supervisorComment && (
                  <div className="bg-blue-50 p-3 rounded-md mb-4">
                    <p className="text-sm text-blue-800">
                      <strong>Supervisor Comment:</strong> {proposal.supervisorComment}
                    </p>
                  </div>
                )}
                
                <Link to="/student/my-proposal">
                  <button
                      type="submit"
                      className="w-full px-4 py-2 bg-teal-600 text-white rounded-md hover:bg-teal-700 focus:outline-none focus:ring-2 focus:ring-teal-500 focus:ring-offset-2 disabled:opacity-50 disabled:cursor-not-allowed"
                      >
                      View Full Proposal
                  </button>
                </Link>
              </div>
            ) : profile?.approvalStatus === 'Approved' ? (
              <div className="text-center">
                <div className="text-gray-400 mb-4">
                  <FileTextOutlined style={{ fontSize: '48px' }} />
                </div>
                <p className="text-gray-600 mb-4">You can now submit your proposal.</p>
                <Link to="/student/submit-proposal">
                  <Button type="primary" className="w-full">
                    Submit Proposal
                  </Button>
                </Link>
              </div>
            ) : (
              <div className="text-center">
                <div className="text-gray-400 mb-4">
                  <FileTextOutlined style={{ fontSize: '48px' }} />
                </div>
                <p className="text-gray-600 mb-4">Complete supervisor approval first.</p>
                <Button disabled className="w-full">
                  Submit Proposal
                </Button>
              </div>
            )}
          </Card>
        </Col>

        {/* What's Next Panel */}
        <Col xs={24}>
          <Card 
            title={
              <div className="flex items-center">
                <ArrowRightOutlined className="mr-2 text-green-600" />
                What's Next?
              </div>
            }
            className="shadow-sm"
          >
            <div className="bg-green-50 p-4 rounded-md">
              <p className="text-green-800 font-medium">{getNextStepGuidance()}</p>
            </div>
          </Card>
        </Col>
      </Row>
    </div>
  );
};

export default StudentDashboard; 