<script setup lang="ts">
import { onMounted, type Ref } from 'vue';
import { ref, reactive } from 'vue';
import L, { geoJSON } from "leaflet";
import "leaflet/dist/leaflet.css";
import { GetCountriesFromJson } from './helpers/countriesListImport';
import { GetValues } from './services/MapService';
import { JsonToCountryList } from './helpers/jsonToCountryList';
import { AssignColorToCountries } from './services/CountryColorCreatorService';
import Country from './Models/Country';
import CountriesTable from './components/CountriesTable.vue';

let map: L.Map | null = null;
let formData = reactive({
  keyword: '',
  query: ''
});

const countries = GetCountriesFromJson().filter((country): country is string => country !== undefined);
const coloredCountries: Ref<Country[]> = ref([]);
const countryLayers: { [key: string]: L.GeoJSON } = {};
const mapContainer = ref<HTMLElement | null>(null);

function GetNewMap() {
  GetValues(formData.keyword, formData.query).then((data) => {
    const countryList = JsonToCountryList(data);
    coloredCountries.value = AssignColorToCountries(countryList, '#FF0000', '#00FF00');
    console.log(coloredCountries.value);
    coloredCountries.value.forEach(country => {
      if (country.name) {
        changeCountryColor(country.name, country.color);
      }
    });
  });
}

function RecolorCountries() {
  coloredCountries.value = AssignColorToCountries(coloredCountries.value, '#FF0000', '#00FF00');
  coloredCountries.value.forEach(country => {
    if (country.name) {
      changeCountryColor(country.name, country.color);
    }
  });
}

function updateCountryValue(updatedCountry: Country) {
  const index = coloredCountries.value.findIndex(country => country.name === updatedCountry.name);
  if (index !== -1) {
    coloredCountries.value[index] = updatedCountry;
  }
}

const generateRandomColor = () => {
  return '#' + Math.floor(Math.random() * 16777215).toString(16);
}

function changeCountryColor(country: string, color: string) {
  if (!countryLayers[country]) return;

  countryLayers[country].setStyle({
    color: color,
    weight: 0,
    opacity: 10,
    fillColor: color,
    fillOpacity: 0.5,
  });
}

onMounted(async () => {
  if (mapContainer.value) {
    map = L.map(mapContainer.value).setView([51.505, -0.09], 3); // Initial view: center at lat/lon with zoom level

    L.tileLayer("https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png", {
      attribution: '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors',
    }).addTo(map);

    for (const country of countries) {
      try {
        const countryGeoJson = await import(/* @vite-ignore */'../node_modules/world-geojson/countries/' + country + '.json');
        const layer = L.geoJSON(countryGeoJson.default, {
          style: (feature) => {
            return {
              color: "black",
              weight: 0,
              opacity: 10,
              fillColor: generateRandomColor(),
              fillOpacity: 0.5,
            };
          },
        }).addTo(map);
        countryLayers[country] = layer;
      } catch (error) {
        console.log(error);
      }
    }
  }
});

</script>

<template>
  <h1>Map Compare</h1>
  <div class="input-container">
    <div>
      <label for="keyword">URL:</label>
      <input type="text" id="keyword" v-model="formData.keyword" name="keyword">
    </div>
    <div>
      <label for="query">Query:</label>
      <input type="text" id="query" v-model="formData.query" name="query">
    </div>
    <div>
      <button @click="RecolorCountries">Reload countries colors</button>
    </div>
  </div>
  <button @click="GetNewMap">Change map</button>
  <div class="map" ref="mapContainer"></div>
  <CountriesTable :countries="coloredCountries" @update-country-value="updateCountryValue" />
</template>

<style scoped>
.input-container {
  display: flex;
  gap: 10px; /* Optional: Adds space between the divs */
}

.input-container > div {
  display: flex;
  flex-direction: column;
}
</style>
