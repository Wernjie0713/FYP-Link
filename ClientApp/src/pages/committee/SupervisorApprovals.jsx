import { useEffect, useState } from 'react';
import { CheckCircleOutlined, CloseCircleOutlined } from '@ant-design/icons';
import api from '../../api/apiClient';
import { toast } from 'react-toastify';

export default function SupervisorApprovals() {
  const [pending, setPending] = useState([]);
  const [loading, setLoading] = useState(true);

  const fetchPending = async () => {
    setLoading(true);
    try {
      const res = await api.get('/api/approvals/supervisors/pending');
      setPending(res.data);
    } catch (err) {
      toast.error('Failed to fetch pending requests');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchPending();
  }, []);

  const handleAction = async (studentId, action) => {
    try {
      await api.post(`/api/approvals/supervisors/${studentId}/${action}`);
      toast.success(`Request ${action}d successfully`);
      fetchPending();
    } catch (err) {
      toast.error(`Failed to ${action} request`);
    }
  };

  return (
    <div className="p-8">
      <h2 className="text-2xl font-bold mb-6">Supervisor Approvals</h2>
      {loading ? (
        <div>Loading...</div>
      ) : pending.length === 0 ? (
        <div className="text-gray-600">No pending supervisor requests.</div>
      ) : (
        <div className="overflow-x-auto">
          <table className="min-w-full bg-white border rounded shadow">
            <thead>
              <tr>
                <th className="px-4 py-2 border-b">Student Name</th>
                <th className="px-4 py-2 border-b">Matriculation Number</th>
                <th className="px-4 py-2 border-b">Requested Supervisor</th>
                <th className="px-4 py-2 border-b">Actions</th>
              </tr>
            </thead>
            <tbody>
              {pending.map((student) => (
                <tr key={student.id}>
                  <td className="px-4 py-2 border-b text-center">{student.name}</td>
                  <td className="px-4 py-2 border-b text-center">{student.matricNumber}</td>
                  <td className="px-4 py-2 border-b text-center">{student.supervisorName || '-'}</td>
                  <td className="px-4 py-2 border-b space-x-2 text-center">
                    <button
                      type="submit"
                      onClick={() => handleAction(student.id, 'approve')}
                      className="w-1/8 px-4 py-2 bg-teal-600 text-white rounded-md hover:bg-teal-700 focus:outline-none focus:ring-2 focus:ring-teal-500 focus:ring-offset-2 disabled:opacity-50 disabled:cursor-not-allowed"
                    >
                      {<CheckCircleOutlined />}
                    </button>
                    <button
                      type="submit" 
                      onClick={() => handleAction(student.id, 'reject')}
                      className="w-1/8 px-4 py-2 bg-red-600 text-white rounded-md hover:bg-red-700 focus:outline-none focus:ring-2 focus:ring-red-500 focus:ring-offset-2 disabled:opacity-50 disabled:cursor-not-allowed"
                    >
                      {<CloseCircleOutlined />}
                    </button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </div>
  );
} 