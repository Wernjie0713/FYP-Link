import React, { useState, useEffect } from 'react';
import apiClient from '../../api/apiClient';
import { toast } from 'react-hot-toast';
import ConfirmationModal from '../../components/common/ConfirmationModal';

const ManageCommittee = () => {
    const [programs, setPrograms] = useState([]);
    const [selectedProgram, setSelectedProgram] = useState('');
    const [committeeMembers, setCommitteeMembers] = useState([]);
    const [availableLecturers, setAvailableLecturers] = useState([]);
    const [modalOpen, setModalOpen] = useState(false);
    const [selectedLecturer, setSelectedLecturer] = useState(null);

    useEffect(() => {
        loadPrograms();
    }, []);

    useEffect(() => {
        if (selectedProgram) {
            loadCommitteeMembers();
            loadAvailableLecturers();
        }
    }, [selectedProgram]);

    const loadPrograms = async () => {
        try {
            const response = await apiClient.get('/api/academicprograms');
            setPrograms(response.data);
        } catch (error) {
            toast.error('Failed to load academic programs');
        }
    };

    const loadCommitteeMembers = async () => {
        try {
            const response = await apiClient.get(`/api/committee/${selectedProgram}/members`);
            setCommitteeMembers(response.data);
        } catch (error) {
            toast.error('Failed to load committee members');
        }
    };

    const loadAvailableLecturers = async () => {
        try {
            const response = await apiClient.get('/api/committee/available-lecturers');
            setAvailableLecturers(response.data);
        } catch (error) {
            toast.error('Failed to load available lecturers');
        }
    };

    const handleAddMember = async (lecturerId) => {
        try {
            await apiClient.post('/api/committee/members', {
                lecturerId: lecturerId,
                academicProgramId: parseInt(selectedProgram)
            });
            toast.success('Lecturer added to committee');
            loadCommitteeMembers();
            loadAvailableLecturers();
        } catch (error) {
            const errorMessage = error.response?.data || 'Failed to add lecturer to committee';
            toast.error(errorMessage);
        }
    };

    const handleRemoveMember = async (lecturerId) => {
        try {
            await apiClient.delete(`/api/committee/members/${lecturerId}/${selectedProgram}`);
            toast.success('Lecturer removed from committee');
            loadCommitteeMembers();
            loadAvailableLecturers();
        } catch (error) {
            toast.error('Failed to remove lecturer from committee');
        }
    };

    const openRemoveModal = (lecturer) => {
        setSelectedLecturer(lecturer);
        setModalOpen(true);
    };

    return (
        <div className="p-6">
            <h1 className="text-3xl font-bold mb-6">Manage Committee</h1>

            <div className="mb-6">
                <label className="block text-sm font-medium text-gray-700 mb-2">
                    Select Academic Program
                </label>
                <select
                    className="w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-indigo-500 focus:border-indigo-500"
                    value={selectedProgram}
                    onChange={(e) => setSelectedProgram(e.target.value)}
                >
                    <option value="">Select a program...</option>
                    {programs.map((program) => (
                        <option key={program.id} value={program.id}>
                            {program.name}
                        </option>
                    ))}
                </select>
            </div>

            {selectedProgram && (
                <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
                    <div className="bg-white p-6 rounded-lg shadow">
                        <h2 className="text-xl font-semibold mb-4">Committee Members</h2>
                        <div className="space-y-4">
                            {committeeMembers.map((lecturer) => (
                                <div key={lecturer.id} className="flex items-center justify-between p-3 bg-gray-50 rounded">
                                    <span>{lecturer.name}</span>
                                    <button
                                        onClick={() => openRemoveModal(lecturer)}
                                        className="px-3 py-1 text-sm text-red-600 hover:text-red-800 hover:bg-red-100 rounded"
                                    >
                                        Remove
                                    </button>
                                </div>
                            ))}
                            {committeeMembers.length === 0 && (
                                <p className="text-gray-500 italic">No committee members assigned</p>
                            )}
                        </div>
                    </div>

                    <div className="bg-white p-6 rounded-lg shadow">
                        <h2 className="text-xl font-semibold mb-4">Available Lecturers</h2>
                        <div className="space-y-4">
                            {availableLecturers.map((lecturer) => (
                                <div key={lecturer.id} className="flex items-center justify-between p-3 bg-gray-50 rounded">
                                    <span>{lecturer.name}</span>
                                    <button
                                        onClick={() => handleAddMember(lecturer.id)}
                                        className="px-3 py-1 text-sm text-green-600 hover:text-green-800 hover:bg-green-100 rounded"
                                    >
                                        Add
                                    </button>
                                </div>
                            ))}
                            {availableLecturers.length === 0 && (
                                <p className="text-gray-500 italic">No available lecturers</p>
                            )}
                        </div>
                    </div>
                </div>
            )}

            <ConfirmationModal
                isOpen={modalOpen}
                onClose={() => setModalOpen(false)}
                onConfirm={() => selectedLecturer && handleRemoveMember(selectedLecturer.id)}
                title="Remove Committee Member"
                message={`Are you sure you want to remove ${selectedLecturer?.name} from the committee? This action cannot be undone.`}
            />
        </div>
    );
};

export default ManageCommittee;