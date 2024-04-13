
export async function GetBaseMap(): Promise<string>{
  try {
    const response = await fetch('https://localhost:7210/BaseMap');
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

export async function GetMap(mapName: string): Promise<string>{
  try {
    const response = await fetch('https://localhost:7210/Map/' + mapName);
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
