function isRecaptchaLoaded(key) {
    try {
        grecaptcha.execute(key, { action: 'homepage' }).then(function(){
            return true;
        });
        return true;
    } catch (ex) {
        console.error(JSON.stringify(ex));
        return false;
    }
}

async function generateCaptchaToken(key, action) {
    return await grecaptcha.execute(key, { action: action })
        .then(function(token) {
            return token;
        });
}