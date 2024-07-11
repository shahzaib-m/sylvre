import api from './api';

const transpileCode = async function(code, target) {
    const response = await api.guest.post('/transpiler', {
        code: code
    }, {
        params: { target: target }
    });

    return response.data;
}

export default {
    transpileCode: transpileCode
}