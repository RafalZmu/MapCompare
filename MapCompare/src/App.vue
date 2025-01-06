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
import { el } from 'vuetify/locale';

let map: L.Map | null = null;
let formData = reactive({
  keyword: '',
  query: ''
});

const countries = GetCountriesFromJson().filter((country): country is string => country !== undefined);
const coloredCountries: Ref<Country[]> = ref([]);
const countryLayers: { [key: string]: L.GeoJSON } = {};
const mapContainer = ref<HTMLElement | null>(null);
const sourceOptions = ['UNSD', 'Web'];
let selectedSource = ref('Web');
let colorLow = ref('#FF0000');
let colorHigh = ref('#00FF00');

function GetNewMap() {
  let source = 0;
  if(selectedSource.value == "UNSD")
  {
    source = 1;
  }
  else if(selectedSource.value == "Web")
  {
    source = 2;
  }

  GetValues(formData.keyword, formData.query, source).then((data) => {
    const countryList = JsonToCountryList(data);
    coloredCountries.value = AssignColorToCountries(countryList, colorLow.value, colorHigh.value);
    console.log(coloredCountries.value);
    coloredCountries.value.forEach(country => {
      if (country.name) {
        changeCountryColor(country.name, country.color);
      }
    });
  });
}

function RecolorCountries() {
  coloredCountries.value = AssignColorToCountries(coloredCountries.value, colorLow.value, colorHigh.value);
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
  <h1 style="justify-self: center;">Map Compare</h1>
  <v-container class="input-container">
    <v-row>
      <v-col>
        <v-text-field type="text" label="Keyword" id="keyword" v-model="formData.keyword" name="keyword"></v-text-field>
      </v-col>
      <v-col>
        <v-text-field type="text" id="query" label="Addictional information" v-model="formData.query"
          name="query"></v-text-field>
      </v-col>
      <v-col>
        <v-menu :bottom="true">
          <template v-slot:activator="{ props }">
            <v-btn class="source-button" color="primary" dark v-bind="props">
              Source: {{ selectedSource }}
            </v-btn>
          </template>

          <v-list>
            <v-list-item v-for="(item, index) in sourceOptions" :key="index" @click="selectedSource = item">
              <v-list-item-title>{{ item }}</v-list-item-title>
            </v-list-item>
          </v-list>
        </v-menu>
      </v-col>
    </v-row>
  </v-container>
  <v-container>
    <v-row>
      <v-col>
        <v-btn style="width: 100%;" @click="GetNewMap">Generage new map</v-btn>
      </v-col>
      <v-col>
        <v-btn style="width: 100%;" @click="RecolorCountries">Recolor countries</v-btn>
      </v-col>
    </v-row>
  </v-container>
  <div class="map" ref="mapContainer"></div>
  <v-container class="color-pickers">
    <v-row>
      <v-col>
        <h2>Pick color for low values</h2>
        <v-color-picker hide-inputs v-model="colorLow" @input="changeCountryColor" />
      </v-col>
      <v-col>
        <h2>Pick color for high values</h2>
        <v-color-picker hide-inputs v-model="colorHigh" @input="changeCountryColor" />
      </v-col>
    </v-row>
  </v-container>
  <v-container v-if="coloredCountries.length > 0">
    <CountriesTable :countries="coloredCountries" @update-country-value="updateCountryValue" />
  </v-container>
</template>

<style scoped>
.input-container {
  display: flex;
  gap: 10px; 
}

.color-pickers {
  display: flex;
  justify-content: space-around;
}
.source-button {
  width: 100%;
  height: 80% !important;
}

.v-row{
  display: flex;
  flex-wrap: nowrap;
}
.v-col{
  flex: 1;
}
</style>
