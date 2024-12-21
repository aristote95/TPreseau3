document.getElementById("registerForm").addEventListener("submit", async (event) => {
    event.preventDefault();

    const username = document.getElementById("username").value.trim();
    const email = document.getElementById("email").value.trim();
    const password = document.getElementById("password").value.trim();
    const role = document.getElementById("role").value;

    // Validation des champs côté frontend
    if (!username || !email || !password || !role) {
        alert("Tous les champs sont obligatoires.");
        return;
    }

    if (!validateEmail(email)) {
        alert("Veuillez entrer une adresse email valide.");
        return;
    }
    

    try {
        const response = await fetch("http://localhost:5146/api/auth/register", {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
            },
            body: JSON.stringify({ username, email, password, role }),
        });

        if (response.ok) {
            alert("Compte créé avec succès ! Vous pouvez maintenant vous connecter.");
            window.location.href = "index.html"; // Redirige vers la page de connexion.
        } else {
            const error = await response.json();
            alert(`Erreur : ${error.message || "Une erreur est survenue lors de l'enregistrement."}`);
        }
    } catch (error) {
        console.error("Erreur :", error);
        alert("Impossible de contacter le serveur. Veuillez vérifier votre connexion.");
    }
});

// Fonction pour valider l'adresse email
function validateEmail(email) {
    const re = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return re.test(email);
}