import React, { useState, useEffect } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { toast } from 'react-toastify';
import apiClient from '../../api/apiClient';

const MyProposal = () => {
  const [loading, setLoading] = useState(true);
  const [proposal, setProposal] = useState(null);
  const navigate = useNavigate();

  useEffect(() => {
    const fetchProposal = async () => {
      try {
        const response = await apiClient.get('/api/proposals/my-proposal');
        setProposal(response.data);
        setLoading(false);
      } catch (error) {
        console.error('Error fetching proposal:', error);
        toast.error('Failed to load proposal data');
        setLoading(false);
      }
    };

    fetchProposal();
  }, []);

  if (loading) {
    return <div className="flex justify-center items-center h-full">Loading...</div>;
  }

  if (!proposal) {
    return (
      <div className="container mx-auto px-4 py-8">
        <div className="bg-white rounded-lg shadow-md p-6 text-center">
          <h1 className="text-2xl font-bold mb-4">No Proposal Found</h1>
          <p className="text-gray-600 mb-6">You haven't submitted a project proposal yet.</p>
          <Link 
            to="/student/submit-proposal" 
            className="px-4 py-2 bg-indigo-600 text-white rounded-md hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500"
          >
            Submit a Proposal
          </Link>
        </div>
      </div>
    );
  }

  // 1. Get the base URL from the environment variable
  const API_URL = import.meta.env.VITE_API_BASE_URL;

  // 2. Construct the full URL for the PDF using the relative path from your database
  const pdfUrl = `${API_URL}${proposal.pdfFilePath}`;

  // Helper function to determine overall status based on current evaluations
  const getOverallStatus = () => {
    const currentEvaluations = proposal.evaluations.filter(e => e.isCurrent);
    
    if (!currentEvaluations || currentEvaluations.length === 0) return 'Pending';
    
    if (currentEvaluations.some(e => !e.result)) return 'Pending Review';
    if (currentEvaluations.some(e => e.result === 'Rejected')) return 'Rejected';
    if (currentEvaluations.some(e => e.result === 'Accepted with Conditions')) return 'Accepted with Conditions';
    if (currentEvaluations.every(e => e.result === 'Accepted')) return 'Accepted';
    return 'Pending';
  };

  // Separate current and historical evaluations
  const currentEvaluations = proposal.evaluations.filter(e => e.isCurrent);
  const historicalEvaluations = proposal.evaluations.filter(e => !e.isCurrent);

  // Helper function to get status color
  const getStatusColor = (status) => {
    switch (status) {
      case 'Accepted':
        return 'bg-green-100 text-green-800';
      case 'Accepted with Conditions':
        return 'bg-yellow-100 text-yellow-800';
      case 'Rejected':
        return 'bg-red-100 text-red-800';
      default:
        return 'bg-blue-100 text-blue-800';
    }
  };

  return (
    <div className="container mx-auto px-4 py-8">
      <h1 className="text-2xl font-bold mb-6">My Project Proposal</h1>
      
      <div className="bg-white rounded-lg shadow-md p-6">
        <div className="mb-6 pb-4 border-b border-gray-200">
          <div className="flex justify-between items-start">
            <div>
              <h2 className="text-xl font-semibold text-gray-800">{proposal.title}</h2>
              <p className="text-sm text-gray-500 mt-1">Project Type: {proposal.projectType}</p>
            </div>
            <span className={`px-3 py-1 text-xs font-medium rounded-full ${getStatusColor(getOverallStatus())}`}>
              {getOverallStatus()}
            </span>
          </div>
        </div>
        
        {proposal.abstract && (
          <div className="mb-6">
            <h3 className="text-lg font-medium text-gray-700 mb-2">Abstract</h3>
            <div className="bg-gray-50 p-4 rounded-md">
              <p className="text-gray-700 whitespace-pre-line">{proposal.abstract}</p>
            </div>
          </div>
        )}
        
        {proposal.pdfFilePath && (
          <div className="mb-6">
            <h3 className="text-lg font-medium text-gray-700 mb-2">Uploaded Document</h3>
            <a 
              href={pdfUrl} 
              target="_blank" 
              rel="noopener noreferrer"
              className="flex items-center text-indigo-600 hover:text-indigo-800"
            >
              <svg xmlns="http://www.w3.org/2000/svg" className="h-5 w-5 mr-2" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 10v6m0 0l-3-3m3 3l3-3m2 8H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z" />
              </svg>
              View Uploaded PDF
            </a>
          </div>
        )}
        
        <div className="mt-8">
          <h3 className="text-lg font-medium text-gray-700 mb-2">Submission Details</h3>
          <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div className="bg-gray-50 p-4 rounded-md">
              <p className="text-sm text-gray-500">Academic Session</p>
              <p className="font-medium">{proposal.academicSession}</p>
            </div>
            <div className="bg-gray-50 p-4 rounded-md">
              <p className="text-sm text-gray-500">Semester</p>
              <p className="font-medium">{proposal.semester}</p>
            </div>
            <div className="bg-gray-50 p-4 rounded-md">
              <p className="text-sm text-gray-500">Supervisor</p>
              <p className="font-medium">{proposal.supervisorName || 'Not assigned'}</p>
            </div>
          </div>
        </div>

        {currentEvaluations.length > 0 && (
          <div className="mt-8">
            <h3 className="text-lg font-medium text-gray-700 mb-4">Current Evaluation Status</h3>
            <div className="space-y-4">
              {currentEvaluations.map((evaluation, index) => (
                <div key={index} className="bg-gray-50 p-6 rounded-lg border border-gray-200">
                  <div className="flex justify-between items-start mb-4">
                    <div>
                      <h4 className="font-medium text-gray-900">{evaluation.evaluatorName}</h4>
                      {evaluation.result ? (
                        <span className={`inline-block mt-1 px-2 py-1 text-xs font-medium rounded ${getStatusColor(evaluation.result)}`}>
                          {evaluation.result}
                        </span>
                      ) : (
                        <span className="inline-block mt-1 px-2 py-1 text-xs font-medium rounded bg-gray-100 text-gray-600">
                          Pending Feedback
                        </span>
                      )}
                    </div>
                  </div>
                  {evaluation.comments && (
                    <div className="prose prose-sm max-w-none">
                      <p className="text-gray-700 whitespace-pre-line">{evaluation.comments}</p>
                    </div>
                  )}
                </div>
              ))}
            </div>
          </div>
        )}

        {historicalEvaluations.length > 0 && (
          <div className="mt-8">
            <h3 className="text-lg font-medium text-gray-700 mb-4">Previous Feedback History</h3>
            <div className="space-y-4">
              {historicalEvaluations.map((evaluation, index) => (
                <div key={index} className="bg-gray-50/50 p-6 rounded-lg border border-gray-100">
                  <div className="flex justify-between items-start mb-4">
                    <div>
                      <h4 className="font-medium text-gray-700">{evaluation.evaluatorName}</h4>
                      <span className={`inline-block mt-1 px-2 py-1 text-xs font-medium rounded opacity-75 ${getStatusColor(evaluation.result)}`}>
                        {evaluation.result}
                      </span>
                    </div>
                  </div>
                  <div className="prose prose-sm max-w-none">
                    <p className="text-gray-600 whitespace-pre-line">{evaluation.comments}</p>
                  </div>
                </div>
              ))}
            </div>
          </div>
        )}

        {(getOverallStatus() === 'Rejected' || getOverallStatus() === 'Accepted with Conditions') && (
          <div className="mt-8 flex justify-end">
            <button
              onClick={() => navigate(`/student/proposal/edit/${proposal.id}`)}
              className="px-4 py-2 bg-indigo-600 text-white rounded-md hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500"
            >
              Resubmit Proposal
            </button>
          </div>
        )}
      </div>
    </div>
  );
};

export default MyProposal;