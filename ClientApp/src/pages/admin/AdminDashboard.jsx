import React, { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import { Card, Row, Col, Button, Spin, Alert } from 'antd';
import { 
  UserOutlined, 
  TeamOutlined, 
  FileTextOutlined, 
  ClockCircleOutlined,
  BookOutlined,
  UserSwitchOutlined,
  SettingOutlined
} from '@ant-design/icons';
import apiClient from '../../api/apiClient';
import toast from 'react-hot-toast';

const AdminDashboard = () => {
  const [stats, setStats] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    fetchDashboardStats();
  }, []);

  const fetchDashboardStats = async () => {
    try {
      setLoading(true);
      const response = await apiClient.get('/api/dashboard/admin-stats');
      setStats(response.data);
      setError(null);
    } catch (err) {
      console.error('Error fetching dashboard stats:', err);
      setError('Failed to load dashboard statistics');
      toast.error('Failed to load dashboard statistics');
    } finally {
      setLoading(false);
    }
  };

  const statCards = [
    {
      title: 'Total Students',
      value: stats?.totalStudents || 0,
      icon: <UserOutlined />,
      color: '#1890ff',
      bgColor: '#e6f7ff'
    },
    {
      title: 'Total Lecturers',
      value: stats?.totalLecturers || 0,
      icon: <TeamOutlined />,
      color: '#52c41a',
      bgColor: '#f6ffed'
    },
    {
      title: 'Total Proposals',
      value: stats?.totalProposals || 0,
      icon: <FileTextOutlined />,
      color: '#722ed1',
      bgColor: '#f9f0ff'
    },
    {
      title: 'Pending Approvals',
      value: stats?.pendingSupervisorApprovals || 0,
      icon: <ClockCircleOutlined />,
      color: '#fa8c16',
      bgColor: '#fff7e6'
    }
  ];

  const quickActions = [
    {
      title: 'Manage Academic Programs',
      description: 'Add, edit, or remove academic programs',
      icon: <BookOutlined />,
      link: '/admin/programs',
      color: '#1890ff'
    },
    {
      title: 'Manage Lecturers',
      description: 'Manage lecturer accounts and assignments',
      icon: <UserSwitchOutlined />,
      link: '/admin/lecturers',
      color: '#52c41a'
    },
    {
      title: 'Manage Committee',
      description: 'Configure committee members and settings',
      icon: <SettingOutlined />,
      link: '/admin/committee',
      color: '#722ed1'
    }
  ];

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
            <Button size="small" onClick={fetchDashboardStats}>
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
        <h1 className="text-3xl font-bold text-gray-800 mb-2">Admin Dashboard</h1>
        <p className="text-gray-600">System overview and quick access to management functions</p>
      </div>

      {/* Statistics Cards */}
      <div className="mb-8">
        <h2 className="text-xl font-semibold text-gray-700 mb-4">System Statistics</h2>
        <Row gutter={[16, 16]}>
          {statCards.map((card, index) => (
            <Col xs={24} sm={12} lg={6} key={index}>
              <Card 
                className="text-center shadow-sm hover:shadow-md transition-shadow"
                bodyStyle={{ padding: '24px' }}
              >
                <div 
                  className="w-12 h-12 rounded-full flex items-center justify-center mx-auto mb-3"
                  style={{ backgroundColor: card.bgColor, color: card.color }}
                >
                  {card.icon}
                </div>
                <div className="text-2xl font-bold text-gray-800 mb-1">
                  {card.value.toLocaleString()}
                </div>
                <div className="text-gray-600">{card.title}</div>
              </Card>
            </Col>
          ))}
        </Row>
      </div>

      {/* Quick Actions */}
      <div className="mb-8">
        <h2 className="text-xl font-semibold text-gray-700 mb-4">Quick Actions</h2>
        <Row gutter={[16, 16]}>
          {quickActions.map((action, index) => (
            <Col xs={24} md={8} key={index}>
              <Link to={action.link} className="block">
                <Card 
                  className="h-full shadow-sm hover:shadow-md transition-all duration-200 hover:scale-105 cursor-pointer"
                  bodyStyle={{ padding: '24px' }}
                >
                  <div className="text-center">
                    <div 
                      className="w-16 h-16 rounded-full flex items-center justify-center mx-auto mb-4"
                      style={{ backgroundColor: `${action.color}15`, color: action.color }}
                    >
                      {action.icon}
                    </div>
                    <h3 className="text-lg font-semibold text-gray-800 mb-2">
                      {action.title}
                    </h3>
                    <p className="text-gray-600 text-sm">
                      {action.description}
                    </p>
                  </div>
                </Card>
              </Link>
            </Col>
          ))}
        </Row>
      </div>

      {/* Additional Info */}
      <div className="bg-gray-50 rounded-lg p-6">
        <h3 className="text-lg font-semibold text-gray-800 mb-3">System Information</h3>
        <div className="grid grid-cols-1 md:grid-cols-2 gap-4 text-sm text-gray-600">
          <div>
            <p><strong>Last Updated:</strong> {new Date().toLocaleString()}</p>
            <p><strong>System Status:</strong> <span className="text-green-600">Operational</span></p>
          </div>
          <div>
            <p><strong>Total Users:</strong> {(stats?.totalStudents || 0) + (stats?.totalLecturers || 0)}</p>
            <p><strong>Active Proposals:</strong> {stats?.totalProposals || 0}</p>
          </div>
        </div>
      </div>
    </div>
  );
};

export default AdminDashboard; 