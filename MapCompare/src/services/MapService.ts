
export async function GetBaseMap(): Promise<string>{
  try {
    const response = await fetch('https://localhost:7210/Map');
    if (!response.ok) {
      throw new Error('Network response was not ok');
    }
    const data = await response.text();
    const json = JSON.parse(data);
    
    console.log(json.svgImage);
    return json.svgImage;
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
