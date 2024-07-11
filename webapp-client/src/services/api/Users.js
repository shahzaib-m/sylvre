import api from './api';

const registerUser = async function(username, password, email, fullName) {
    const response = await api.guest.post('/users', {
        username: username,
        password: password,
        email: email,
        fullName: fullName
    });

    return response.data;
}

const getUserById = async function(userId) {
    const response = await api.auth.get(`/users/${userId}`);

    return response.data;
}

const updateUser = async function(userId, username=null, email=null, fullName=null) {
    await api.auth.put(`/users/${userId}`, {
        username: username,
        email: email,
        fullName: fullName
    });

    return;
}

const deleteUser = async function(userId, base64CurrentPassword) {
    await api.auth.delete(`/users/${userId}`, {
        headers: { 'Sylvre-Reauthenticate-Pass': base64CurrentPassword }
    });

    return;
}

const changeUserPassword = async function(userId, base64CurrentPassword, newPassword) {
    await api.auth.put(`/users/${userId}/password`, {
        newPassword: newPassword
    }, { 
        headers: { 'Sylvre-Reauthenticate-Pass': base64CurrentPassword }
    });

    return;
}

const getUserByIdentity = async function() {
    const response = await api.auth.get('/users/identity');

    return response.data;
}

export default {
    registerUser: registerUser,
    getUserById: getUserById,
    updateUser: updateUser,
    deleteUser: deleteUser,
    changeUserPassword: changeUserPassword,
    getUserByIdentity: getUserByIdentity
}