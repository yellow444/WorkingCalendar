import { defineConfig } from "vite";
import react from "@vitejs/plugin-react";

export default defineConfig({
    plugins: [react()],
    base: "/", // если будет подпрефикс (Ingress), сменишь на "/app/" и добавишь UsePathBase("/app")
    build: {
        outDir: "../WorkingCalendar.Server/wwwroot",
        emptyOutDir: true,
        assetsDir: "assets",
        sourcemap: true,
    },
    server: {
        port: 5173,
        strictPort: true,
        proxy: {
            "/CheckDayWorkingCalendar": "http://localhost:8080",
            "/GetYearWorkingCalendar": "http://localhost:8080",
            "/hc": "http://localhost:8080"
        }
    }
});
