import React, { createContext, useContext } from 'react';
import { useLocalStorage } from '../hooks/useLocalStorage';

export interface SettingsContextValue {
  text: string;
  setText: (value: string) => void;
}

const SettingsContext = createContext<SettingsContextValue | undefined>(
  undefined,
);

export const SettingsProvider: React.FC<React.PropsWithChildren> = ({
  children,
}) => {
  const [text, setText] = useLocalStorage<string>('text', '');

  return (
    <SettingsContext.Provider value={{ text, setText }}>
      {children}
    </SettingsContext.Provider>
  );
};

export const useSettings = (): SettingsContextValue => {
  const context = useContext(SettingsContext);
  if (!context) {
    throw new Error('useSettings must be used within a SettingsProvider');
  }
  return context;
};

export default SettingsContext;
