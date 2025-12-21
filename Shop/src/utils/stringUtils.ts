const addEndSlash = (url: string) => {
    if(!url) return '';
    const trimmed = url.trim();
    return trimmed;
}

export default addEndSlash;