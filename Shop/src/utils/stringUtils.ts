const addEndSlash = (url: string) => {
    if(!url) return '';
    const trimmed = url.trim();
    if (!trimmed) return '';
    return trimmed.endsWith('/') ? trimmed : `${trimmed}/`;
}

export default addEndSlash;