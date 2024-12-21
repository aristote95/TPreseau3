document.addEventListener("DOMContentLoaded", async () => {
    const token = localStorage.getItem("token");
    if (!token) {
        alert("Vous devez être connecté pour accéder à cette page.");
        window.location.href = "index.html";
        return;
    }

    try {
        const response = await fetch("http://localhost:5146/api/clients/residential", {
            method: "GET",
            headers: {
                "Authorization": `Bearer ${token}`
            }
        });

        if (response.ok) {
            const clients = await response.json();
            const table = document.getElementById("residentialTable").querySelector("tbody");
            clients.forEach(client => {
                const row = table.insertRow();
                row.innerHTML = `
                    <td>${client.ClientId}</td>
                    <td>${client.ClientName}</td>
                    <td>${client.ClientType}</td>
                `;
            });
        } else {
            alert("Erreur lors du chargement des clients résidentiels.");
        }
    } catch (error) {
        console.error("Erreur :", error);
    }
});
