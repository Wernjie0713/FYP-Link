import { useState, useEffect } from 'react';
import { useNavigate, useSearchParams, Link } from 'react-router-dom';
import api from '../../utils/axios';

export default function ConfirmEmail() {
  const [searchParams] = useSearchParams();
  const navigate = useNavigate();
  const [status, setStatus] = useState('verifying'); // verifying, success, error
  const [error, setError] = useState('');

  useEffect(() => {
    const confirmEmail = async () => {
      const userId = searchParams.get('userId');
      const token = searchParams.get('token');

      if (!userId || !token) {
        setStatus('error');
        setError('Invalid verification link');
        return;
      }

      try {
        const response = await api.get('/api/auth/confirm-email', {
          params: {
            userId: userId,
            token: token
          }
        });
        setStatus('success');
      } catch (err) {
        console.error('Error confirming email:', err);
        setStatus('error');
        if (err.response?.data?.message) {
          setError(err.response.data.message);
        } else {
          setError('Failed to verify email address');
        }
      }
    };

    confirmEmail();
  }, [searchParams]);

  return (
    <div className="min-h-screen flex items-center justify-center bg-gray-100">
      <div className="bg-white p-8 rounded-lg shadow-md w-full max-w-md">
        {status === 'verifying' && (
          <div className="text-center">
            <h2 className="text-2xl font-bold text-gray-900 mb-4">Verifying Email</h2>
            <p className="text-gray-600">Please wait while we verify your email address...</p>
          </div>
        )}

        {status === 'success' && (
          <div className="text-center">
            <h2 className="text-2xl font-bold text-gray-900 mb-4">Email Verified!</h2>
            <p className="text-gray-600 mb-6">
              Your email has been successfully verified. You can now log in to your account.
            </p>
            <Link
              to="/login"
              className="inline-block w-full px-4 py-2 bg-teal-600 text-white rounded-md hover:bg-teal-700 text-center"
            >
              Go to Login
            </Link>
          </div>
        )}

        {status === 'error' && (
          <div className="text-center">
            <h2 className="text-2xl font-bold text-gray-900 mb-4">Verification Failed</h2>
            <p className="text-red-600 mb-6">{error}</p>
            <Link
              to="/register"
              className="inline-block w-full px-4 py-2 bg-teal-600 text-white rounded-md hover:bg-teal-700 text-center"
            >
              Back to Registration
            </Link>
          </div>
        )}
      </div>
    </div>
  );
} 