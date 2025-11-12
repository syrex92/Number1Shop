import Api from "./api.ts";

class GenericApi extends Api{
    async as<T>(response: Response): Promise<T> {
        if (!response.ok) {
            throw new Error(`HTTP error: ${response.status}`);
        }
        // Предполагается JSON-ответ. Можно добавить switch для других форматов.
        const data = await response.json();
        return data as T;
    }

    
}

export default GenericApi;