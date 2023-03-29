loaded = [];
function loadJs(sourceUrl) {
	if(loaded[sourceUrl]) {
		console.info(`${sourceUrl} already loaded`);

		return new this.Promise(function(resolve, reject) {
			resolve();
		});
	}

	return new Promise(function(resolve, reject) {	
		const tag = document.createElement('script');

		if(sourceUrl ==='https://www.google.com/recaptcha/api.js') {
			tag.src = sourceUrl + '?render=explicit';
		}

        tag.async = true;
        tag.defer = true;
        tag.src = sourceUrl;
        tag.type = "text/javascript";

		console.info(`${sourceUrl} created`);

		loaded[sourceUrl] = true;

		tag.onload = function () {
			console.info(`${sourceUrl} loaded`);
            resolve(sourceUrl);
        };

		tag.onerror = function () {
			console.error(`${sourceUrl} failed to load`);
            reject(sourceUrl);
        };

		document.body.appendChild(tag);
	});
}

function loadJsById(scriptId, sourceUrl) {
	if(loaded[sourceUrl]) {
		console.info(`${sourceUrl} already loaded`);

		return new this.Promise(function(resolve, reject) {
			resolve();
		});
	}

	return new Promise(function(resolve, reject) {	
		const tag = document.createElement('script');

		if(sourceUrl ==='https://www.google.com/recaptcha/api.js') {
			tag.src = sourceUrl + '?render=explicit';
		}

        tag.async = true;
        tag.defer = true;
		tag.id = scriptId;
        tag.src = sourceUrl;
        tag.type = "text/javascript";

		console.info(`${scriptId} created`);

		loaded[sourceUrl] = true;

		tag.onload = function () {
			console.info(`${scriptId} loaded`);
            resolve(sourceUrl);
        };

		tag.onerror = function () {
			console.error(`${scriptId} failed to load`);
            reject(sourceUrl);
        };

		document.body.appendChild(tag);
	});
}

