import { useState, useEffect } from 'react';
import { FiEdit2, FiTrash2, FiPlus, FiTarget } from 'react-icons/fi';
import apiClient from '../../api/apiClient';
import toast from 'react-hot-toast';
import { useAuth } from '../../context/AuthContext';

const positionOptions = [
  { value: 'Lecturer', label: 'Lecturer' },
  { value: 'SeniorLecturer', label: 'Senior Lecturer' },
  { value: 'AssociateProfessor', label: 'Associate Professor' },
  { value: 'Professor', label: 'Professor' }
];

const domainOptions = [
  { value: 'Research', label: 'Research' },
  { value: 'Development', label: 'Development' }
];

export default function ManageLecturers() {
  const { user } = useAuth();
  const isCommittee = user?.roles?.includes('Committee');
  const [lecturers, setLecturers] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [showModal, setShowModal] = useState(false);
  const [showDeleteModal, setShowDeleteModal] = useState(false);
  const [showDomainModal, setShowDomainModal] = useState(false);
  const [selectedLecturer, setSelectedLecturer] = useState(null);
  const [selectedDomain, setSelectedDomain] = useState('');
  const [formData, setFormData] = useState({
    name: '',
    email: '',
    staffId: '',
    department: '',
    currentPosition: 'Lecturer' // matches the value property from positionOptions
  });

  useEffect(() => {
    fetchLecturers();
  }, []);

  const fetchLecturers = async () => {
    try {
      const response = await apiClient.get('/api/lecturers');
      setLecturers(response.data);
      setError('');
    } catch (err) {
      setError('Failed to fetch lecturers');
      console.error('Error fetching lecturers:', err);
    } finally {
      setLoading(false);
    }
  };

  const fetchDomains = async () => {
    try {
      const res = await apiClient.get('/api/academicprograms');
      setDomainOptions(res.data.map(p => ({ value: p.name, label: p.name })));
    } catch (err) {
      setDomainOptions([]);
    }
  };

  const handleDomainSubmit = async () => {
    try {
      await apiClient.put(`/api/lecturers/${selectedLecturer.id}/domain`, { domain: selectedDomain });
      toast.success('Domain updated successfully!');
      fetchLecturers();
      setShowDomainModal(false);
      setSelectedLecturer(null);
      setSelectedDomain('');
    } catch (err) {
      console.error('Error updating domain:', err);
      toast.error('Failed to update domain');
    }
  };

  const openDomainModal = (lecturer) => {
    setSelectedLecturer(lecturer);
    setSelectedDomain(lecturer.domain || '');
    setShowDomainModal(true);
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    try {
      if (selectedLecturer) {
        // Update existing lecturer
        await apiClient.put(`/api/lecturers/${selectedLecturer.id}`, formData);
        toast.success('Lecturer updated successfully!');
        fetchLecturers();
        setShowModal(false);
        resetForm();
      } else {
        // Create new lecturer
        await apiClient.post('/api/lecturers', formData);
        toast.success('Lecturer created successfully!');
        fetchLecturers();
        setShowModal(false);
        resetForm();
      }
    } catch (err) {
      console.error('Error saving lecturer:', err);
      if (err.response?.data?.message) {
        setError(err.response.data.message);
      } else if (err.response?.data?.errors) {
        const errorMessages = Object.values(err.response.data.errors)
          .flat()
          .join(', ');
        setError(errorMessages);
      } else {
        setError(selectedLecturer ? 'Failed to update lecturer' : 'Failed to create lecturer');
      }
    }
  };

  const handleDelete = async () => {
    if (!selectedLecturer) return;

    try {
      await apiClient.delete(`/api/lecturers/${selectedLecturer.id}`);
      toast.success('Lecturer deleted successfully!');
      fetchLecturers();
      setShowDeleteModal(false);
      setSelectedLecturer(null);
    } catch (err) {
      console.error('Error deleting lecturer:', err);
      if (err.response?.data?.message) {
        setError(err.response.data.message);
      } else {
        setError('Failed to delete lecturer');
      }
    }
  };

  const openEditModal = (lecturer) => {
    setSelectedLecturer(lecturer);
    setFormData({
      name: lecturer.name,
      email: lecturer.email,
      staffId: lecturer.staffId,
      department: lecturer.department,
      currentPosition: lecturer.currentPosition
    });
    setError('');
    setShowModal(true);
  };

  const openCreateModal = () => {
    setSelectedLecturer(null);
    resetForm();
    setError('');
    setShowModal(true);
  };

  const resetForm = () => {
    setFormData({
      name: '',
      email: '',
      staffId: '',
      department: '',
      currentPosition: 'Lecturer'
    });
    setSelectedLecturer(null);
    setError('');
  };

  if (loading) {
    return <div className="text-center py-8">Loading...</div>;
  }

  return (
    <div>
      <div className="flex justify-between items-center mb-6">
        <h1 className="text-2xl font-bold text-gray-900">Manage Lecturers</h1>
        {!isCommittee && (
          <button
            onClick={openCreateModal}
            className="flex items-center px-4 py-2 bg-teal-600 text-white rounded-lg hover:bg-teal-700 transition-colors"
          >
            <FiPlus className="mr-2" />
            Add Lecturer
          </button>
        )}
      </div>

      {error && (
        <div className="bg-red-50 text-red-600 p-4 rounded-lg mb-6">
          {error}
        </div>
      )}

      <div className="bg-white rounded-lg shadow overflow-hidden">
        <table className="min-w-full divide-y divide-gray-200">
          <thead className="bg-gray-50">
            <tr>
              <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                Name
              </th>
              <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                Staff ID
              </th>
              <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                Email
              </th>
              <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                Department
              </th>
              <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                Position
              </th>
              <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                Domain
              </th>
              <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">
                Actions
              </th>
            </tr>
          </thead>
          <tbody className="bg-white divide-y divide-gray-200">
            {lecturers.map((lecturer) => (
              <tr key={lecturer.id}>
                <td className="px-6 py-4 whitespace-nowrap">
                  <div className="text-sm font-medium text-gray-900">{lecturer.name}</div>
                </td>
                <td className="px-6 py-4 whitespace-nowrap">
                  <div className="text-sm text-gray-500">{lecturer.staffId}</div>
                </td>
                <td className="px-6 py-4 whitespace-nowrap">
                  <div className="text-sm text-gray-500">{lecturer.email}</div>
                </td>
                <td className="px-6 py-4 whitespace-nowrap">
                  <div className="text-sm text-gray-500">{lecturer.department}</div>
                </td>
                <td className="px-6 py-4 whitespace-nowrap">
                  <div className="text-sm text-gray-500">
                    {positionOptions.find(option => option.value === lecturer.currentPosition)?.label || lecturer.currentPosition}
                  </div>
                </td>
                <td className="px-6 py-4 whitespace-nowrap">
                  <div className="text-sm text-gray-500">{lecturer.domain || '-'}</div>
                </td>
                <td className="px-6 py-4 whitespace-nowrap text-right text-sm font-medium">
                  {isCommittee ? (
                    <button
                      onClick={() => openDomainModal(lecturer)}
                      className="text-teal-600 hover:text-teal-900"
                    >
                      <FiTarget className="inline" />
                    </button>
                  ) : (
                    <>
                      <button
                        onClick={() => openEditModal(lecturer)}
                        className="text-teal-600 hover:text-teal-900 mr-4"
                      >
                        <FiEdit2 className="inline" />
                      </button>
                      <button
                        onClick={() => {
                          setSelectedLecturer(lecturer);
                          setShowDeleteModal(true);
                        }}
                        className="text-red-600 hover:text-red-900"
                      >
                        <FiTrash2 className="inline" />
                      </button>
                    </>
                  )}
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>

      {/* Create/Edit Modal */}
      {showModal && (
        <div className="fixed inset-0 bg-gray-500 bg-opacity-75 flex items-center justify-center p-4">
          <div className="bg-white rounded-lg max-w-md w-full p-6">
            <h2 className="text-xl font-semibold mb-4">
              {selectedLecturer ? 'Edit Lecturer' : 'Add New Lecturer'}
            </h2>
            <form onSubmit={handleSubmit}>
              <div className="mb-4">
                <label className="block text-sm font-medium text-gray-700 mb-2">
                  Name
                </label>
                <input
                  type="text"
                  value={formData.name}
                  onChange={(e) => setFormData({ ...formData, name: e.target.value })}
                  className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-1 focus:ring-teal-500"
                  required
                />
              </div>
              <div className="mb-4">
                <label className="block text-sm font-medium text-gray-700 mb-2">
                  Staff ID
                </label>
                <input
                  type="text"
                  value={formData.staffId}
                  onChange={(e) => setFormData({ ...formData, staffId: e.target.value })}
                  className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-1 focus:ring-teal-500"
                  required
                />
              </div>
              <div className="mb-4">
                <label className="block text-sm font-medium text-gray-700 mb-2">
                  Email
                </label>
                <input
                  type="email"
                  value={formData.email}
                  onChange={(e) => setFormData({ ...formData, email: e.target.value })}
                  className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-1 focus:ring-teal-500"
                  required
                />
              </div>
              <div className="mb-4">
                <label className="block text-sm font-medium text-gray-700 mb-2">
                  Department
                </label>
                <input
                  type="text"
                  value={formData.department}
                  onChange={(e) => setFormData({ ...formData, department: e.target.value })}
                  className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-1 focus:ring-teal-500"
                  required
                />
              </div>
              <div className="mb-6">
                <label className="block text-sm font-medium text-gray-700 mb-2">
                  Position
                </label>
                <select
                  value={formData.currentPosition}
                  onChange={(e) => setFormData({ ...formData, currentPosition: e.target.value })}
                  className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-1 focus:ring-teal-500"
                  required
                >
                  {positionOptions.map(option => (
                    <option key={option.value} value={option.value}>
                      {option.label}
                    </option>
                  ))}
                </select>
              </div>
              <div className="flex justify-end space-x-3">
                <button
                  type="button"
                  onClick={() => {
                    setShowModal(false);
                    resetForm();
                  }}
                  className="px-4 py-2 text-sm font-medium text-gray-700 hover:bg-gray-50 border border-gray-300 rounded-md"
                >
                  Cancel
                </button>
                <button
                  type="submit"
                  className="px-4 py-2 text-sm font-medium text-white bg-teal-600 hover:bg-teal-700 rounded-md"
                >
                  {selectedLecturer ? 'Update' : 'Create'}
                </button>
              </div>
            </form>
          </div>
        </div>
      )}

      {/* Domain Modal */}
      {showDomainModal && (
        <div className="fixed inset-0 bg-gray-500 bg-opacity-75 flex items-center justify-center p-4">
          <div className="bg-white rounded-lg max-w-md w-full p-6">
            <h2 className="text-xl font-semibold mb-4">Set Domain</h2>
            <div className="mb-6">
              <label className="block text-sm font-medium text-gray-700 mb-2">
                Domain
              </label>
              <select
                value={selectedDomain}
                onChange={(e) => setSelectedDomain(e.target.value)}
                className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-1 focus:ring-teal-500"
                required
              >
                <option value="">Select a domain</option>
                {domainOptions.map(option => (
                  <option key={option.value} value={option.value}>
                    {option.label}
                  </option>
                ))}
              </select>
            </div>
            <div className="flex justify-end space-x-3">
              <button
                onClick={() => {
                  setShowDomainModal(false);
                  setSelectedLecturer(null);
                  setSelectedDomain('');
                }}
                className="px-4 py-2 text-sm font-medium text-gray-700 hover:bg-gray-50 border border-gray-300 rounded-md"
              >
                Cancel
              </button>
              <button
                onClick={handleDomainSubmit}
                className="px-4 py-2 text-sm font-medium text-white bg-teal-600 hover:bg-teal-700 rounded-md"
              >
                Save
              </button>
            </div>
          </div>
        </div>
      )}

      {/* Delete Confirmation Modal */}
      {showDeleteModal && (
        <div className="fixed inset-0 bg-gray-500 bg-opacity-75 flex items-center justify-center p-4">
          <div className="bg-white rounded-lg max-w-md w-full p-6">
            <h2 className="text-xl font-semibold mb-4">Confirm Delete</h2>
            <p className="mb-6 text-gray-600">
              Are you sure you want to delete "{selectedLecturer.name}"? This will also delete their user account.
            </p>
            <div className="flex justify-end space-x-3">
              <button
                onClick={() => {
                  setShowDeleteModal(false);
                  setSelectedLecturer(null);
                }}
                className="px-4 py-2 text-sm font-medium text-gray-700 hover:bg-gray-50 border border-gray-300 rounded-md"
              >
                Cancel
              </button>
              <button
                onClick={handleDelete}
                className="px-4 py-2 text-sm font-medium text-white bg-red-600 hover:bg-red-700 rounded-md"
              >
                Delete
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}