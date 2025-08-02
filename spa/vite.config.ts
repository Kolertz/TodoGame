import { defineConfig } from 'vite';
import react from '@vitejs/plugin-react';

export default defineConfig({
    plugins: [react()],
    server: {
        port: 51311,
        proxy: {
            // Proxy only the Keycloak realm endpoint
            '/realms': 'http://localhost:8080'
        }
    }
});
