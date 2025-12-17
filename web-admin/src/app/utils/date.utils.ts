export function dateToInputDateString(date: Date | string | null | undefined): string {
  if (!date) return '';

  if (typeof date === 'string') {
    if (/^\d{4}-\d{2}-\d{2}$/.test(date)) {
      return date;
    }

    return date.substring(0, 10);
  }

  const year: number = date.getFullYear();
  const month: string = String(date.getMonth() + 1).padStart(2, '0');
  const day: string = String(date.getDate()).padStart(2, '0');

  return `${year}-${month}-${day}`;
}

export function inputDateStringToDate(inputValue: string | null | undefined): Date | null {
  if (!inputValue) return null;

  const [yearString, monthString, dayString] = inputValue.split('-');

  const year: number = Number(yearString);
  const month: number = Number(monthString);
  const day: number = Number(dayString);

  return new Date(year, month - 1, day);
}
