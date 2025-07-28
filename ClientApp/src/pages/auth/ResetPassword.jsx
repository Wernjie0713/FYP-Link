import React, { useState, useEffect } from 'react';
import { useNavigate, useSearchParams } from 'react-router-dom';
import { Card, Form, Input, Button, Alert, Spin } from 'antd';
import { LockOutlined, EyeInvisibleOutlined, EyeTwoTone, CheckCircleOutlined } from '@ant-design/icons';
import apiClient from '../../api/apiClient';
import toast from 'react-hot-toast';

const ResetPassword = () => {
  const [searchParams] = useSearchParams();
  const navigate = useNavigate();
  const [loading, setLoading] = useState(false);
  const [success, setSuccess] = useState(false);
  const [error, setError] = useState(null);
  const [form] = Form.useForm();

  const userId = searchParams.get('userId');
  const encodedToken = searchParams.get('token');
  const token = encodedToken ? decodeURIComponent(encodedToken) : null;

  useEffect(() => {
    // Check if required parameters are present
    if (!userId || !encodedToken || !token) {
      setError('Invalid reset link. Please request a new password reset.');
    }
  }, [userId, encodedToken, token]);

  const onFinish = async (values) => {
    if (!userId || !encodedToken || !token) {
      setError('Invalid reset link. Please request a new password reset.');
      return;
    }

    setLoading(true);
    setError(null);
    
    try {
      const response = await apiClient.post('/api/auth/reset-password', {
        userId: userId,
        token: token,
        newPassword: values.newPassword
      });
      
      setSuccess(true);
      toast.success('Password reset successfully');
      
      // Navigate to login after a short delay
      setTimeout(() => {
        navigate('/login');
      }, 3000);
    } catch (err) {
      console.error('Error resetting password:', err);
      const errorMessage = err.response?.data?.message || 'Failed to reset password. Please try again.';
      setError(errorMessage);
      toast.error('Failed to reset password');
    } finally {
      setLoading(false);
    }
  };

  const validatePassword = (_, value) => {
    if (!value) {
      return Promise.reject(new Error('Please enter your new password'));
    }
    
    if (value.length < 6) {
      return Promise.reject(new Error('Password must be at least 6 characters long'));
    }
    
    return Promise.resolve();
  };

  const validateConfirmPassword = (_, value) => {
    if (!value) {
      return Promise.reject(new Error('Please confirm your new password'));
    }
    
    const newPassword = form.getFieldValue('newPassword');
    if (value !== newPassword) {
      return Promise.reject(new Error('Passwords do not match'));
    }
    
    return Promise.resolve();
  };

  if (success) {
    return (
      <div className="min-h-screen bg-slate-50 flex items-center justify-center py-12 px-4 sm:px-6 lg:px-8">
        <div className="max-w-md w-full space-y-8">
          <Card className="shadow-lg">
            <div className="text-center">
              <div className="mx-auto flex items-center justify-center h-12 w-12 rounded-full bg-green-100 mb-4">
                <CheckCircleOutlined className="h-6 w-6 text-green-600" />
              </div>
              <h2 className="text-2xl font-bold text-gray-900 mb-2">Password Reset Successful</h2>
              <p className="text-gray-600 mb-6">
                Your password has been reset successfully. You can now log in with your new password.
              </p>
              <p className="text-sm text-gray-500 mb-6">
                Redirecting to login page...
              </p>
              <Button 
                type="primary" 
                className="w-full"
                onClick={() => navigate('/login')}
              >
                Go to Login
              </Button>
            </div>
          </Card>
        </div>
      </div>
    );
  }

  if (!userId || !encodedToken || !token) {
    return (
      <div className="min-h-screen bg-slate-50 flex items-center justify-center py-12 px-4 sm:px-6 lg:px-8">
        <div className="max-w-md w-full space-y-8">
          <Card className="shadow-lg">
            <div className="text-center">
              <h2 className="text-2xl font-bold text-gray-900 mb-2">Invalid Reset Link</h2>
              <p className="text-gray-600 mb-6">
                The password reset link is invalid or has expired. Please request a new password reset.
              </p>
              <button
                type="submit"
                onClick={() => navigate('/forgot-password')}
                className="w-full bg-teal-600 text-white font-bold py-2 px-4 rounded hover:bg-teal-700 focus:outline-none focus:shadow-outline disabled:opacity-50 disabled:cursor-not-allowed"
            >
                Request New Reset Link
            </button>
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
          <h2 className="text-3xl font-bold text-gray-900">Reset Password</h2>
          <p className="mt-2 text-gray-600">
            Enter your new password below.
          </p>
        </div>

        <Card className="shadow-lg">
          <Form
            form={form}
            name="reset-password"
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
              name="newPassword"
              label="New Password"
              rules={[
                {
                  validator: validatePassword,
                },
              ]}
            >
              <Input.Password
                prefix={<LockOutlined className="text-gray-400" />}
                placeholder="Enter your new password"
                disabled={loading}
                iconRender={(visible) => (visible ? <EyeTwoTone /> : <EyeInvisibleOutlined />)}
              />
            </Form.Item>

            <Form.Item
              name="confirmPassword"
              label="Confirm New Password"
              rules={[
                {
                  validator: validateConfirmPassword,
                },
              ]}
            >
              <Input.Password
                prefix={<LockOutlined className="text-gray-400" />}
                placeholder="Confirm your new password"
                disabled={loading}
                iconRender={(visible) => (visible ? <EyeTwoTone /> : <EyeInvisibleOutlined />)}
              />
            </Form.Item>

            <Form.Item>
              <button
                    type="submit"
                    loading={loading}
                    disabled={loading}
                    className="w-full bg-teal-600 text-white font-bold py-2 px-4 rounded hover:bg-teal-700 focus:outline-none focus:shadow-outline disabled:opacity-50 disabled:cursor-not-allowed"
                >
                    {loading ? 'Resetting...' : 'Reset Password'}
                </button>
            </Form.Item>
          </Form>
        </Card>
      </div>
    </div>
  );
};

export default ResetPassword; 