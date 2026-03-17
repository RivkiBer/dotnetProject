const uri = '/user';
let Users = [];
let token = localStorage.getItem('token');

if (!token) {
    window.location.href = 'login.html';
} else {
    checkUserRole();
}

function getAuthHeaders() {
    return {
        'Authorization': `Bearer ${token}`,
        'Content-Type': 'application/json'
    };
}

function checkUserRole() {
    // נסה להביא את רשימת כל המשתמשים (רק Admin יכול)
    fetch(uri, {
        headers: getAuthHeaders()
    })
    .then(response => {
        if (response.ok) {
            // Admin - הצג את adminSection
            document.getElementById('userTitle').textContent = '🔐 אתה מנהל - גישה מלאה לכל המשתמשים';
            document.getElementById('adminSection').style.display = 'block';
            document.getElementById('userSection').style.display = 'none';
            getItems();
        } else if (response.status === 403 || response.status === 401) {
            // Regular User
            document.getElementById('userTitle').textContent = '👤 פרופיל משתמש רגיל';
            document.getElementById('adminSection').style.display = 'none';
            document.getElementById('userSection').style.display = 'block';
            loadMyProfile();
        }
    })
    .catch(error => {
        console.error('Error checking role:', error);
        document.getElementById('adminSection').style.display = 'none';
        document.getElementById('userSection').style.display = 'block';
        loadMyProfile();
    });
}

function getItems() {
    fetch(uri, {
        headers: getAuthHeaders()
    })
        .then(response => response.json())
        .then(data => _displayItems(data))
        .catch(error => {
            console.error('Unable to get items.', error);
            alert('❌ שגיאה בטעינת המשתמשים');
        });
}

function addItem() {
    const addNameTextbox = document.getElementById('add-name');
    const addPassTextbox = document.getElementById('add-pass');
    const addTypeSelect = document.getElementById('add-type');

    if (!addNameTextbox.value.trim() || !addPassTextbox.value.trim()) {
        alert('⚠️ אנא מלא את כל השדות');
        return;
    }

    const item = {
        Name: addNameTextbox.value.trim(),
        Pass: addPassTextbox.value.trim(),
        Type: addTypeSelect.value
    };

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
                throw new Error('שגיאה בהוספת משתמש');
            }
            return response.json();
        })
        .then(() => {
            alert('✅ משתמש נוסף בהצלחה');
            getItems();
            addNameTextbox.value = '';
            addPassTextbox.value = '';
            addTypeSelect.value = 'Regular';
        })
        .catch(error => {
            console.error('Unable to add item.', error);
            alert('❌ ' + error.message);
        });
}

function deleteItem(id) {
    fetch(`${uri}/${id}`, {
            method: 'DELETE',
            headers: getAuthHeaders()
        })
        .then(response => {
            if (!response.ok) {
                throw new Error('שגיאה במחיקת משתמש');
            }
            return getItems();
        })
        .catch(error => {
            console.error('Unable to delete item.', error);
            alert('❌ ' + error.message);
        });
}

function displayEditForm(id) {
    const item = Users.find(item => item.id === id);

    if (item) {
        document.getElementById('edit-name').value = item.name;
        document.getElementById('edit-pass').value = item.pass;
        document.getElementById('edit-type').value = item.type;
        document.getElementById('edit-id').value = item.id;
        document.getElementById('editForm').style.display = 'block';
        // גלול לעמוד החלקו
        document.getElementById('editForm').scrollIntoView({ behavior: 'smooth', block: 'nearest' });
    }
}

function updateItem() {
    const itemId = document.getElementById('edit-id').value;

    if (!document.getElementById('edit-name').value.trim() || !document.getElementById('edit-pass').value.trim()) {
        alert('אנא מלא את כל השדות');
        return false;
    }

    const item = {
        Id: parseInt(itemId, 10),
        Name: document.getElementById('edit-name').value.trim(),
        Pass: document.getElementById('edit-pass').value.trim(),
        Type: document.getElementById('edit-type').value
    };

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
                if (response.status === 401) {
                    throw new Error('אינך מורשה לעדכן משתמש זה');
                }
                throw new Error('שגיאה בעדכון משתמש');
            }
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

function logout() {
    localStorage.removeItem('token');
    window.location.href = 'login.html';
}

function _displayCount(itemCount) {
    const name = (itemCount === 1) ? 'משתמש' : 'משתמשים';
    document.getElementById('counter').textContent = `📊 סה"כ: ${itemCount} ${name}`;
}

