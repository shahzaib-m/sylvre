import api from './api';

const getUsernameAvailability = async function(username) {
    const response = await api.guest.get(`/availability/username/${username}`);

    return response.data;
}

const getEmailAvailability = async function(email) {
    const response = await api.guest.get(`/availability/email/${email}`);

    return response.data;
}

export default {
    getUsernameAvailability: getUsernameAvailability,
    getEmailAvailability: getEmailAvailability,
}