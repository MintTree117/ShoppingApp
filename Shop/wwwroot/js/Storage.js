window.localStorageHelper = {
    set: function (key, value) {
        localStorage.setItem(key, value);
    },
    get: function (key) {
        return localStorage.getItem(key);
    },
    remove: function (key) {
        localStorage.removeItem(key);
    },
    clear: function () {
        localStorage.clear();
    }
};

window.sessionStorageHelper = {
    set: function (key, value) {
        sessionStorage.setItem(key, value);
    },
    get: function (key) {
        return sessionStorage.getItem(key);
    },
    remove: function (key) {
        sessionStorage.removeItem(key);
    },
    clear: function () {
        sessionStorage.clear();
    }
};
