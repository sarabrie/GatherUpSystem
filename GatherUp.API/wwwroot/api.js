//// שורה 1 המעודכנת:
//// שורה 1 המעודכנת (התיקון):
//const API_BASE = 'https://localhost:7042/api';
//function saveToken(token) { localStorage.setItem('token', token); }
//function getToken()       { return localStorage.getItem('token'); }
//function clearToken()     { localStorage.removeItem('token'); localStorage.removeItem('currentEventId'); }

//function authHeaders() {
//    return { 'Content-Type': 'application/json', 'Authorization': `Bearer ${getToken()}` };
//}

//// redirect to login if not authenticated
//function requireAuth() {
//    if (!getToken()) { location.href = 'login.html'; }
//}

//// ── Generic fetch wrapper ──────────────────────────────────
//async function apiCall(method, path, body = null, auth = true) {
//    const opts = { method, headers: auth ? authHeaders() : { 'Content-Type': 'application/json' } };
//    if (body) opts.body = JSON.stringify(body);
//    const res = await fetch(API_BASE + path, opts);
//    if (res.status === 401 && !path.includes('/auth/login')) { clearToken(); location.href = 'login.html'; return; }
//    const text = await res.text();
//    let data = {};
//    try { data = text ? JSON.parse(text) : {}; } catch { data = { error: text }; }
//    if (!res.ok) throw new Error(data.error || text || res.statusText);
//    return data;
//}

//// ── Auth ───────────────────────────────────────────────────
//async function login(email, id) {
//    const data = await apiCall('POST', '/auth/login', { email, id }, false);
//    saveToken(data.token);
//    return data;
//}

//async function register(name, email, nationalId) {
//    return apiCall('POST', '/person/register', { id: nationalId, name, email }, false);
//}

//// ── Events ─────────────────────────────────────────────────
//async function getMyEvents()                    { return apiCall('GET',  '/events/my-events'); }
//async function getEventDetails(eventId)         { return apiCall('GET',  `/events/${eventId}`); }
//async function createEvent(eventObj)            { return apiCall('POST', '/events/create', eventObj); }
//async function editEvent(eventId, eventObj)     { return apiCall('PUT',  `/events/${eventId}/edit`, eventObj); }

//// ── Participants ────────────────────────────────────────────
//async function getParticipants(eventId)         { return apiCall('GET',  `/participants/event/${eventId}`); }
//async function addParticipant(eventId, p)       { return apiCall('POST', `/participants/event/${eventId}`, p); }
//async function sendReminder(eventId, type)      { return apiCall('POST', `/participants/event/${eventId}/remind?reminderType=${encodeURIComponent(type)}`); }

//// ── Polls ──────────────────────────────────────────────────
//async function getEventPolls(eventId)           { return apiCall('GET',  `/polls/event/${eventId}`); }
//async function createPoll(eventId, poll)        { return apiCall('POST', `/polls/event/${eventId}`, poll); }
//async function getPollResults(pollId, qId)      { return apiCall('GET',  `/polls/${pollId}/questions/${qId}/results`); }

//// ── Finance ────────────────────────────────────────────────
//async function getSuppliers(eventId)            { return apiCall('GET',  `/financial/${eventId}/suppliers`); }
//async function addSupplier(eventId, vendor)     { return apiCall('POST', `/financial/${eventId}/suppliers`, vendor); }
//async function getReceiptsReport(eventId)       { return apiCall('GET',  `/financial/${eventId}/receipts-report`); }
//async function getFinancialStatus(eventId)      { return apiCall('GET',  `/financial/${eventId}/status`); }
//async function addReceipt(receipt)              { return apiCall('POST', `/financial/receipts`, receipt); }

//// ── UI helpers ─────────────────────────────────────────────
//function showError(msg) {
//    let el = document.getElementById('error-msg');
//    if (!el) { el = Object.assign(document.createElement('div'), { id: 'error-msg' }); Object.assign(el.style, { position:'fixed', bottom:'24px', left:'50%', transform:'translateX(-50%)', background:'#e74c3c', color:'white', padding:'12px 24px', borderRadius:'10px', fontWeight:'600', zIndex:'999', fontSize:'14px' }); document.body.appendChild(el); }
//    el.textContent = msg;
//    el.style.display = 'block';
//    setTimeout(() => el.style.display = 'none', 3500);
//}

//function showSuccess(msg) {
//    let el = document.getElementById('success-msg');
//    if (!el) { el = Object.assign(document.createElement('div'), { id: 'success-msg' }); Object.assign(el.style, { position:'fixed', bottom:'24px', left:'50%', transform:'translateX(-50%)', background:'#2ecc71', color:'white', padding:'12px 24px', borderRadius:'10px', fontWeight:'600', zIndex:'999', fontSize:'14px' }); document.body.appendChild(el); }
//    el.textContent = msg;
//    el.style.display = 'block';
//    setTimeout(() => el.style.display = 'none', 3000);
//}
const API_BASE = 'https://localhost:7042/api';

