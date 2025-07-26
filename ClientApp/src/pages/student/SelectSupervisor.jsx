import { useState, useEffect } from 'react';
import api from '../../utils/axios';
import { toast } from 'react-hot-toast';
import { FiAlertCircle } from 'react-icons/fi';

export default function SelectSupervisor() {
  const [lecturers, setLecturers] = useState([]);
  const [loading, setLoading] = useState(true);
  const [selectedLecturer, setSelectedLecturer] = useState(null);
  const [showConfirmModal, setShowConfirmModal] = useState(false);
  const [submitting, setSubmitting] = useState(false);
  const [studentProfile, setStudentProfile] = useState(null);

  useEffect(() => {
    fetchStudentProfile();
    fetchLecturers();
  }, []);

  const fetchStudentProfile = async () => {
    try {
      const response = await api.get('/api/students/my-profile');
      setStudentProfile(response.data);
    } catch (err) {
      console.error('Error fetching student profile:', err);
      toast.error('Failed to load your profile information');
    }
  };

  const fetchLecturers = async () => {
    try {
      const response = await api.get('/api/lecturers');
      setLecturers(response.data);
    } catch (err) {
      console.error('Error fetching lecturers:', err);
      toast.error('Failed to load available supervisors');
    } finally {
      setLoading(false);
    }
  };

  const handleSelectLecturer = (lecturer) => {
    setSelectedLecturer(lecturer);
    setShowConfirmModal(true);
  };

  const handleConfirmSelection = async () => {
    if (!selectedLecturer) return;

    setSubmitting(true);
    try {
      await api.post('/api/students/select-supervisor', {
        supervisorId: selectedLecturer.id
      });
      toast.success('Supervisor selection submitted successfully');
      await fetchStudentProfile(); // Refresh the profile data
      setShowConfirmModal(false);
    } catch (err) {
      console.error('Error selecting supervisor:', err);
      toast.error(err.response?.data?.message || 'Failed to submit supervisor selection');
    } finally {
      setSubmitting(false);
    }
  };

  if (loading) {
    return (
      <div className="flex justify-center items-center min-h-[400px]">
        <div className="animate-spin rounded-full h-8 w-8 border-t-2 border-b-2 border-teal-500"></div>
      </div>
    );
  }

  const renderContent = () => {
    if (!studentProfile) return null;

    if (studentProfile.approvalStatus === 'Approved') {
      return (
        <div className="space-y-6">
          <div className="bg-green-50 border border-green-200 rounded-lg p-6 text-center">
            <h2 className="text-xl font-semibold text-green-800 mb-2">Supervisor Approved</h2>
            <p className="text-green-700">
              Your supervisor, <span className="font-semibold">{studentProfile.supervisorName}</span>, has been approved.
            </p>
          </div>
          
          {/* General FYP Agreement Section */}
          <div className="bg-slate-50 border border-slate-200 rounded-lg p-6">
            <h2 className="text-xl font-semibold text-slate-800 mb-4 text-center">General FYP Agreement</h2>
            
            <div className="space-y-4 text-slate-700">
              <p className="font-medium">This agreement outlines the mutual understanding between you and your supervisor for the successful completion of your Final Year Project.</p>
              
              <div className="space-y-2">
                <h3 className="text-lg font-medium text-slate-800">Key Terms:</h3>
                <ul className="list-disc pl-6 space-y-2">
                  <li><span className="font-medium">Mutual Commitment:</span> Both parties commit to the successful completion of this Final Year Project with professionalism and academic integrity.</li>
                  <li><span className="font-medium">Meeting Frequency:</span> Regular meetings will be scheduled bi-weekly, or as needed based on project milestones and progress.</li>
                  <li><span className="font-medium">Communication:</span> Primary communication will be through university email and scheduled face-to-face meetings. Urgent matters may be addressed through other agreed-upon channels.</li>
                  <li><span className="font-medium">Academic Integrity:</span> The student must adhere to all university policies regarding academic integrity and plagiarism. All work must be original or properly cited.</li>
                  <li><span className="font-medium">Progress Responsibility:</span> The student is responsible for maintaining consistent progress, meeting deadlines, and communicating any challenges promptly.</li>
                  <li><span className="font-medium">Feedback Timeline:</span> The supervisor will provide feedback on submitted work within a reasonable timeframe, typically within one week.</li>
                </ul>
              </div>
              
              <p className="italic text-sm text-slate-600 mt-4">This is a general agreement. Your department may have additional specific requirements or forms to complete.</p>
            </div>
          </div>
        </div>
      );
    }

    if (studentProfile.approvalStatus === 'Pending') {
      return (
        <div className="bg-yellow-50 border border-yellow-200 rounded-lg p-6 text-center">
          <h2 className="text-xl font-semibold text-yellow-800 mb-2">Supervisor Selection Pending</h2>
          <p className="text-yellow-700">
            Your supervisor selection has been submitted and is now pending approval.
          </p>
        </div>
      );
    }

    console.log(studentProfile);

    return (
      <>
        {studentProfile.approvalStatus === 'Rejected' && (
          <div className="bg-red-50 border border-red-200 rounded-lg p-6 mb-6">
            <div className="flex items-center">
              <FiAlertCircle className="text-red-500 mr-2" size={20} />
              <h2 className="text-lg font-semibold text-red-800">Previous Selection Rejected</h2>
            </div>
            <p className="text-red-700 mt-2">
              Your previous supervisor request was rejected. Please select a new supervisor.
            </p>
          </div>
        )}

        <div className="bg-white shadow overflow-hidden rounded-lg">
          <table className="min-w-full divide-y divide-gray-200">
            <thead className="bg-gray-50">
              <tr>
                <th scope="col" className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Name
                </th>
                <th scope="col" className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Department
                </th>
                <th scope="col" className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Domain
                </th>
                <th scope="col" className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Action
                </th>
              </tr>
            </thead>
            <tbody className="bg-white divide-y divide-gray-200">
              {lecturers.map((lecturer) => (
                <tr key={lecturer.id}>
                  <td className="px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900">
                    {lecturer.name}
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                    {lecturer.department}
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                    {lecturer.domain || 'Not specified'}
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                    <button
                      onClick={() => handleSelectLecturer(lecturer)}
                      className="text-teal-600 hover:text-teal-900 font-medium"
                    >
                      Select
                    </button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </>
    );
  };

  return (
    <div className="max-w-4xl mx-auto p-6 space-y-6">
      <h1 className="text-2xl font-bold text-gray-900">Select Supervisor</h1>
      
      {renderContent()}

      {/* Confirmation Modal */}
      {showConfirmModal && (
        <div className="fixed inset-0 bg-gray-500 bg-opacity-75 flex items-center justify-center p-4">
          <div className="bg-white rounded-lg max-w-md w-full p-6">
            <h3 className="text-lg font-medium text-gray-900 mb-4">
              Confirm Supervisor Selection
            </h3>
            <p className="text-sm text-gray-500 mb-4">
              Are you sure you want to select <span className="font-medium">{selectedLecturer.name}</span> as your supervisor?
            </p>
            <div className="flex justify-end space-x-4">
              <button
                onClick={() => setShowConfirmModal(false)}
                disabled={submitting}
                className="px-4 py-2 text-sm font-medium text-gray-700 hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-teal-500 disabled:opacity-50"
              >
                Cancel
              </button>
              <button
                onClick={handleConfirmSelection}
                disabled={submitting}
                className="px-4 py-2 text-sm font-medium text-white bg-teal-600 hover:bg-teal-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-teal-500 disabled:opacity-50"
              >
                {submitting ? 'Confirming...' : 'Confirm'}
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}