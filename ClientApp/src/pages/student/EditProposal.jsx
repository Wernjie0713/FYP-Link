import React, { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { toast } from 'react-toastify';
import apiClient from '../../api/apiClient';

const EditProposal = () => {
  const { proposalId } = useParams();
  const navigate = useNavigate();
  const [loading, setLoading] = useState(true);
  const [formData, setFormData] = useState({
    title: '',
    abstract: '',
    projectType: '',
    academicSession: '',
    semester: 1,
  });
  const [pdfFile, setPdfFile] = useState(null);

  useEffect(() => {
    const fetchProposal = async () => {
      try {
        const response = await apiClient.get('/api/proposals/my-proposal');
        const proposal = response.data;
        setFormData({
          title: proposal.title,
          abstract: proposal.abstract || '',
          projectType: proposal.projectType,
          academicSession: proposal.academicSession,
          semester: proposal.semester,
        });
        setLoading(false);
      } catch (error) {
        console.error('Error fetching proposal:', error);
        toast.error('Failed to load proposal data');
        navigate('/student/my-proposal');
      }
    };

    fetchProposal();
  }, [proposalId, navigate]);

  const handleInputChange = (e) => {
    const { name, value } = e.target;
    setFormData(prev => ({ ...prev, [name]: value }));
  };

  const handleFileChange = (e) => {
    if (e.target.files[0]) {
      setPdfFile(e.target.files[0]);
    }
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setLoading(true);

    try {
      const submitData = new FormData();
      submitData.append('title', formData.title);
      submitData.append('abstract', formData.abstract);
      submitData.append('projectType', formData.projectType);
      submitData.append('academicSession', formData.academicSession);
      submitData.append('semester', formData.semester);
      if (pdfFile) {
        submitData.append('pdfFile', pdfFile);
      }

      await apiClient.put(`/api/proposals/${proposalId}`, submitData, {
        headers: {
          'Content-Type': 'multipart/form-data',
        },
      });

      toast.success('Proposal resubmitted successfully');
      navigate('/student/my-proposal');
    } catch (error) {
      console.error('Error resubmitting proposal:', error);
      toast.error('Failed to resubmit proposal');
      setLoading(false);
    }
  };

  if (loading) {
    return <div className="flex justify-center items-center h-full">Loading...</div>;
  }

  return (
    <div className="container mx-auto px-4 py-8">
      <div className="max-w-3xl mx-auto">
        <h1 className="text-2xl font-bold mb-6">Edit Proposal</h1>

        <form onSubmit={handleSubmit} className="space-y-6">
          <div>
            <label htmlFor="title" className="block text-sm font-medium text-gray-700">Title</label>
            <input
              type="text"
              id="title"
              name="title"
              value={formData.title}
              onChange={handleInputChange}
              required
              className="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-indigo-500 focus:ring-indigo-500"
            />
          </div>

          <div>
            <label htmlFor="abstract" className="block text-sm font-medium text-gray-700">Abstract</label>
            <textarea
              id="abstract"
              name="abstract"
              value={formData.abstract}
              onChange={handleInputChange}
              rows={4}
              className="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-indigo-500 focus:ring-indigo-500"
            />
          </div>

          <div>
            <label htmlFor="projectType" className="block text-sm font-medium text-gray-700">Project Type</label>
            <select
              id="projectType"
              name="projectType"
              value={formData.projectType}
              onChange={handleInputChange}
              required
              className="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-indigo-500 focus:ring-indigo-500"
            >
              <option value="">Select a project type</option>
              <option value="Development">Development</option>
              <option value="Research">Research</option>
            </select>
          </div>

          <div>
            <label htmlFor="academicSession" className="block text-sm font-medium text-gray-700">Academic Session</label>
            <select
              id="academicSession"
              name="academicSession"
              value={formData.academicSession}
              onChange={handleInputChange}
              required
              className="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-indigo-500 focus:ring-indigo-500"
            >
              <option value="">Select an academic session</option>
              <option value="2024/2025">2024/2025</option>
              <option value="2025/2026">2025/2026</option>
              <option value="2026/2027">2026/2027</option>
            </select>
          </div>

          <div>
            <label htmlFor="semester" className="block text-sm font-medium text-gray-700">Semester</label>
            <select
              id="semester"
              name="semester"
              value={formData.semester}
              onChange={handleInputChange}
              required
              className="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-indigo-500 focus:ring-indigo-500"
            >
              <option value={1}>1</option>
              <option value={2}>2</option>
            </select>
          </div>

          <div>
            <label htmlFor="pdfFile" className="block text-sm font-medium text-gray-700">PDF Document (Optional)</label>
            <input
              type="file"
              id="pdfFile"
              onChange={handleFileChange}
              accept=".pdf"
              className="mt-1 block w-full text-sm text-gray-500
                file:mr-4 file:py-2 file:px-4
                file:rounded-md file:border-0
                file:text-sm file:font-medium
                file:bg-indigo-50 file:text-indigo-700
                hover:file:bg-indigo-100"
            />
          </div>

          <div className="flex justify-end space-x-4">
            <button
              type="button"
              onClick={() => navigate('/student/my-proposal')}
              className="px-4 py-2 border border-gray-300 rounded-md text-gray-700 hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500"
            >
              Cancel
            </button>
            <button
              type="submit"
              disabled={loading}
              className="px-4 py-2 bg-indigo-600 text-white rounded-md hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500 disabled:opacity-50"
            >
              {loading ? 'Saving...' : 'Save Changes'}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
};

export default EditProposal;