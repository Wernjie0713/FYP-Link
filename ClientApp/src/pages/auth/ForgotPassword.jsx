import React, { useState } from 'react';
import { Link } from 'react-router-dom';
import { Card, Form, Input, Button, Alert, Spin } from 'antd';
import { MailOutlined, ArrowLeftOutlined } from '@ant-design/icons';
import apiClient from '../../api/apiClient';
import toast from 'react-hot-toast';

const ForgotPassword = () => {
  const [loading, setLoading] = useState(false);
  const [success, setSuccess] = useState(false);
  const [error, setError] = useState(null);

  const onFinish = async (values) => {
    setLoading(true);
    setError(null);
    
    try {
      const response = await apiClient.post('/api/auth/forgot-password', {
        email: values.email
      });
      
      setSuccess(true);
      toast.success('Password reset link sent successfully');
    } catch (err) {
      console.error('Error requesting password reset:', err);
      setError('Failed to send password reset link. Please try again.');
      toast.error('Failed to send password reset link');
    } finally {
      setLoading(false);
    }
  };

  if (success) {
    return (
      <div className="min-h-screen bg-slate-50 flex items-center justify-center py-12 px-4 sm:px-6 lg:px-8">
        <div className="max-w-md w-full space-y-8">
          <Card className="shadow-lg">
            <div className="text-center">
              <div className="mx-auto flex items-center justify-center h-12 w-12 rounded-full bg-green-100 mb-4">
                <MailOutlined className="h-6 w-6 text-green-600" />
              </div>
              <h2 className="text-2xl font-bold text-gray-900 mb-2">Check Your Email</h2>
              <p className="text-gray-600 mb-6">
                If an account with that email exists, a password reset link has been sent.
              </p>
              <p className="text-sm text-gray-500 mb-6">
                Please check your email inbox and click the reset link to set a new password.
              </p>
              <Link to="/login">
                <button
                type="submit"
                className="w-full bg-teal-600 text-white font-bold py-2 px-4 rounded hover:bg-teal-700 focus:outline-none focus:shadow-outline disabled:opacity-50 disabled:cursor-not-allowed"
            >
                    {'Return to Login'}
                </button>
              </Link>
            </div>
          </Card>
        </div>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-slate-50 flex items-center justify-center py-12 px-4 sm:px-6 lg:px-8">
      <div className="max-w-md w-full space-y-8">
        <div className="text-center">
          <h2 className="text-3xl font-bold text-gray-900">Forgot Password</h2>
          <p className="mt-2 text-gray-600">
            Enter your email address and we'll send you a link to reset your password.
          </p>
        </div>

        <Card className="shadow-lg">
          <Form
            name="forgot-password"
            onFinish={onFinish}
            layout="vertical"
            size="large"
          >
            {error && (
              <Alert
                message="Error"
                description={error}
                type="error"
                showIcon
                className="mb-4"
              />
            )}

            <Form.Item
              name="email"
              label="Email Address"
              rules={[
                {
                  required: true,
                  message: 'Please enter your email address',
                },
                {
                  type: 'email',
                  message: 'Please enter a valid email address',
                },
              ]}
            >
              <Input
                prefix={<MailOutlined className="text-gray-400" />}
                placeholder="Enter your email address"
                disabled={loading}
              />
            </Form.Item>

            <Form.Item>
                <button
                    type="submit"
                    disabled={loading}
                    className="w-full bg-teal-600 text-white font-bold py-2 px-4 rounded hover:bg-teal-700 focus:outline-none focus:shadow-outline disabled:opacity-50 disabled:cursor-not-allowed"
                >
                    {loading ? 'Sending...' : 'Send Reset Link'}
                </button>
            </Form.Item>

            <div className="text-center">
              <Link 
                to="/login" 
                className="flex items-center justify-center text-sm text-gray-600 hover:text-gray-900"
              >
                <ArrowLeftOutlined className="mr-1" />
                Back to Login
              </Link>
            </div>
          </Form>
        </Card>
      </div>
    </div>
  );
};

export default ForgotPassword; 