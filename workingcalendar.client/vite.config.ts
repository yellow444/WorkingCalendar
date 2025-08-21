import { fileURLToPath, URL } from 'node:url';
import { defineConfig } from 'vite';
import plugin from '@vitejs/plugin-react';
import { env } from 'process';

function resolveTarget(): string {
  // 1) Явный ASPNETCORE_URLS (например, "http://localhost:8080;https://localhost:7080")
  const urls = env.ASPNETCORE_URLS?.split(';').filter(Boolean);
  if (urls && urls.length > 0) return urls[0];

  // 2) Раздельные переменные портов (dev)
  const httpsPort = env.ASPNETCORE_HTTPS_PORTS ?? env.ASPNETCORE_HTTPS_PORT;
  if (httpsPort) return `https://localhost:${httpsPort.split(';')[0]}`;
  const httpPort = env.ASPNETCORE_HTTP_PORTS ?? env.ASPNETCORE_HTTP_PORT;
  if (httpPort)  return `http://localhost:${httpPort.split(';')[0]}`;

  // 3) По умолчанию — контейнерный порт API
  return 'http://localhost:8080';
}

const target = resolveTarget();
console.log('[VITE PROXY TARGET]:', target);

// https://vitejs.dev/config/
export default defineConfig({
  plugins: [plugin()],
  resolve: {
    alias: { '@': fileURLToPath(new URL('./src', import.meta.url)) }
  },
  server: {
    // Выровняли с SpaProxyServerUrl из .csproj
    port: 5173,
    proxy: {
      // Проксируем вызовы API к бэкенду без SSL-ошибок
      '^/WorkingCalendar': {
        target,
        changeOrigin: true,
        secure: false
        // rewrite не делаем, чтобы путь /WorkingCalendar/... дошёл как есть
      }
    }
  }
});
