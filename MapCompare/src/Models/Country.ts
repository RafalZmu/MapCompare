class Country{
    name: string | undefined;
    year!: string | "-9999";
    value!: number | 0;
    description!: string | "";
    color!: string | "#000000";

    constructor(name: string, year: string, value: number, description: string, color: string){
        this.name = name;
        this.year = year;
        this.value = value;
        this.description = description;
        this.color = color;
    }
}

export default Country;