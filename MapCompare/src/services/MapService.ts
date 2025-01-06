
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

// export async function GetValues(url: string,query: string): Promise<string>{
//     const response = await fetch('https://localhost:7210/Map/GetMap?keyword='+url+'&description='+ query.replace(' ', '_'));
//     if (!response.ok) {
//       throw new Error('Network response was not ok');
//     }
//     const data = await response.text();
//     return data;
// }

export async function GetValues(keyword: string, description: string, source: number): Promise<string>{
    let url = '';
    if (source == 1){// from unsd
      url = 'https://localhost:7210/Map/GetMap?keyword='+keyword.replace(' ', '_')+'&description='+ description.replace(' ', '_')
    }
    else if (source == 2){// from web
      url = 'https://localhost:7210/Map/GetMapFromWeb?keyword='+keyword.replace(' ', '_')+'&description='+ description.replace(' ', '_')
    }
    const response = await fetch(url);
    if (!response.ok) {
      throw new Error('Network response was not ok');
    }
    const data = await response.text();
    return data;
}
