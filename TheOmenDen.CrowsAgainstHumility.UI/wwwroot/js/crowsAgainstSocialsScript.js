export async function cawOutOnSocialMedia(title, text, url) {
    try {
            await navigator.share({
                title: title,
                text: text,
                url: url
            });   
    } catch (error) {
        console.error('Error occurred while sharing on social media', error);
    }
}

export function canCawOutOnSocialMedia() {
    return navigator.canShare;
}