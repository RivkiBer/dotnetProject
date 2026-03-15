const uri = '/user';
let Users = [];
let token = localStorage.getItem('token');

if (!token) {
    window.location.href = 'login.html';
} else {
    getItems();
}

function getAuthHeaders() {
    return { 'Authorization': `Bearer ${token}` };
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
    const addPassTextbox = document.getElementById('add-pass');
    const addTypeSelect = document.getElementById('add-type');

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
        .then(response => response.json())
        .then(() => {
            getItems();
            addNameTextbox.value = '';
            addPassTextbox.value = '';
        })
        .catch(error => console.error('Unable to add item.', error));
}

function deleteItem(id) {
    fetch(`${uri}/${id}`, {
            method: 'DELETE',
            headers: getAuthHeaders()
        })
        .then(() => getItems())
        .catch(error => console.error('Unable to delete item.', error));
}

function displayEditForm(id) {
    const item = Users.find(item => item.id === id);

    document.getElementById('edit-name').value = item.name;
    document.getElementById('edit-pass').value = item.pass;
    document.getElementById('edit-type').value = item.type;
    document.getElementById('edit-id').value = item.id;
    document.getElementById('editForm').style.display = 'block';
}

function updateItem() {
    const itemId = document.getElementById('edit-id').value;
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
        .then(() => getItems())
        .catch(error => console.error('Unable to update item.', error));

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

function _displayItems(data) {
    const tBody = document.getElementById('User');
    tBody.innerHTML = '';

    _displayCount(data.length);

    const button = document.createElement('button');

    data.forEach(item => {
        let typeCell = document.createElement('td');
        typeCell.innerText = item.type;

        let nameCell = document.createElement('td');
        nameCell.innerText = item.name;

        let editButton = button.cloneNode(false);
        editButton.innerText = 'Edit';
        editButton.setAttribute('onclick', `displayEditForm(${item.id})`);

        let deleteButton = button.cloneNode(false);
        deleteButton.innerText = 'Delete';
        deleteButton.setAttribute('onclick', `deleteItem(${item.id})`);

        let row = document.createElement('tr');
        row.appendChild(typeCell);
        row.appendChild(nameCell);
        row.appendChild(editButton);
        row.appendChild(deleteButton);

        tBody.appendChild(row);
    });

    Users = data;
}

// Check if already logged in
if (token) {
    document.getElementById('loginForm').style.display = 'none';
    document.getElementById('userSection').style.display = 'block';
    getItems();
}