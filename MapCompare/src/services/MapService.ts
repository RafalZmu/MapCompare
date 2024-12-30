
export async function GetBaseMap(): Promise<string>{
  try {
    const response = await fetch('https://localhost:7210/Map/BaseMap');
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

export async function GetValues(url: string,query: string): Promise<string>{
    const response = await fetch('https://localhost:7210/Map/GetMap?keyword='+url+'&description='+ query.replace(' ', '_'));
    if (!response.ok) {
      throw new Error('Network response was not ok');
    }
    const data = await response.text();
    return data;
}
