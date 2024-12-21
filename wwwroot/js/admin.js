document.addEventListener("DOMContentLoaded", async () => {
    const token = localStorage.getItem("token");
    if (!token) {
        alert("Vous devez être connecté pour accéder à cette page.");
        window.location.href = "index.html";
        return;
    }

    const userTableBody = document.querySelector("#userTable tbody");

    // Charger les utilisateurs existants
    async function loadUsers() {
        try {
            const response = await fetch("http://localhost:5146/api/users", {
                method: "GET",
                headers: {
                    "Authorization": `Bearer ${token}`
                }
            });

            if (response.ok) {
                const users = await response.json();
                userTableBody.innerHTML = ""; // Réinitialiser le tableau
                users.forEach(user => {
                    const row = userTableBody.insertRow();
                    row.innerHTML = `
                        <td>${user.UserId}</td>
                        <td>${user.Username}</td>
                        <td>${user.Email}</td>
                        <td>${user.Role}</td>
                        <td>
                            <button class="delete-btn" data-id="${user.UserId}">Supprimer</button>
                        </td>
                    `;
                });
            } else {
                alert("Erreur lors du chargement des utilisateurs.");
            }
        } catch (error) {
            console.error("Erreur :", error);
        }
    }

    // Ajouter un nouvel utilisateur
    document.getElementById("addUserForm").addEventListener("submit", async (event) => {
        event.preventDefault();

        const username = document.getElementById("username").value;
        const email = document.getElementById("email").value;
        const password = document.getElementById("password").value;
        const roleId = document.getElementById("role").value;

        try {
            const response = await fetch("http://localhost:5146/api/users", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                    "Authorization": `Bearer ${token}`
                },
                body: JSON.stringify({ Username: username, Email: email, Password: password, RoleId: parseInt(roleId) })
            });

            if (response.ok) {
                alert("Utilisateur ajouté avec succès.");
                loadUsers(); // Recharger la liste des utilisateurs
            } else {
                const error = await response.text();
                alert("Erreur : " + error);
            }
        } catch (error) {
            console.error("Erreur :", error);
        }
    });

    // Charger les utilisateurs au démarrage
    loadUsers();
});
