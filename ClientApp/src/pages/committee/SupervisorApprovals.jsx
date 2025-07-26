import { useEffect, useState } from 'react';
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
                      className="bg-green-600 text-white px-3 py-1 rounded hover:bg-green-700"
                      onClick={() => handleAction(student.id, 'approve')}
                    >
                      Approve
                    </button>
                    <button
                      className="bg-red-600 text-white px-3 py-1 rounded hover:bg-red-700"
                      onClick={() => handleAction(student.id, 'reject')}
                    >
                      Reject
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