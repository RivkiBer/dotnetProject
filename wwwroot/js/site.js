const uri = '/Bakery';
let Pastrys = [];
let token = localStorage.getItem('token');
let connection; // Signaction
let currentUserData = {}; // Store current user info

if (!token) {
    window.location.href = 'login.html';
} else {
    // Parse user data from token
    try {
        const tokenParts = token.split('.');
        const decodedPayload = atob(tokenParts[1]);
        currentUserData = JSON.parse(decodedPayload);
    } catch (e) {
        console.error('Error parsing token:', e);
    }
    
    initializeSignalR();
    getItems();
}

function showMessage(action, itemName, username, userType, isOwnAction) {
    const container = document.getElementById('messagesContainer');
    container.style.display = 'block';
    
    const actionEmoji = {
        'add': '✅',
        'update': '✏️',
        'delete': '🗑️'
    }[action] || action;
    
    const actionVerb = {
        'add': 'הוסיף/ה',
        'update': 'עדכן/ה',
        'delete': 'מחק/ה'
    }[action] || action;
    
    let messageText = '';
    
    
    // הצג את שם המשתמש, הפעולה, ושם המאפה
    messageText = `${actionEmoji} <strong>${username}</strong> ${actionVerb}  ${itemName}`;
    
    const msgClass = `message-${action}`;
    const msgHTML = `
        <div class="message-item ${msgClass}">
            <span>${messageText}</span>
            <button class="message-close" onclick="this.parentElement.remove()">✕</button>
        </div>
    `;
    
    container.innerHTML += msgHTML;
    
    //  ההודעה אחרי 5 שניות
    setTimeout(() => {
        const messages = container.querySelectorAll('.message-item');
        if (messages.length > 0) {
            messages[0].style.animation = 'slideUp 0.3s ease';
            setTimeout(() => messages[0].remove(), 300);
        }
    }, 5000);
}

// SignalR 
function initializeSignalR() {
    connection = new signalR.HubConnectionBuilder()
        .withUrl('/itemsHub', { accessTokenFactory: () => token })
        .withAutomaticReconnect()
        .build();

    connection.on('ReceiveItemUpdate', (message) => {
        console.log('📡 עדכון בזמן אמת מתקבל:', message);
        
        // בדוק אם זו יוזר עצמאי או משתמש אחר
        const isOwnAction = message.UserId === currentUserData.userid;
        
        // הצג ההודעה עם טיים משתמש
        showMessage(message.Action, message.ItemName, message.Username, message.UserType, isOwnAction);
        
        // עדכן את הרשת אוטומטית
        getItems();
    });

    connection.start()
        .then(() => console.log('✅ חיבור SignalR הוקם בהצלחה'))
        .catch(err => console.error('❌ שגיאה בהחיבור SignalR:', err));
}

function getAuthHeaders() {
    return { 'Authorization': `Bearer ${token}` };
}

function logout() {
    localStorage.removeItem('token');
    window.location.href = 'login.html';
}

function getItems() {
    fetch(uri, {
        headers: getAuthHeaders()
    })
        .then(response => response.json())
        .then(data => _displayItems(data))
        .catch(error => console.error('Unable to get items.', error));
}

function addItem() {
    const addNameTextbox = document.getElementById('add-name');

    const item = {
        isMilky: document.getElementById('add-isMilky').checked,
        name: addNameTextbox.value.trim()
    };

    if (!item.name) {
        alert('אנא הכנס שם המאפה');
        return;
    }

    fetch(uri, {
            method: 'POST',
            headers: {
                'Accept': 'application/json',
                'Content-Type': 'application/json',
                ...getAuthHeaders()
            },
            body: JSON.stringify(item)
        })
        .then(response => {
            if (!response.ok) {
                throw new Error('שגיאה בהוספת המאפה');
            }
            return response.json();
        })
        .then(() => {
            // הצג הודעה מיידית
            showMessage('add', item.name, currentUserData.username, currentUserData.type, true);
            
            getItems();
            addNameTextbox.value = '';
        })
        .catch(error => {
            console.error('Unable to add item.', error);
            alert('❌ ' + error.message);
        });
}

