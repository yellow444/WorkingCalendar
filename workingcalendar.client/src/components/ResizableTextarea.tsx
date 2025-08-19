import React from 'react';
import { useSettings } from '../context/SettingsContext';

export interface ResizableTextareaProps {
  width: number;
  height: number;
}

export const ResizableTextarea: React.FC<ResizableTextareaProps> = ({
  width,
  height,
}) => {
  const { text, setText } = useSettings();

  return (
    <textarea
      data-testid="calendar-textarea"
      value={text}
      onChange={(e) => setText(e.target.value)}
      style={{ width, height }}
    />
  );
};

export default ResizableTextarea;
