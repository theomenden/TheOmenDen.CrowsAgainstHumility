function loadJs(sourceUrl) {
	if (sourceUrl.Length === 0) {
		console.error("Invalid source URL");
		return;
	}

	const tag = document.createElement('script');
	tag.src = sourceUrl;
	tag.type = "text/javascript";
	tag.async = "async";
    tag.defer = "defer";

	tag.onload = function () {
		console.log("Script loaded successfully");
	}

	tag.onerror = function () {
		console.error("Failed to load script");
	}

	document.body.appendChild(tag);
}

function loadJsById(scriptId, sourceUrl) {
	if (sourceUrl.Length === 0) {
        console.error("Invalid source URL");
    }

	const tag = document.createElement('script');
	tag.src = sourceUrl;
	tag.type = "text/javascript";
	tag.id = scriptId;
    tag.async = "async";
    tag.defer = "defer";

	tag.onload = function () {
		console.log("Script loaded successfully");
	}

	tag.onerror = function () {
		console.error("Failed to load script");
	}

	document.body.appendChild(tag);
}