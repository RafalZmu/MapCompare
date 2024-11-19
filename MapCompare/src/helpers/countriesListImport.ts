import countries from "../assets/world.json";

export function GetCountriesFromJson(): string[] {
    let countriesNames: string[] = [];
    countriesNames = countries.map((country: any) => {
        return country.name;
    });

   return countriesNames; 
}


