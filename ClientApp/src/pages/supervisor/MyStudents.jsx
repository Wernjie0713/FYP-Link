import React, { useState, useEffect } from 'react';
import apiClient from '../../api/apiClient';
import { Button, Table, Select, Modal, Form, Input, message } from 'antd';
import { EyeOutlined } from '@ant-design/icons';

const { TextArea } = Input;

const MyStudents = () => {
    const [students, setStudents] = useState([]);
    const [loading, setLoading] = useState(false);
    const [modalVisible, setModalVisible] = useState(false);
    const [selectedProposal, setSelectedProposal] = useState(null);
    const [comment, setComment] = useState('');
    const [filters, setFilters] = useState({
        academicSession: undefined,
        semester: undefined
    });

    const fetchStudents = async () => {
        try {
            setLoading(true);
            const response = await apiClient.get('/api/supervisor/my-students');
            setStudents(response.data);
        } catch (error) {
            message.error('Failed to fetch students');
        } finally {
            setLoading(false);
        }
    };

    const fetchProposalDetails = async (proposalId) => {
        try {
            const response = await apiClient.get(`/api/supervisor/proposals/${proposalId}`);
            setSelectedProposal(response.data);
            setComment(response.data.supervisorComment || '');
        } catch (error) {
            message.error('Failed to fetch proposal details');
        }
    };

    const handleSubmitComment = async () => {
        try {
            await apiClient.post(`/api/supervisor/proposals/${selectedProposal.id}/comment`, {
                comment: comment
            });
            message.success('Comment submitted successfully');
            setModalVisible(false);
            fetchStudents(); // Refresh the list
        } catch (error) {
            message.error('Failed to submit comment');
        }
    };

    useEffect(() => {
        fetchStudents();
    }, []);

    const columns = [
        {
            title: 'Name',
            dataIndex: 'name',
            key: 'name',
        },
        {
            title: 'Matric Number',
            dataIndex: 'matricNumber',
            key: 'matricNumber',
        },
        {
            title: 'Proposal Title',
            dataIndex: ['proposal', 'title'],
            key: 'proposalTitle',
        },
        {
            title: 'Academic Session',
            dataIndex: ['proposal', 'academicSession'],
            key: 'academicSession',
        },
        {
            title: 'Semester',
            dataIndex: ['proposal', 'semester'],
            key: 'semester',
        },
        {
            title: 'Actions',
            key: 'actions',
            render: (_, record) => (
                <Button 
                    icon={<EyeOutlined />}
                    onClick={() => {
                        fetchProposalDetails(record.proposal?.id);
                        setModalVisible(true);
                    }}
                    disabled={!record.proposal}
                >
                    Review Proposal
                </Button>
            ),
        },
    ];

    const filteredStudents = students.filter(student => {
        if (filters.academicSession && student.proposal?.academicSession !== filters.academicSession) {
            return false;
        }
        if (filters.semester && student.proposal?.semester !== filters.semester) {
            return false;
        }
        return true;
    });

    const uniqueSessions = [...new Set(students.map(s => s.proposal?.academicSession).filter(Boolean))];
    const uniqueSemesters = [...new Set(students.map(s => s.proposal?.semester).filter(Boolean))];

    return (
        <div className="p-6">
            <h1 className="text-2xl font-bold mb-6">My Students</h1>
            
            <div className="flex gap-4 mb-6">
                <Select
                    placeholder="Select Academic Session"
                    allowClear
                    style={{ width: 200 }}
                    onChange={(value) => setFilters(prev => ({ ...prev, academicSession: value }))}
                    options={uniqueSessions.map(session => ({ value: session, label: session }))}
                />
                <Select
                    placeholder="Select Semester"
                    allowClear
                    style={{ width: 200 }}
                    onChange={(value) => setFilters(prev => ({ ...prev, semester: value }))}
                    options={uniqueSemesters.map(sem => ({ value: sem, label: `Semester ${sem}` }))}
                />
            </div>

            <Table 
                columns={columns} 
                dataSource={filteredStudents}
                loading={loading}
                rowKey="matricNumber"
            />

            <Modal
                title="Review Proposal"
                open={modalVisible}
                onCancel={() => setModalVisible(false)}
                footer={null}
                width={800}
            >
                {selectedProposal && (
                    <div className="space-y-6">
                        <div>
                            <h2 className="text-xl font-semibold mb-2">{selectedProposal.title}</h2>
                            <p className="text-gray-600 mb-2">
                                <strong>Student:</strong> {selectedProposal.student.name} ({selectedProposal.student.matricNumber})
                            </p>
                            <p className="text-gray-600 mb-4">
                                <strong>Project Type:</strong> {selectedProposal.projectType}
                            </p>
                            <div className="bg-gray-50 p-4 rounded">
                            <h3 className="font-semibold mb-2">Abstract</h3>
                            <p>{selectedProposal.abstract}</p>
                        </div>
                        
                        {selectedProposal.pdfFilePath && (
                            <div className="mb-4">
                                <strong>Uploaded Document:</strong>
                                <a
                                    href={`${import.meta.env.VITE_API_BASE_URL}${selectedProposal.pdfFilePath}`}
                                    target="_blank"
                                    rel="noopener noreferrer"
                                    className="text-blue-600 hover:underline ml-2"
                                >
                                    View PDF
                                </a>
                            </div>
                        )}
                        </div>

                        <div className="border-t pt-6">
                            <h3 className="font-semibold mb-4">Supervisor's Comment</h3>
                            <Form layout="vertical">
                                <Form.Item>
                                    <TextArea 
                                        rows={4} 
                                        value={comment}
                                        onChange={(e) => setComment(e.target.value)}
                                        placeholder="Enter your comments here..."
                                    />
                                </Form.Item>
                                <Form.Item>
                                    <button
                                        type="submit"
                                        onClick={handleSubmitComment}
                                        className="w-1/8 px-4 py-2 bg-teal-600 text-white rounded-md hover:bg-teal-700 focus:outline-none focus:ring-2 focus:ring-teal-500 focus:ring-offset-2 disabled:opacity-50 disabled:cursor-not-allowed"
                                        >
                                        Submit Comment
                                    </button>
                                </Form.Item>
                            </Form>
                        </div>

                        {selectedProposal.evaluations?.length > 0 && (
                            <div className="border-t pt-6">
                                <h3 className="font-semibold mb-4">Final Evaluations</h3>
                                {selectedProposal.evaluations.map((evaluation, index) => (
                                    <div key={evaluation.id} className="bg-gray-50 p-4 rounded mb-4">
                                        <p className="font-medium">
                                            Evaluator {index + 1}: {evaluation.evaluator.name}
                                        </p>
                                        <p className="text-gray-600 mb-2">
                                            <strong>Result:</strong> {evaluation.result}
                                        </p>
                                        <p className="text-gray-600">
                                            <strong>Comments:</strong> {evaluation.comments}
                                        </p>
                                    </div>
                                ))}
                            </div>
                        )}
                    </div>
                )}
            </Modal>
        </div>
    );
};

export default MyStudents;