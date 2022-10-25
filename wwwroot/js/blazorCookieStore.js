window.blazorExtensions = {
    WriteCookie: function (name, value, days) {
        let expires = "";

        if (days) {
            const date = new Date();
            date.setTime(date.getTime() + (days * 86400000));

            expires = `; expires=${date.toUTCString()}`;
        }

        document.cookie = `${name}=${value}${expires};path=/`;
    },
    DeleteCookie: function (name) {
        document.cookie = `${name}=;expires=Thu, 01 Jan 1970 00:00:1 GMT`;
    }
}