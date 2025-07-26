import { Link } from 'react-router-dom';

export default function RegistrationSuccess() {
  return (
    <div className="min-h-screen flex items-center justify-center bg-gray-100">
      <div className="bg-white p-8 rounded-lg shadow-md w-full max-w-md text-center">
        <div className="mb-6">
          <h2 className="text-2xl font-bold text-gray-900 mb-4">
            Registration Successful!
          </h2>
          <p className="text-gray-600">
            A verification link has been sent to your email. Please check your inbox and click the link to activate your account before logging in.
          </p>
        </div>

        <div className="mt-6">
          <Link 
            to="/login" 
            className="text-blue-500 hover:text-blue-600 font-medium"
          >
            Return to Login
          </Link>
        </div>
      </div>
    </div>
  );
}