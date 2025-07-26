import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import axios from 'axios';
import { toast } from 'react-toastify';
import apiClient from '../../api/apiClient';

const SubmitProposal = () => {
  const navigate = useNavigate();
  const [loading, setLoading] = useState(false);
  const [studentProfile, setStudentProfile] = useState(null);
  const [hasProposal, setHasProposal] = useState(false);
  const [submissionType, setSubmissionType] = useState('online'); // 'online' or 'pdf'
  const academicSessions = ['2024/2025', '2025/2026', '2026/2027'];

  const [formData, setFormData] = useState({
    title: '',
    projectType: 'Research',
    abstract: '',
    pdfFile: null,
    academicSession: '2024/2025',
    semester: 1
  });

  // Fetch student profile and check if they already have a proposal
  useEffect(() => {
    const fetchStudentProfile = async () => {
      try {
        const response = await apiClient.get('/api/students/my-profile');
        setStudentProfile(response.data);

        // Check if student already has a proposal
        const proposalsResponse = await apiClient.get('/api/students/my-proposals');
        setHasProposal(proposalsResponse.data.length > 0);
        
        // If student has a proposal, redirect to the proposal page
        if (proposalsResponse.data.length > 0) {
          toast.info('You have already submitted a proposal');
          navigate('/student/my-proposal');
        }
        
        // Check if student has an approved supervisor
        if (response.data.approvalStatus !== 'Approved' || !response.data.supervisorId) {
          toast.error('You must have an approved supervisor before submitting a proposal');
          navigate('/student/select-supervisor');
        }
      } catch (error) {
        console.error('Error fetching data:', error);
        toast.error('Failed to load student profile');
      }
    };

    fetchStudentProfile();
  }, [navigate]);

  const handleInputChange = (e) => {
    const { name, value } = e.target;
    setFormData({
      ...formData,
      [name]: value
    });
  };

  const handleFileChange = (e) => {
    setFormData({
      ...formData,
      pdfFile: e.target.files[0]
    });
  };

  const handleSubmissionTypeChange = (type) => {
    setSubmissionType(type);
    // Clear the other field type
    if (type === 'online') {
      setFormData({
        ...formData,
        pdfFile: null
      });
    } else {
      setFormData({
        ...formData,
        abstract: ''
      });
    }
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    
    // Validate form
    if (!formData.title || !formData.projectType) {
      toast.error('Please fill in all required fields');
      return;
    }
    
    if (submissionType === 'online' && !formData.abstract) {
      toast.error('Please provide an abstract for your proposal');
      return;
    }
    
    if (submissionType === 'pdf' && !formData.pdfFile) {
      toast.error('Please upload a PDF file');
      return;
    }
    
    setLoading(true);
    
    try {
      // Create FormData object for multipart/form-data
      const submitData = new FormData();
      submitData.append('title', formData.title);
      submitData.append('projectType', formData.projectType);
      submitData.append('academicSession', formData.academicSession);
      submitData.append('semester', formData.semester);
      
      if (submissionType === 'online') {
        submitData.append('abstract', formData.abstract);
      } else {
        submitData.append('pdfFile', formData.pdfFile);
      }
      
      // Submit proposal
      const response = await apiClient.post('/api/proposals', submitData, {
        headers: {
          'Content-Type': 'multipart/form-data'
        }
      });
      
      toast.success('Proposal submitted successfully!');
      navigate('/student/my-proposal');
    } catch (error) {
      console.error('Error submitting proposal:', error);
      toast.error(error.response?.data?.message || 'Failed to submit proposal');
    } finally {
      setLoading(false);
    }
  };

  if (!studentProfile) {
    return <div className="flex justify-center items-center h-full">Loading...</div>;
  }

  return (
    <div className="container mx-auto px-4 py-8">
      <h1 className="text-2xl font-bold mb-6">Submit Project Proposal</h1>
      
      <div className="bg-white rounded-lg shadow-md p-6">
        <form onSubmit={handleSubmit}>
          {/* Title */}
          <div className="mb-4">
            <label htmlFor="title" className="block text-sm font-medium text-gray-700 mb-1">
              Project Title <span className="text-red-500">*</span>
            </label>
            <input
              type="text"
              id="title"
              name="title"
              value={formData.title}
              onChange={handleInputChange}
              className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-indigo-500 focus:border-indigo-500"
              required
            />
          </div>
          
          {/* Academic Session */}
          <div className="mb-4">
            <label htmlFor="academicSession" className="block text-sm font-medium text-gray-700 mb-1">
              Academic Session <span className="text-red-500">*</span>
            </label>
            <select
              id="academicSession"
              name="academicSession"
              value={formData.academicSession}
              onChange={handleInputChange}
              className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-indigo-500 focus:border-indigo-500"
              required
            >
              {academicSessions.map(session => (
                <option key={session} value={session}>{session}</option>
              ))}
            </select>
          </div>

          {/* Semester */}
          <div className="mb-4">
            <label htmlFor="semester" className="block text-sm font-medium text-gray-700 mb-1">
              Semester <span className="text-red-500">*</span>
            </label>
            <select
              id="semester"
              name="semester"
              value={formData.semester}
              onChange={handleInputChange}
              className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-indigo-500 focus:border-indigo-500"
              required
            >
              <option value={1}>1</option>
              <option value={2}>2</option>
            </select>
          </div>

          {/* Project Type */}
          <div className="mb-4">
            <label htmlFor="projectType" className="block text-sm font-medium text-gray-700 mb-1">
              Project Type <span className="text-red-500">*</span>
            </label>
            <select
              id="projectType"
              name="projectType"
              value={formData.projectType}
              onChange={handleInputChange}
              className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-indigo-500 focus:border-indigo-500"
              required
            >
              <option value="Research">Research</option>
              <option value="Development">Development</option>
            </select>
          </div>
          
          {/* Submission Type Selector */}
          <div className="mb-6">
            <label className="block text-sm font-medium text-gray-700 mb-2">
              Submission Method <span className="text-red-500">*</span>
            </label>
            <div className="flex space-x-4">
              <button
                type="button"
                className={`px-4 py-2 rounded-md ${submissionType === 'online' ? 'bg-indigo-600 text-white' : 'bg-gray-200 text-gray-700'}`}
                onClick={() => handleSubmissionTypeChange('online')}
              >
                Fill Online Form
              </button>
              <button
                type="button"
                className={`px-4 py-2 rounded-md ${submissionType === 'pdf' ? 'bg-indigo-600 text-white' : 'bg-gray-200 text-gray-700'}`}
                onClick={() => handleSubmissionTypeChange('pdf')}
              >
                Upload PDF
              </button>
            </div>
          </div>
          
          {/* Conditional Fields based on Submission Type */}
          {submissionType === 'online' ? (
            <div className="mb-6">
              <label htmlFor="abstract" className="block text-sm font-medium text-gray-700 mb-1">
                Project Abstract <span className="text-red-500">*</span>
              </label>
              <textarea
                id="abstract"
                name="abstract"
                value={formData.abstract}
                onChange={handleInputChange}
                rows="8"
                className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-indigo-500 focus:border-indigo-500"
                placeholder="Provide a detailed description of your project, including objectives, methodology, and expected outcomes..."
                required={submissionType === 'online'}
              />
              <p className="text-sm text-gray-500 mt-1">
                Minimum 300 words recommended for a comprehensive project description.
              </p>
            </div>
          ) : (
            <div className="mb-6">
              <label htmlFor="pdfFile" className="block text-sm font-medium text-gray-700 mb-1">
                Upload Proposal PDF <span className="text-red-500">*</span>
              </label>
              <input
                type="file"
                id="pdfFile"
                accept=".pdf"
                onChange={handleFileChange}
                className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-indigo-500 focus:border-indigo-500"
                required={submissionType === 'pdf'}
              />
              <p className="text-sm text-gray-500 mt-1">
                Maximum file size: 10MB. Your PDF should include a comprehensive project description.
              </p>
            </div>
          )}
          
          {/* Submit Button */}
          <div className="flex justify-end">
            <button
              type="submit"
              className="px-4 py-2 bg-indigo-600 text-white rounded-md hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500"
              disabled={loading}
            >
              {loading ? 'Submitting...' : 'Submit Proposal'}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
};

export default SubmitProposal;