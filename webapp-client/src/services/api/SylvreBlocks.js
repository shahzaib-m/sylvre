import api from './api';

const createSylvreBlock = async function(name, body) {
    const response = await api.auth.post('/sylvreblocks', {
        name: name,
        body: body
    });

    return response.data;
}

const getAllSylvreBlocks = async function(noBody=true) {
    const response = await api.auth.get('/sylvreblocks', {
        params: { noBody: noBody }
    });

    return response.data;
}

const getSylvreBlockById = async function(id) {
    const response = await api.auth.get(`/sylvreblocks/${id}`);

    return response.data;
}

const updateSylvreBlockById = async function(id, name=null, body=null) {
    await api.auth.put(`/sylvreblocks/${id}`, {
        name: name,
        body: body
    });

    return;
}

const deleteSylvreBlockById = async function(id) {
    await api.auth.delete(`/sylvreblocks/${id}`);

    return;
}

const getAllSampleSylvreBlocks = async function(noBody=true) {
    const response = await api.guest.get('/sylvreblocks/samples', {
        params: { noBody: noBody }
    });

    return response.data;
}

const getSampleSylvreBlockById = async function(id) {
    const response = await api.guest.get(`/sylvreblocks/samples/${id}`);

    return response.data;
}

export default {
    createSylvreBlock: createSylvreBlock,
    getAllSylvreBlocks: getAllSylvreBlocks,
    getSylvreBlockById: getSylvreBlockById,
    updateSylvreBlockById: updateSylvreBlockById,
    deleteSylvreBlockById: deleteSylvreBlockById,
    getAllSampleSylvreBlocks: getAllSampleSylvreBlocks,
    getSampleSylvreBlockById: getSampleSylvreBlockById
}