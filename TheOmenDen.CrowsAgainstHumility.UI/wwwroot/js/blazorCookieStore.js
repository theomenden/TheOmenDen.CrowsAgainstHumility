window.blazorExtensions = {
    WriteCookie: function (name, value, days) {
        let expires = '';

        if (days) {
            const date = new Date();
            date.setTime(date.getTime() + (days * 86400000));

            expires = `; expires=${date.toUTCString()}`;
        }

        document.cookie = `${name}=${value}${expires};path=/`;
    },
    DeleteCookie: function (name) {
        document.cookie = `${name}=;expires=Thu, 01 Jan 1970 00:00:1 GMT`;
    },
    ReadCookie: function (cname) {
        const name = cname + '=';
        const decodedCookie = decodeURIComponent(document.cookie);
        const ca = decodedCookie.split(';');

        for (let i = 0; i < ca.length; i++) {
            let c = ca[i];
            while (c.charAt(0) === ' ') {
                c = c.substring(1);
            }
            if (c.indexOf(name) === 0) {
                return c.substring(name.length, c.length);
            }
        }

        return '';
    }
}