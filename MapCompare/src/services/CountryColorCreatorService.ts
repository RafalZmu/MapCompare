import Country from "../Models/Country";
// Responsible for assigning colors to countries based on their values colorLowerBound and colorUpperBound are represented in hex format
export function AssignColorToCountries(countries: Country[], colorLowerBound: string, colorUpperBound: string): Country[]{
    const countriesArray = countries;
    const maxValue = Math.max(...countriesArray.map((country) => country.value));
    const minValue = Math.min(...countriesArray.map((country) => country.value));
    console.log(maxValue);
    console.log(minValue);

    countriesArray.forEach((country) => {
        const percetage = (country.value - minValue) / (maxValue - minValue);
        country.color = interpolateColor(colorLowerBound, colorUpperBound, percetage);
    });

    return countriesArray;
}

function interpolateColor(color1: string, color2: string, percentage: number): string {
    // Extract RGB components from hex colors
    const r1 = parseInt(color1.substring(1, 3), 16);
    const g1 = parseInt(color1.substring(3, 5), 16);
    const b1 = parseInt(color1.substring(5, 7), 16);

    const r2 = parseInt(color2.substring(1, 3), 16);
    const g2 = parseInt(color2.substring(3, 5), 16);
    const b2 = parseInt(color2.substring(5, 7), 16);

    // Perform interpolation
    const r = Math.round(r1 + (r2 - r1) * percentage);
    const g = Math.round(g1 + (g2 - g1) * percentage);
    const b = Math.round(b1 + (b2 - b1) * percentage);

    // Convert back to hex and return
    return `#${r.toString(16).padStart(2, '0')}${g.toString(16).padStart(2, '0')}${b.toString(16).padStart(2, '0')}`;
}