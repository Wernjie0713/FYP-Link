import { useEffect, useState, useRef } from 'react';
import api from '../../api/apiClient';
import { FiEye, FiUserPlus } from 'react-icons/fi';

const SESSIONS = [
  '2024/2025',
  '2025/2026',
];
const SEMESTERS = [1, 2];

export default function ManageProposals() {
  const [session, setSession] = useState('');
  const [semester, setSemester] = useState('');
  const [proposals, setProposals] = useState([]);
  const [loading, setLoading] = useState(false);
  const [selected, setSelected] = useState(null);
  const [modalOpen, setModalOpen] = useState(false);
  const [assignModalOpen, setAssignModalOpen] = useState(false);
  const [assigningProposal, setAssigningProposal] = useState(null);
  const [availableEvaluators, setAvailableEvaluators] = useState([]);
  const [selectedEvaluators, setSelectedEvaluators] = useState([]);
  const [assignLoading, setAssignLoading] = useState(false);
  const selectRef1 = useRef();
  const selectRef2 = useRef();

  const fetchProposals = async () => {
    setLoading(true);
    try {
      const params = {};
      if (session) params.session = session;
      if (semester) params.semester = semester;
      const res = await api.get('/api/proposals', { params });
      setProposals(res.data);
    } catch (err) {
      // handle error
    } finally {
      setLoading(false);
    }
  };

  const openAssignModal = async (proposal) => {
    setAssigningProposal(proposal);
    // Pre-populate with existing evaluator IDs if any
    const existingEvaluatorIds = proposal.evaluations
      ? proposal.evaluations.map(e => e.evaluatorId)
      : [];
    setSelectedEvaluators(existingEvaluatorIds);
    setAssignLoading(true);
    try {
      const res = await api.get(`/api/evaluations/available/${proposal.id}`);
      setAvailableEvaluators(res.data);
    } catch (err) {
      setAvailableEvaluators([]);
    } finally {
      setAssignLoading(false);
      setAssignModalOpen(true);
    }
  };

  const handleAssign = async () => {
    if (selectedEvaluators.length !== 2) return;
    setAssignLoading(true);
    try {
      await api.post('/api/evaluations', {
        proposalId: assigningProposal.id,
        evaluatorIds: selectedEvaluators
      });
      setAssignModalOpen(false);
      setAssigningProposal(null);
      setSelectedEvaluators([]);
      fetchProposals();
    } catch (err) {
      // handle error
    } finally {
      setAssignLoading(false);
    }
  };

  useEffect(() => {
    fetchProposals();
    // eslint-disable-next-line
  }, [session, semester]);

  return (
    <div className="p-8">
      <h2 className="text-2xl font-bold mb-6">Manage Proposals</h2>
      <div className="flex gap-4 mb-6">
        <div>
          <label className="block text-sm font-medium mb-1">Academic Session</label>
          <select
            className="border rounded px-3 py-2"
            value={session}
            onChange={e => setSession(e.target.value)}
          >
            <option value="">All</option>
            {SESSIONS.map(s => (
              <option key={s} value={s}>{s}</option>
            ))}
          </select>
        </div>
        <div>
          <label className="block text-sm font-medium mb-1">Semester</label>
          <select
            className="border rounded px-3 py-2"
            value={semester}
            onChange={e => setSemester(e.target.value)}
          >
            <option value="">All</option>
            {SEMESTERS.map(s => (
              <option key={s} value={s}>{s}</option>
            ))}
          </select>
        </div>
      </div>
      {loading ? (
        <div>Loading...</div>
      ) : proposals.length === 0 ? (
        <div className="text-gray-600">No proposals found.</div>
      ) : (
        <div className="overflow-x-auto">
          <table className="min-w-full bg-white border rounded shadow">
            <thead>
              <tr>
                <th className="px-4 py-2 border-b text-left">Title</th>
                <th className="px-4 py-2 border-b text-left">Student</th>
                <th className="px-4 py-2 border-b text-left">Supervisor</th>
                <th className="px-4 py-2 border-b text-left">Project Type</th>
                <th className="px-4 py-2 border-b text-left">Evaluators</th>
                <th className="px-4 py-2 border-b text-center">Actions</th>
              </tr>
            </thead>
            <tbody>
              {proposals.map((p) => (
                <tr key={p.id}>
                  <td className="px-4 py-2 border-b">{p.title}</td>
                  <td className="px-4 py-2 border-b">{p.studentName}</td>
                  <td className="px-4 py-2 border-b">{p.supervisorName}</td>
                  <td className="px-4 py-2 border-b">{p.projectType}</td>
                  <td className="px-4 py-2 border-b">
                    {p.evaluations && p.evaluations.length > 0
                      ? p.evaluations.map(e => e.evaluatorName).join(', ')
                      : <span className="text-gray-400">Not assigned</span>}
                  </td>
                  <td className="px-4 py-2 border-b text-center">
                    <button
                      className="bg-teal-600 text-white p-2 rounded hover:bg-teal-700"
                      onClick={() => { setSelected(p); setModalOpen(true); }}
                      title="View Details"
                    >
                      <FiEye className="w-4 h-4" />
                    </button>
                    <button
                      className="ml-2 bg-blue-600 text-white p-2 rounded hover:bg-blue-700"
                      onClick={() => openAssignModal(p)}
                      title="Assign Evaluators"
                    >
                      <FiUserPlus className="w-4 h-4" />
                    </button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
      {/* Modal for details */}
      {modalOpen && selected && (
        <div className="fixed inset-0 flex items-center justify-center bg-black bg-opacity-40 z-50">
          <div className="bg-white rounded-lg shadow-lg p-8 w-full max-w-lg relative">
            <button
              className="absolute top-2 right-2 text-gray-500 hover:text-gray-800"
              onClick={() => setModalOpen(false)}
            >
              &times;
            </button>
            <h3 className="text-xl font-bold mb-4">Proposal Details</h3>
            <div className="mb-2"><strong>Title:</strong> {selected.title}</div>
            <div className="mb-2"><strong>Student:</strong> {selected.studentName}</div>
            <div className="mb-2"><strong>Supervisor:</strong> {selected.supervisorName}</div>
            <div className="mb-2"><strong>Project Type:</strong> {selected.projectType}</div>
            <div className="mb-2"><strong>Academic Session:</strong> {selected.academicSession}</div>
            <div className="mb-2"><strong>Semester:</strong> {selected.semester}</div>
            <div className="mb-2"><strong>Abstract:</strong> <div className="whitespace-pre-line border p-2 rounded bg-gray-50 mt-1">{selected.abstract}</div></div>
            {/* Evaluation Results */}
            <div className="mt-6">
              <h4 className="text-lg font-semibold mb-2">Evaluation Results</h4>
              {selected.evaluations && selected.evaluations.length > 0 ? (
                <div className="space-y-4">
                  {selected.evaluations.map((evalItem) => (
                    <div key={evalItem.id} className="border rounded p-4 bg-gray-50">
                      <div className="mb-1"><strong>Evaluator:</strong> {evalItem.evaluatorName}</div>
                      <div className="mb-1"><strong>Result:</strong> {evalItem.result}</div>
                      <div><strong>Comments:</strong> <span className="whitespace-pre-line">{evalItem.comments}</span></div>
                    </div>
                  ))}
                </div>
              ) : (
                <div className="text-gray-500">No evaluations yet.</div>
              )}
            </div>
            {/* Comments/Evaluation will be added later */}
          </div>
        </div>
      )}
      {/* Assign Evaluators Modal */}
      {assignModalOpen && assigningProposal && (
        <div className="fixed inset-0 flex items-center justify-center bg-black bg-opacity-40 z-50">
          <div className="bg-white rounded-lg shadow-lg p-8 w-full max-w-md relative">
            <button
              className="absolute top-2 right-2 text-gray-500 hover:text-gray-800"
              onClick={() => setAssignModalOpen(false)}
            >
              &times;
            </button>
            <h3 className="text-xl font-bold mb-4">Assign Evaluators</h3>
            {assignLoading ? (
              <div>Loading...</div>
            ) : availableEvaluators.length === 0 ? (
              <div className="text-gray-500">No available evaluators found.</div>
            ) : (
              <>
                <div className="mb-4">
                  <label className="block mb-1 font-medium">Select Evaluator 1</label>
                  <select
                    ref={selectRef1}
                    className="border rounded px-3 py-2 w-full"
                    value={selectedEvaluators[0] || ''}
                    onChange={e => {
                      const val = parseInt(e.target.value);
                      setSelectedEvaluators([val, selectedEvaluators[1] || ''].filter(Boolean));
                    }}
                  >
                    <option value="">-- Select --</option>
                    {availableEvaluators.map(ev => (
                      <option key={ev.id} value={ev.id}>{ev.name} ({ev.domain})</option>
                    ))}
                  </select>
                </div>
                <div className="mb-4">
                  <label className="block mb-1 font-medium">Select Evaluator 2</label>
                  <select
                    ref={selectRef2}
                    className="border rounded px-3 py-2 w-full"
                    value={selectedEvaluators[1] || ''}
                    onChange={e => {
                      const val = parseInt(e.target.value);
                      setSelectedEvaluators([selectedEvaluators[0] || '', val].filter(Boolean));
                    }}
                  >
                    <option value="">-- Select --</option>
                    {availableEvaluators
                      .filter(ev => ev.id !== selectedEvaluators[0])
                      .map(ev => (
                        <option key={ev.id} value={ev.id}>{ev.name} ({ev.domain})</option>
                      ))}
                  </select>
                </div>
                <button
                  className="bg-teal-600 text-white px-4 py-2 rounded hover:bg-teal-700 w-full"
                  disabled={selectedEvaluators.length !== 2 || assignLoading}
                  onClick={handleAssign}
                >
                  Save Assignments
                </button>
              </>
            )}
          </div>
        </div>
      )}
    </div>
  );
}