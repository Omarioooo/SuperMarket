(function () {
    // ????? SignalR ???????? ?????????
    const connection = new signalR.HubConnectionBuilder()
        .withUrl("https://localhost:7255/notificationHub")
        .configureLogging(signalR.LogLevel.Information)
        .build();

    // ??? ??????? ?? SignalR
    async function startSignalR() {
        try {
            await connection.start();
            console.log("SignalR Connected");
        } catch (err) {
            console.error("SignalR Connection Error:", err);
            setTimeout(startSignalR, 5000); // ????? ???????? ??? 5 ?????
        }
    }

    // ????? ????????? ?????? ?????? ?? ???????
    let notifications = [];
    connection.on("ReceiveNotification", (message) => {
        notifications.push({
            id: uid("note"),
            message: message.message,
            timestamp: Date.now(),
            recipientType: message.recipientType || "unknown",
            recipientId: message.recipientId || "unknown"
        });
        if (window.renderNotifications) {
            window.renderNotifications();
        }
    });

    startSignalR();

    // ???? ?????? ?????? ????? ??? ??? API
    async function fetchApi(endpoint, options = {}) {
        const response = await fetch(`http://localhost:5042/api/${endpoint}`, {
            ...options,
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${localStorage.getItem('token')}`,
                ...(options.headers || {}),
            },
        });
        if (!response.ok) {
            const error = await response.json().catch(() => ({}));
            throw new Error(error.message || `Failed to fetch ${endpoint}`);
        }
        return response.json();
    }

    // ???? ?????? ??????
    function uid(prefix = "id") {
        return prefix + "_" + Math.random().toString(36).slice(2, 9);
    }

    // ???? ????? ?????? ???? XSS
    function escapeHtml(s) {
        if (!s) return "";
        return s.replace(/[&<>"']/g, (c) => ({
            "&": "&amp;",
            "<": "&lt;",
            ">": "&gt;",
            '"': "&quot;",
            "'": "&#39;",
        }[c]));
    }

    // ???? ?????? ???????? ?????? ?? ??? JWT token
    function decodeJwt(token) {
        try {
            const payload = JSON.parse(atob(token.split('.')[1]));
            return {
                id: payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'],
                role: payload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'],
                username: payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name']
            };
        } catch (err) {
            console.error("JWT Decode Error:", err);
            return {};
        }
    }

    // ????? ?????? ????
    window.registerUser = async function ({
        username,
        password,
        role,
        marketName,
        marketDesc,
        photo
    }) {
        const formData = new FormData();
        formData.append('Email', username);
        formData.append('Password', password);
        formData.append('Role', role);
        if (role === "market") {
            formData.append('MarketName', marketName || `${username}'s Market`);
            formData.append('Description', marketDesc || '');
        } else {
            formData.append('FirstName', username.split('@')[0]);
            formData.append('LastName', '');
        }
        if (photo) formData.append('Photo', photo);

        try {
            const response = await fetch('http://localhost:5042/api/Account/Register', {
                method: 'POST',
                body: formData,
                headers: {}
            });
            if (!response.ok) {
                const error = await response.json().catch(() => ({}));
                throw new Error(error.message || 'Registration failed');
            }
            // ??? ???????? ???? ????? ???? ?????? ???? ??? token ??????? ????????
            const loginResponse = await fetchApi('Account/Login', {
                method: 'POST',
                body: JSON.stringify({ UserNameOrEmail: username, Password: password })
            });
            localStorage.setItem('token', loginResponse.token);
            const user = decodeJwt(loginResponse.token);
            return {
                id: user.id,
                username: user.username,
                role: user.role
            };
        } catch (err) {
            throw new Error(err.message);
        }
    };

    // ????? ??????
    window.loginUser = async function ({ username, password }) {
        try {
            const response = await fetchApi('Account/Login', {
                method: 'POST',
                body: JSON.stringify({ UserNameOrEmail: username, Password: password })
            });
            localStorage.setItem('token', response.token);
            const user = decodeJwt(response.token);
            return {
                id: user.id,
                username: user.username,
                role: user.role
            };
        } catch (err) {
            throw new Error(err.message);
        }
    };

    // ???????? ?? ???
    window.subscribeToMarket = async function (clientId, marketId) {
        try {
            await fetchApi(`Client/Subscribe/${marketId}`, {
                method: 'POST'
            });
        } catch (err) {
            throw new Error(err.message);
        }
    };

    // ????? ???????? ?? ???
    window.unsubscribeFromMarket = async function (clientId, marketId) {
        try {
            await fetchApi(`Client/UnSubscribe/${marketId}`, {
                method: 'POST'
            });
        } catch (err) {
            throw new Error(err.message);
        }
    };

    // ????? ????
    window.addProduct = async function ({ marketId, name, price, description, photo }) {
        const formData = new FormData();
        formData.append('Name', name);
        formData.append('Price', price);
        formData.append('Description', description || '');
        if (photo) formData.append('Photo', photo);

        try {
            return await fetchApi('Market/CreateProduct', {
                method: 'POST',
                body: formData,
                headers: {}
            });
        } catch (err) {
            throw new Error(err.message);
        }
    };

    // ????? ????? ??????? ??? ????????
    window.getAllMarkets = async function () {
        console.warn("getAllMarkets not implemented. Add /api/Markets endpoint.");
        return [];
    };

    window.getMarketByOwner = async function (ownerUserId) {
        console.warn("getMarketByOwner not implemented. Add /api/Markets/owner/{userId} endpoint.");
        return null;
    };

    window.getProductsByMarket = async function (marketId) {
        console.warn("getProductsByMarket not implemented. Add /api/Products/market/{marketId} endpoint.");
        return [];
    };

    window.getSubscriptionsByClient = async function (clientId) {
        console.warn("getSubscriptionsByClient not implemented. Add /api/Client/Subscriptions/{clientId} endpoint.");
        return [];
    };

    window.getSubscriptionsByMarket = async function (marketId) {
        console.warn("getSubscriptionsByMarket not implemented. Add /api/Market/Subscriptions/{marketId} endpoint.");
        return [];
    };

    // ??? ????????? ?? SignalR
    window.getNotificationsForRecipient = function ({ recipientType, recipientId }) {
        return notifications.filter(
            (n) => n.recipientType === recipientType && n.recipientId === recipientId
        );
    };

    // ????? ????? ????
    window.addNotification = function ({ recipientType, recipientId, message }) {
        notifications.push({
            id: uid("note"),
            recipientType,
            recipientId,
            message,
            timestamp: Date.now()
        });
        if (window.renderNotifications) {
            window.renderNotifications();
        }
    };

    // ????? ??????
    window.getMarketByOwner = getMarketByOwner;
    window.getProductsByMarket = getProductsByMarket;
    window.getSubscriptionsByMarket = getSubscriptionsByMarket;
    window.getSubscriptionsByClient = getSubscriptionsByClient;
    window.addProduct = addProduct;
    window.escapeHtml = escapeHtml;

    // ????? ????????? ??? ????? ???????
    connection.onclose(() => {
        console.log("SignalR Disconnected");
        startSignalR();
    });
})();