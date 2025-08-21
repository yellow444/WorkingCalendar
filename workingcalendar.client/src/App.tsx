import React from 'react';
import { SettingsProvider } from './context/SettingsContext';
import { ResizableTextarea } from './components/ResizableTextarea';

const App: React.FC = () => (
  <SettingsProvider>
    <ResizableTextarea width={400} height={300} />
  </SettingsProvider>
);

export default App;
