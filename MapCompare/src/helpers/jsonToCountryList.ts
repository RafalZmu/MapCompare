import Country from "../Models/Country";

export function JsonToCountryList(json: string) : Country[] {
    const countriesArray : Country[] = [];
    const jsonArray = JSON.parse(json);
    console.log(jsonArray);

    jsonArray.forEach((item: any) => {
        const name = item["Country"];
        const year = item["Year"];
        const value = parseFloat(item["Value"]);
        const description = item["Description"];
        const color = "#000000"; // default color

        const country = new Country(name, year, value, description, color);
        countriesArray.push(country);
    });
    console.log(countriesArray);
    return countriesArray;
}