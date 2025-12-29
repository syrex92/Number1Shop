import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

// https://vite.dev/config/
export default defineConfig({
    plugins: [react()],
    preview: {
        port: 8105,
        strictPort: true,
    },
    server: {
        port: 8105,
        strictPort: true,
        host: true,
        origin: "http://localhost:8105",
    },
    //envPrefix: ""
})
