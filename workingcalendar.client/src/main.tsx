import React from 'react'
import ReactDOM from 'react-dom/client'
import App from './App.tsx'
import './index.css'


ReactDOM.createRoot(document.getElementById('root')!).render(
  <React.StrictMode>
    <React.Suspense fallback={<Loading />}>

      <App />
    </React.Suspense>
  </React.StrictMode>,
)
function Loading() {
  return <h2>🌀 Loading...</h2>;
}