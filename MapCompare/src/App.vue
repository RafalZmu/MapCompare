<script setup lang="ts">
import { onMounted, type Ref } from 'vue';
import { ref, reactive } from 'vue';
import L, { geoJSON } from "leaflet";
import "leaflet/dist/leaflet.css";
import { GetCountriesFromJson } from './helpers/countriesListImport';
import { GetValues } from './services/MapService';

let map = null;
let formData = reactive({
  keyword: '',
  query: ''
});

const countries = GetCountriesFromJson();

function GetNewMap(){ 
  GetValues(formData.keyword, formData.query).then((data) => {
    console.log(data);
  });
}

// GeoJSON data (you can also load this from a file or URL)
const mapContainer = ref(null);

onMounted(async () => {
  if (mapContainer.value) {
    map = L.map(mapContainer.value).setView([51.505, -0.09], 3); // Initial view: center at lat/lon with zoom level

    L.tileLayer("https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png", {
      attribution: '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors',
    }).addTo(map);

    for (const country of countries){
      try {
        const countryGeoJson = await import(/* @vite-ignore */'../node_modules/world-geojson/countries/' + country + '.json');
        L.geoJSON(countryGeoJson.default, {
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
      } catch (error) {
        console.log(error)
      }

    }
  }
});

const generateRandomColor = () => {
  return '#' + Math.floor(Math.random()*16777215).toString(16);
}
</script>

<template>
  <h1>Map Compare</h1>
  <div>
    <label for="keyword">URL:</label>
    <input type="text" id="kayword" v-model="formData.keyword" name="keyword">
  </div>
  <div>
    <label for="query">Query:</label>
    <input type="text" id="query" v-model="formData.query" name="query">
  </div>
  <button @click="GetNewMap">Change map</button>
  <div class="map" ref="mapContainer" ></div>
</template>
