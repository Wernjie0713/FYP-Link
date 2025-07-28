import { useState, useEffect } from 'react';
import { Table, Button, Modal, Form, Select, Input, Spin, Alert, Tag } from 'antd';
import { EyeOutlined } from '@ant-design/icons';
import apiClient from '../../api/apiClient';
import toast from 'react-hot-toast';

const { TextArea } = Input;
const { Option } = Select;

const MyAssignments = () => {
  const [assignments, setAssignments] = useState([]);
  const [loading, setLoading] = useState(true);
  const [modalVisible, setModalVisible] = useState(false);
  const [selectedProposal, setSelectedProposal] = useState(null);
  const [evaluationResult, setEvaluationResult] = useState('');
  const [evaluationComments, setEvaluationComments] = useState('');
  const [submitting, setSubmitting] = useState(false);
  const [coEvaluatorFeedback, setCoEvaluatorFeedback] = useState(null);
  const [myEvaluation, setMyEvaluation] = useState(null);
  const [evaluationHistory, setEvaluationHistory] = useState([]);

  // Fetch assignments on component mount
  useEffect(() => {
    fetchAssignments();
  }, []);

  const fetchAssignments = async () => {
    setLoading(true);
    try {
      const response = await apiClient.get('/api/evaluator/my-assignments');
      setAssignments(response.data);
    } catch (error) {
      console.error('Error fetching assignments:', error);
      toast.error('Failed to load assignments');
    } finally {
      setLoading(false);
    }
  };

  const handleViewEvaluate = async (proposalId) => {
    try {
      const response = await apiClient.get(`/api/evaluator/proposals/${proposalId}`);
      setSelectedProposal(response.data);
      
      // Find my evaluation and co-evaluator feedback
      const currentEvaluationId = response.data.currentEvaluationId;
      const myEvaluation = response.data.evaluations.find(
        e => e.evaluationId === currentEvaluationId
      );
      const otherEvaluations = response.data.evaluations.filter(
        e => e.evaluationId !== currentEvaluationId && e.isCurrent
      );
      
      setMyEvaluation(myEvaluation);
      setCoEvaluatorFeedback(otherEvaluations.length > 0 ? otherEvaluations : null);
      
      // Get evaluation history (non-current evaluations)
      const history = response.data.evaluations.filter(e => !e.isCurrent);
      setEvaluationHistory(history);
      
      // Pre-fill form if this evaluator has already submitted
      if (myEvaluation && myEvaluation.result) {
        setEvaluationResult(myEvaluation.result);
        setEvaluationComments(myEvaluation.comments || '');
      } else {
        setEvaluationResult('');
        setEvaluationComments('');
      }
      
      setModalVisible(true);
    } catch (error) {
      console.error('Error fetching proposal details:', error);
      toast.error('Failed to load proposal details');
    }
  };

  const handleSubmitEvaluation = async () => {
    if (!evaluationResult) {
      toast.error('Please select a result');
      return;
    }
    
    if (!evaluationComments) {
      toast.error('Please provide comments');
      return;
    }
    
    setSubmitting(true);
    try {
      await apiClient.post(`/api/evaluator/evaluations/${selectedProposal.currentEvaluationId}`, {
        result: evaluationResult,
        comments: evaluationComments
      });
      
      toast.success('Evaluation submitted successfully');
      setModalVisible(false);
      fetchAssignments(); // Refresh the list
    } catch (error) {
      console.error('Error submitting evaluation:', error);
      if (error.response?.status === 403) {
        toast.error(error.response.data.message || 'Cannot evaluate until supervisor submits comments');
      } else {
        toast.error('Failed to submit evaluation');
      }
    } finally {
      setSubmitting(false);
    }
  };

  const columns = [
    {
      title: 'Proposal Title',
      dataIndex: 'title',
      key: 'title',
    },
    {
      title: 'Student',
      dataIndex: 'studentName',
      key: 'studentName',
    },
    {
      title: 'Supervisor',
      dataIndex: 'supervisorName',
      key: 'supervisorName',
    },
    {
      title: 'Status',
      key: 'status',
      render: (_, record) => {
        if (record.result) {
          return <Tag color="green">Evaluated</Tag>;
        } else {
          return <Tag color="orange">Pending</Tag>;
        }
      },
    },
    {
      title: 'Actions',
      key: 'actions',
      render: (_, record) => (
        <button
            type="submit"
            onClick={() => handleViewEvaluate(record.proposalId)}
            className="w-1/8 px-4 py-2 bg-teal-600 text-white rounded-md hover:bg-teal-700 focus:outline-none focus:ring-2 focus:ring-teal-500 focus:ring-offset-2 disabled:opacity-50 disabled:cursor-not-allowed"
            >
            {<EyeOutlined />}  View & Evaluate
        </button>
      ),
    },
  ];

  return (
    <div>
      <h1 className="text-2xl font-bold mb-6">My Evaluation Assignments</h1>
      
      {loading ? (
        <div className="flex justify-center my-12">
          <Spin size="large" />
        </div>
      ) : (
        <Table 
          columns={columns} 
          dataSource={assignments} 
          rowKey="evaluationId" 
          pagination={false}
          locale={{ emptyText: 'No assignments found' }}
        />
      )}
      
      {/* Evaluation Modal */}
      <Modal
        title="Evaluate Proposal"
        open={modalVisible}
        onCancel={() => setModalVisible(false)}
        footer={null}
        width={800}
      >
        {selectedProposal ? (
          <div>
            <h2 className="text-xl font-bold mb-4">{selectedProposal.title}</h2>
            
            <div className="grid grid-cols-2 gap-4 mb-4">
              <div>
                <p className="font-semibold">Student:</p>
                <p>{selectedProposal.student.name}</p>
              </div>
              <div>
                <p className="font-semibold">Supervisor:</p>
                <p>{selectedProposal.supervisor.name}</p>
              </div>
              <div>
                <p className="font-semibold">Project Type:</p>
                <p>{selectedProposal.projectType}</p>
              </div>
              <div>
                <p className="font-semibold">Academic Session:</p>
                <p>{selectedProposal.academicSession} (Semester {selectedProposal.semester})</p>
              </div>
            </div>
            
            <div className="mb-4">
              <p className="font-semibold">Abstract:</p>
              <p className="bg-gray-50 p-3 rounded">{selectedProposal.abstract}</p>
            </div>
            
            {selectedProposal.pdfFilePath && (
              <div className="mb-4">
                <p className="font-semibold">Proposal Document:</p>
                <a 
                  href={`${import.meta.env.VITE_API_BASE_URL}${selectedProposal.pdfFilePath}`} 
                  target="_blank" 
                  rel="noopener noreferrer"
                  className="text-blue-500 hover:underline"
                >
                  View PDF
                </a>
              </div>
            )}
            
            <div className="mb-6">
              <p className="font-semibold">Supervisor's Comment:</p>
              {selectedProposal.supervisorComment ? (
                <div className="bg-blue-50 p-3 rounded">
                  <p>{selectedProposal.supervisorComment}</p>
                  <p className="text-xs text-gray-500 mt-1">
                    Commented on: {new Date(selectedProposal.supervisorCommentedAt).toLocaleString()}
                  </p>
                </div>
              ) : (
                <Alert 
                  message="Awaiting supervisor's initial review before evaluation can proceed." 
                  type="warning" 
                />
              )}
            </div>
            
            {/* Co-Evaluator Feedback Section */}
            <div className="mb-6">
              <h3 className="text-lg font-semibold mb-2">Co-Evaluator Feedback</h3>
              {coEvaluatorFeedback ? (
                coEvaluatorFeedback.map(evaluation => (
                  <div key={evaluation.evaluationId} className="bg-gray-50 p-3 rounded mb-2">
                    <div className="flex justify-between">
                      <p className="font-medium">{evaluation.evaluatorName}</p>
                      {evaluation.result && (<Tag color={getResultColor(evaluation.result)}>{evaluation.result}</Tag>)}
                    </div>
                    <p className="mt-1">{evaluation.comments}</p>
                    {evaluation.result ? (
                      <p className="text-xs text-gray-500 mt-1">
                        Submitted: {new Date(evaluation.createdAt).toLocaleString()}
                      </p>
                    ) :
                    (
                      <p className="text-xs text-gray-500 mt-1">
                        There is no evaluation be given yet.
                      </p>
                    )}
                  </div>
                ))
              ) : (
                <p className="text-gray-500">No co-evaluator feedback available yet.</p>
              )}
            </div>
            
            {/* Evaluation History Section */}
            {evaluationHistory.length > 0 && (
              <div className="mb-6">
                <h3 className="text-lg font-semibold mb-2">Submission History</h3>
                <div className="bg-gray-100 border border-gray-300 rounded-lg p-4">
                  {evaluationHistory.map(evaluation => (
                    <div key={evaluation.evaluationId} className="bg-white p-3 rounded mb-3 border-l-4 border-gray-400">
                      <div className="flex justify-between items-start mb-2">
                        <p className="font-medium text-gray-700">{evaluation.evaluatorName}</p>
                        <div className="flex items-center gap-2">
                          <Tag color="gray">Historical</Tag>
                          {evaluation.result && (
                            <Tag color={getResultColor(evaluation.result)}>{evaluation.result}</Tag>
                          )}
                        </div>
                      </div>
                      {evaluation.comments && (
                        <p className="text-gray-600 mb-2">{evaluation.comments}</p>
                      )}
                      <p className="text-xs text-gray-500">
                        Submitted: {new Date(evaluation.createdAt).toLocaleString()}
                      </p>
                    </div>
                  ))}
                </div>
              </div>
            )}
            
            {/* Evaluation Form */}
            <div className="border-t pt-4">
              <h3 className="text-lg font-semibold mb-4">Submit Your Evaluation</h3>
              
              {/* Re-evaluation Lock Alert */}
              {isEvaluationLocked(myEvaluation) && (
                <Alert 
                  message="You have already submitted your evaluation for this proposal. Re-evaluation is not allowed." 
                  type="info" 
                  className="mb-4"
                />
              )}
              
              <Form layout="vertical">
                <Form.Item label="Result" required>
                  <Select
                    value={evaluationResult}
                    onChange={setEvaluationResult}
                    placeholder="Select evaluation result"
                    disabled={!selectedProposal.supervisorComment || isEvaluationLocked(myEvaluation)}
                  >
                    <Option value="Accepted">Accepted</Option>
                    <Option value="Accepted with Conditions">Accepted with Conditions</Option>
                    <Option value="Rejected">Rejected</Option>
                  </Select>
                </Form.Item>
                
                <Form.Item label="Comments" required>
                  <TextArea
                    rows={4}
                    value={evaluationComments}
                    onChange={e => setEvaluationComments(e.target.value)}
                    placeholder="Provide detailed feedback on the proposal"
                    disabled={!selectedProposal.supervisorComment || isEvaluationLocked(myEvaluation)}
                  />
                </Form.Item>
                
                <Form.Item>
                  <button
                      type="submit"
                      onClick={handleSubmitEvaluation}
                      loading={submitting}
                      disabled={!selectedProposal.supervisorComment || isEvaluationLocked(myEvaluation)}
                      className="w-1/8 px-4 py-2 bg-teal-600 text-white rounded-md hover:bg-teal-700 focus:outline-none focus:ring-2 focus:ring-teal-500 focus:ring-offset-2 disabled:opacity-50 disabled:cursor-not-allowed"
                      >
                      Submit Evaluation
                  </button>
                </Form.Item>
                
                {!selectedProposal.supervisorComment && (
                  <Alert 
                    message="Awaiting supervisor's initial review before evaluation can proceed." 
                    type="warning" 
                  />
                )}
              </Form>
            </div>
          </div>
        ) : (
          <div className="flex justify-center my-12">
            <Spin size="large" />
          </div>
        )}
      </Modal>
    </div>
  );
};

// Helper function to get color for result tag
const getResultColor = (result) => {
  switch (result) {
    case 'Accepted':
      return 'green';
    case 'Accepted with Conditions':
      return 'orange';
    case 'Rejected':
      return 'red';
    default:
      return 'blue';
  }
};

// Helper function to check if evaluation is locked
const isEvaluationLocked = (myEvaluation) => {
  return myEvaluation && myEvaluation.result;
};

export default MyAssignments;