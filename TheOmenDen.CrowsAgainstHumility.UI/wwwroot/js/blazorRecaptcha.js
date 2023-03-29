function renderRecaptcha(dotNetObject, siteKey) {
    setTimeout(function () {
        grecaptcha.ready(function() {
            grecaptcha.execute(siteKey).then(function(token){
                dotNetObject.invokeMethodAsync('OnRecaptchaResponse', token);
            });
        });
    }.bind(this), 1000);
}

function getResponse(widgetId) {
    return grecaptcha.getResponse(widgetId);
}