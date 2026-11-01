import react from '@vitejs/plugin-react'
import { defineConfig } from 'vite'

// https://vite.dev/config/
export default defineConfig({
  plugins: [react()],
  server: {
    // During development, forward API calls to the ASP.NET Core API.
    proxy: {
      '/api': 'http://localhost:5080',
    },
  },
})
