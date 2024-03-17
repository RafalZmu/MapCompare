
export async function GetMap(): Promise<string>{
  try {
    const response = await fetch('https://localhost:7210/Map');
    if (!response.ok) {
      throw new Error('Network response was not ok');
    }
    const data = await response.text();
    return data;
  } catch (error) {
    console.error('There was a problem with the fetch operation:', error);
  }
  return '';
}