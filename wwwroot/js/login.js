document.getElementById("loginForm").addEventListener("submit", async (event) => {
    event.preventDefault();

    const username = document.getElementById("username").value;
    const password = document.getElementById("password").value;

    try {
        const response = await fetch("http://localhost:5146/api/auth/login", {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify({ username, password })
        });

        if (response.ok) {
            const data = await response.json();
            localStorage.setItem("token", data.Token); // Stocker le token JWT
            window.location.href = "admin.html"; // Rediriger vers l'administration
        } else {
            const error = await response.text();
            document.getElementById("error-message").textContent = error;
        }
    } catch (error) {
        console.error("Erreur :", error);
    }
});
