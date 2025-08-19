import { render, screen } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { SettingsProvider } from '../context/SettingsContext';
import { ResizableTextarea } from './ResizableTextarea';

test('updates text when typing', async () => {
  const user = userEvent.setup();
  render(
    <SettingsProvider>
      <ResizableTextarea width={100} height={100} />
    </SettingsProvider>,
  );
  const textarea = screen.getByTestId(
    'calendar-textarea',
  ) as HTMLTextAreaElement;
  await user.type(textarea, 'hello');
  expect(textarea.value).toBe('hello');
});
