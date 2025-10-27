// ! Index file Scripts
document.addEventListener("DOMContentLoaded", () => {
    const regRole = document.getElementById("reg-role");
    const marketExtra = document.getElementById("market-extra");
    regRole.addEventListener("change", () => {
        marketExtra.style.display = regRole.value === "market" ? "block" : "none";
    });

    const registerForm = document.getElementById("register-form");
    registerForm.addEventListener("submit", async (e) => {
        e.preventDefault();
        const username = document.getElementById("reg-username").value.trim();
        const password = document.getElementById("reg-password").value;
        const role = document.getElementById("reg-role").value;
        const marketName = document.getElementById("reg-market-name")?.value.trim();
        const marketDesc = document.getElementById("reg-market-desc")?.value.trim();
        const photo = document.getElementById("reg-photo")?.files[0];

        try {
            const user = await registerUser({
                username,
                password,
                role,
                marketName,
                marketDesc,
                photo
            });
            alert(`Registered: ${user.username}`);
            localStorage.setItem("loggedInUserId", user.id);
            if (user.role === "client") {
                location.href = "client.html";
            } else {
                location.href = "market.html";
            }
        } catch (err) {
            alert(err.message);
        }
    });

    const loginForm = document.getElementById("login-form");
    loginForm.addEventListener("submit", async (e) => {
        e.preventDefault();
        const username = document.getElementById("login-username").value.trim();
        const password = document.getElementById("login-password").value;
        try {
            const user = await loginUser({ username, password });
            localStorage.setItem("loggedInUserId", user.id);
            if (user.role === "client") {
                location.href = "client.html";
            } else {
                location.href = "market.html";
            }
        } catch (err) {
            alert(err.message || "Invalid credentials");
        }
    });

    document.getElementById("seed-demo")?.addEventListener("click", () => {
        alert("Demo seeding not supported with API. Register manually.");
    });
});

// ! Market file Scripts
document.addEventListener("DOMContentLoaded", () => {
    const userId = localStorage.getItem("loggedInUserId");
    if (!userId) {
        alert("Please login first.");
        location.href = "index.html";
        return;
    }

    // ???? ????? ?????? ?? ?????
    async function checkUserRole() {
        const token = localStorage.getItem("token");
        if (!token) return null;
        try {
            const user = decodeJwt(token); // ?????? ?? decodeJwt ????? ?? app.js
            return user;
        } catch {
            return null;
        }
    }

    checkUserRole().then(user => {
        if (!user || user.role !== "market") {
            alert("This page is for market users only.");
            location.href = "index.html";
            return;
        }

        document.getElementById("who").textContent = user.username;
        document.getElementById("logout").addEventListener("click", () => {
            localStorage.removeItem("loggedInUserId");
            localStorage.removeItem("token");
            location.href = "index.html";
        });

        const market = getMarketByOwner(user.id);
        if (!market) {
            alert("No market found for this user. Please register as a market.");
            location.href = "index.html";
            return;
        }

        function renderProducts() {
            const productsEl = document.getElementById("products");
            productsEl.innerHTML = "<p>Products endpoint not available. Add /api/Products/market/{marketId}.</p>";
        }

        document.getElementById("product-form").addEventListener("submit", async (e) => {
            e.preventDefault();
            const name = document.getElementById("prod-name").value.trim();
            const price = parseFloat(document.getElementById("prod-price").value);
            const description = document.getElementById("prod-desc").value.trim();
            const photo = document.getElementById("prod-photo")?.files[0];
            try {
                const product = await addProduct({
                    marketId: market.id,
                    name,
                    price,
                    description,
                    photo
                });
                const subs = getSubscriptionsByMarket(market.id);
                subs.forEach((s) => {
                    addNotification({
                        recipientType: "client",
                        recipientId: s.clientId,
                        message: `New product "${product.name}" added to ${market.name}`
                    });
                });
                renderProducts();
                renderNotifications();
                e.target.reset();
            } catch (err) {
                alert(err.message);
            }
        });

        function renderNotifications() {
            const ul = document.getElementById("notifications-list");
            ul.innerHTML = "";
            const notes = getNotificationsForRecipient({
                recipientType: "market",
                recipientId: market.id
            });
            notes.sort((a, b) => b.timestamp - a.timestamp);
            notes.forEach((n) => {
                const li = document.createElement("li");
                li.textContent = `${new Date(n.timestamp).toLocaleString()}: ${n.message}`;
                ul.appendChild(li);
            });
        }

        renderProducts();
        renderNotifications();
    });
});

// ! Client file scripts
document.addEventListener("DOMContentLoaded", () => {
    const userId = localStorage.getItem("loggedInUserId");
    if (!userId) {
        alert("Please login first.");
        location.href = "index.html";
        return;
    }

    // ???? ????? ?????? ?? ?????
    async function checkUserRole() {
        const token = localStorage.getItem("token");
        if (!token) return null;
        try {
            const user = decodeJwt(token); // ?????? ?? decodeJwt ????? ?? app.js
            return user;
        } catch {
            return null;
        }
    }

    checkUserRole().then(user => {
        if (!user || user.role !== "client") {
            alert("This page is for client users only.");
            location.href = "index.html";
            return;
        }

        document.getElementById("who").textContent = user.username;
        document.getElementById("logout").addEventListener("click", () => {
            localStorage.removeItem("loggedInUserId");
            localStorage.removeItem("token");
            location.href = "index.html";
        });

        function renderMarkets() {
            const marketsEl = document.getElementById("markets");
            marketsEl.innerHTML = "<p>Markets endpoint not available. Add /api/Markets.</p>";
        }

        function renderNotifications() {
            const ul = document.getElementById("notifications-list");
            ul.innerHTML = "";
            const notes = getNotificationsForRecipient({
                recipientType: "client",
                recipientId: user.id
            });
            notes.sort((a, b) => b.timestamp - a.timestamp);
            notes.forEach((n) => {
                const li = document.createElement("li");
                li.textContent = `${new Date(n.timestamp).toLocaleString()}: ${n.message}`;
                ul.appendChild(li);
            });
        }

        renderMarkets();
        renderNotifications();
    });
});