function deleteItem(id) {
    // Get the item name before deleting
    const item = Pastrys.find(item => item.id === id);
    const itemName = item ? item.name : '';
    
    fetch(`${uri}/${id}`, {
            method: 'DELETE',
            headers: getAuthHeaders()
        })
        .then(response => {
            if (!response.ok) {
                throw new Error('שגיאה במחיקת המאפה');
            }
            // הצג הודעה מיידית
            showMessage('delete', itemName, currentUserData.username, currentUserData.type, true);
            
            return getItems();
        })
        .catch(error => {
            console.error('Unable to delete item.', error);
            alert('❌ ' + error.message);
        });
}

function displayEditForm(id) {
    const item = Pastrys.find(item => item.id === id);

    document.getElementById('edit-name').value = item.name;
    document.getElementById('edit-id').value = item.id;
    document.getElementById('edit-isMilky').checked = item.isMilky;
    document.getElementById('editForm').style.display = 'block';
}

function updateItem() {
    const itemId = document.getElementById('edit-id').value;
    const item = {
        id: parseInt(itemId, 10),
        isMilky: document.getElementById('edit-isMilky').checked,
        name: document.getElementById('edit-name').value.trim()
    };

    if (!item.name) {
        alert('אנא הכנס שם המאפה');
        return;
    }

    fetch(`${uri}/${itemId}`, {
            method: 'PUT',
            headers: {
                'Accept': 'application/json',
                'Content-Type': 'application/json',
                ...getAuthHeaders()
            },
            body: JSON.stringify(item)
        })
        .then(response => {
            if (!response.ok) {
                throw new Error('שגיאה בעדכון המאפה');
            }
            // Show message immediately
            showMessage('update', item.name, currentUserData.username, currentUserData.type, true);
            
            return getItems();
        })
        .catch(error => {
            console.error('Unable to update item.', error);
            alert('❌ ' + error.message);
        });

    closeInput();

    return false;
}

function closeInput() {
    document.getElementById('editForm').style.display = 'none';
}

function _displayCount(itemCount) {
    const name = (itemCount === 1) ? 'מאפה' : 'מאפים';
    document.getElementById('counter').innerText = `📊 סה"כ: ${itemCount} ${name}`;
}

function _displayItems(data) {
    const tBody = document.getElementById('Pastry');
    tBody.innerHTML = '';

    _displayCount(data.length);

    data.forEach(item => {
        let tr = tBody.insertRow();

        // עמודה 1: חלבי
        let td1 = tr.insertCell(0);
        let isMilkyCheckbox = document.createElement('input');
        isMilkyCheckbox.type = 'checkbox';
        isMilkyCheckbox.disabled = true;
        isMilkyCheckbox.checked = item.isMilky;
        td1.appendChild(isMilkyCheckbox);
        td1.style.textAlign = 'center';

        // עמודה 2: שם
        let td2 = tr.insertCell(1);
        td2.textContent = item.name;

        // עמודה 3: עריכה
        let td3 = tr.insertCell(2);
        let editButton = document.createElement('button');
        editButton.className = 'btn-edit';
        editButton.innerText = '✏️ עדכן';
        editButton.onclick = () => displayEditForm(item.id);
        td3.appendChild(editButton);

        // עמודה 4: מחיקה
        let td4 = tr.insertCell(3);
        let deleteButton = document.createElement('button');
        deleteButton.className = 'btn-delete';
        deleteButton.innerText = '🗑️ מחק';
        deleteButton.onclick = () => {
            if (confirm('האם אתה בטוח שאתה רוצה למחוק את המאפה הזה?')) {
                deleteItem(item.id);
            }
        };
        td4.appendChild(deleteButton);
    });

    Pastrys = data;
}