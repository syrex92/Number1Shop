const addEndSlash = (url: string) => {
    if(!url) return '';
    const trimmed = url.trim();
    if (!trimmed.length) return trimmed;
    return trimmed.endsWith("/") ? trimmed : trimmed + "/";
}

export default addEndSlash;