import api from './api';

const login = async function(usernameOrEmail, password) {
    const response = await api.guest.post('/auth/login', {
        usernameOrEmail: usernameOrEmail,
        password: password
    }, {
        params: { strategy: 'cookie' }
    });

    return response.data;
}

const logout = async function() {
    await api.auth.delete('/auth/logout');
}


export default {
    login: login,
    logout: logout
}