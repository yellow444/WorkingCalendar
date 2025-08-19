export interface CalendarResponse {
  data: string;
}

export const fetchCalendar = async (): Promise<CalendarResponse> => {
  const response = await fetch('/WorkingCalendar');
  if (!response.ok) {
    throw new Error('Failed to fetch calendar');
  }
  return (await response.json()) as CalendarResponse;
};