function _displayItems(data) {
    const tBody = document.getElementById('User');
    tBody.innerHTML = '';

    _displayCount(data.length);

    data.forEach(item => {
        let tr = document.createElement('tr');

        // עמודה 1: ID
        let idCell = document.createElement('td');
        idCell.textContent = item.id;
        idCell.style.fontWeight = '600';
        idCell.style.color = '#667eea';
        tr.appendChild(idCell);

        // עמודה 2: שם
        let nameCell = document.createElement('td');
        nameCell.textContent = item.name;
        nameCell.style.fontWeight = '500';
        tr.appendChild(nameCell);

        // עמודה 3: סיסמה (מוסתרת)
        let passCell = document.createElement('td');
        passCell.innerHTML = `<code style="background: #f0f4ff; padding: 4px 8px; border-radius: 4px; font-size: 0.9em;">••••••••</code>`;
        tr.appendChild(passCell);

        // עמודה 4: סוג
        let typeCell = document.createElement('td');
        typeCell.innerHTML = item.type === 'Admin' 
            ? '<span style="background: #667eea; color: white; padding: 4px 12px; border-radius: 4px; font-weight: 600; font-size: 0.9em;">👨‍💼 מנהל</span>'
            : '<span style="background: #e0e7ff; color: #667eea; padding: 4px 12px; border-radius: 4px; font-weight: 600; font-size: 0.9em;">👤 רגיל</span>';
        typeCell.style.textAlign = 'center';
        tr.appendChild(typeCell);

        // עמודה 5: עריכה
        let editCell = document.createElement('td');
        let editButton = document.createElement('button');
        editButton.className = 'btn-edit';
        editButton.innerHTML = '✏️';
        editButton.style.width = 'auto';
        editButton.style.padding = '6px 10px';
        editButton.style.fontSize = '0.9em';
        editButton.title = 'עדכן משתמש';
        editButton.onclick = () => displayEditForm(item.id);
        editCell.appendChild(editButton);
        tr.appendChild(editCell);

        // עמודה 6: מחיקה
        let deleteCell = document.createElement('td');
        let deleteButton = document.createElement('button');
        deleteButton.className = 'btn-delete';
        deleteButton.innerHTML = '🗑️';
        deleteButton.style.width = 'auto';
        deleteButton.style.padding = '6px 10px';
        deleteButton.style.fontSize = '0.9em';
        deleteButton.title = 'מחק משתמש';
        deleteButton.onclick = () => {
            if (confirm(`⚠️ האם אתה בטוח שאתה רוצה למחוק את המשתמש "${item.name}"?`)) {
                deleteItem(item.id);
            }
        };
        deleteCell.appendChild(deleteButton);
        tr.appendChild(deleteCell);

        tBody.appendChild(tr);
    });

    Users = data;
}

function loadMyProfile() {
    fetch(`${uri}/me`, {
        headers: getAuthHeaders()
    })
    .then(response => {
        if (!response.ok) {
            throw new Error('שגיאה בטעינת הפרופיל');
        }
        return response.json();
    })
    .then(user => {
        const profileDiv = document.getElementById('myProfile');
        profileDiv.innerHTML = `
            <div style="display: grid; gap: 15px;">
                <div>
                    <span style="font-weight: 600; color: #667eea;">🆔 ID:</span>
                    <span style="margin-left: 10px; color: #333;">${user.id}</span>
                </div>
                <div>
                    <span style="font-weight: 600; color: #667eea;">👤 שם:</span>
                    <span style="margin-left: 10px; color: #333;">${user.name}</span>
                </div>
                <div>
                    <span style="font-weight: 600; color: #667eea;">👨 סוג:</span>
                    <span style="margin-left: 10px; padding: 4px 12px; background: #e0e7ff; color: #667eea; border-radius: 4px; font-weight: 600;">👤 משתמש רגיל</span>
                </div>
                <button onclick="showMyEditForm()" class="btn-edit" style="max-width: 200px; margin-top: 10px;">✏️ עדכן פרטים</button>
            </div>
        `;

        document.getElementById('edit-my-name').value = user.name;
        document.getElementById('edit-my-pass').value = '';
    })
    .catch(error => {
        console.error('Failed to load profile:', error);
        document.getElementById('myProfile').innerHTML = '<p class="error">❌ ' + error.message + '</p>';
    });
}

function showMyEditForm() {
    document.getElementById('editFormUser').style.display = 'block';
    document.getElementById('editFormUser').scrollIntoView({ behavior: 'smooth' });
}

function closeMyEditForm() {
    document.getElementById('editFormUser').style.display = 'none';
}

function updateMyProfile() {
    const name = document.getElementById('edit-my-name').value.trim();
    const pass = document.getElementById('edit-my-pass').value.trim();

    if (!name || !pass) {
        alert('⚠️ יש למלא את כל השדות');
        return false;
    }

    // קבל את ה-ID של המשתמש הנוכחי תחילה
    fetch(`${uri}/me`, {
        headers: getAuthHeaders()
    })
    .then(response => response.json())
    .then(currentUser => {
        const user = {
            Id: currentUser.id,
            Name: name,
            Pass: pass,
            Type: currentUser.type // משתמש רגיל לא יכול לשנות את זה
        };

        return fetch(`${uri}/${currentUser.id}`, {
            method: 'PUT',
            headers: {
                'Accept': 'application/json',
                'Content-Type': 'application/json',
                ...getAuthHeaders()
            },
            body: JSON.stringify(user)
        });
    })
    .then(response => {
        if (!response.ok) {
            throw new Error('שגיאה בעדכון הפרופיל');
        }
        alert('✅ הפרופיל עודכן בהצלחה');
        closeMyEditForm();
        loadMyProfile();
    })
    .catch(error => {
        console.error('Failed to update profile:', error);
        alert('❌ ' + error.message);
    });

    return false;
}