function saveToken(token) { localStorage.setItem('token', token); }
function getToken() { return localStorage.getItem('token'); }
function clearToken() { localStorage.removeItem('token'); localStorage.removeItem('currentEventId'); }

function authHeaders() {
    return { 'Content-Type': 'application/json', 'Authorization': `Bearer ${getToken()}` };
}

// redirect to login if not authenticated
function requireAuth() {
    if (!getToken()) { location.href = 'login.html'; }
}

// ── Generic fetch wrapper ──────────────────────────────────
async function apiCall(method, path, body = null, auth = true) {
    const opts = { method, headers: auth ? authHeaders() : { 'Content-Type': 'application/json' } };
    if (body) opts.body = JSON.stringify(body);

    const res = await fetch(API_BASE + path, opts);

    // מונע זריקה לעמוד לוגין אם השגיאה (401) הגיעה מתוך ניסיון התחברות כושל
    if (res.status === 401 && !path.includes('/auth/login')) {
        clearToken();
        location.href = 'login.html';
        return;
    }

    const text = await res.text();
    let data = {};

    // ניסיון פארס חכם - אם השרת החזיר טקסט רגיל, הקוד לא יישבר
    try {
        data = text ? JSON.parse(text) : {};
    } catch {
        data = { error: text };
    }

    if (!res.ok) {
        // מחלץ את השגיאה בצורה הטובה ביותר: מה-JSON, מהטקסט הגולמי, או מסטטוס ה-HTTP
        let errorMsg = data.error || text || res.statusText;
        throw new Error(errorMsg);
    }

    return data;
}

// ── Auth ───────────────────────────────────────────────────
async function login(email, id) {
    const data = await apiCall('POST', '/auth/login', { email, id }, false);
    saveToken(data.token);
    return data;
}

async function register(name, email, nationalId) {
    return apiCall('POST', '/person/register', { id: nationalId, name, email }, false);
}

async function getAllPersons() {
    return apiCall('GET', '/person/all');
}

// ── Events ─────────────────────────────────────────────────
async function getMyEvents() { return apiCall('GET', '/events/my-events'); }
async function getEventDetails(eventId) { return apiCall('GET', `/events/${eventId}`); }
async function createEvent(createEventRequest) { return apiCall('POST', '/events/create', createEventRequest); }
async function editEvent(eventId, eventObj) { return apiCall('PUT', `/events/${eventId}/edit`, eventObj); }

// ── Participants ────────────────────────────────────────────
async function getParticipants(eventId) { return apiCall('GET', `/participants/event/${eventId}`); }
async function addParticipant(eventId, p) { return apiCall('POST', `/participants/event/${eventId}`, p); }
async function sendReminder(eventId, type) { return apiCall('POST', `/participants/event/${eventId}/remind?reminderType=${encodeURIComponent(type)}`); }

// ── Polls ──────────────────────────────────────────────────
async function getEventPolls(eventId) { return apiCall('GET', `/polls/event/${eventId}`); }
async function createPoll(eventId, poll) { return apiCall('POST', `/polls/event/${eventId}`, poll); }
async function getPollResults(pollId, qId) { return apiCall('GET', `/polls/${pollId}/questions/${qId}/results`); }

// ── Finance ────────────────────────────────────────────────
async function getSuppliers(eventId) { return apiCall('GET', `/financial/${eventId}/suppliers`); }
async function addSupplier(eventId, vendor) { return apiCall('POST', `/financial/${eventId}/suppliers`, vendor); }
async function getReceiptsReport(eventId) { return apiCall('GET', `/financial/${eventId}/receipts-report`); }
async function getFinancialStatus(eventId) { return apiCall('GET', `/financial/${eventId}/status`); }
async function addReceipt(receipt) { return apiCall('POST', `/financial/receipts`, receipt); }

// ── UI helpers ─────────────────────────────────────────────
function showError(msg) {
    let el = document.getElementById('error-msg');
    if (!el) { el = Object.assign(document.createElement('div'), { id: 'error-msg' }); Object.assign(el.style, { position: 'fixed', bottom: '24px', left: '50%', transform: 'translateX(-50%)', background: '#e74c3c', color: 'white', padding: '12px 24px', borderRadius: '10px', fontWeight: '600', zIndex: '999', fontSize: '14px' }); document.body.appendChild(el); }
    el.textContent = msg;
    el.style.display = 'block';
    setTimeout(() => el.style.display = 'none', 3500);
}

function showSuccess(msg) {
    let el = document.getElementById('success-msg');
    if (!el) { el = Object.assign(document.createElement('div'), { id: 'success-msg' }); Object.assign(el.style, { position: 'fixed', bottom: '24px', left: '50%', transform: 'translateX(-50%)', background: '#2ecc71', color: 'white', padding: '12px 24px', borderRadius: '10px', fontWeight: '600', zIndex: '999', fontSize: '14px' }); document.body.appendChild(el); }
    el.textContent = msg;
    el.style.display = 'block';
    setTimeout(() => el.style.display = 'none', 3000);
}