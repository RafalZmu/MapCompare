<script setup lang="ts">
import { onMounted, type Ref } from 'vue';
import { ref, reactive } from 'vue';
import L, { geoJSON } from "leaflet";
import "leaflet/dist/leaflet.css";
import * as countriesJson from "world-geojson";
import poland from 'world-geojson/countries/poland.json';

let map = null;


onMounted(async () => {
})

// GeoJSON data (you can also load this from a file or URL)
const geojsonData: GeoJSON.FeatureCollection = {
  "type": "FeatureCollection",
  "features": [
    {
      "type": "Feature",
      "properties": { "name": "Country 1", "color": "#ff0000" },
      "geometry": {
        "type": "Polygon",
        "coordinates": [
          [
            [-10.0, 50.0],
            [10.0, 50.0],
            [10.0, 60.0],
            [-10.0, 60.0],
            [-10.0, 50.0]
          ]
        ]
      }
    },
    {
      "type": "Feature",
      "properties": { "name": "Country 2", "color": "#00ff00" },
      "geometry": {
        "type": "Polygon",
        "coordinates": [
          [
            [10.0, 50.0],
            [30.0, 50.0],
            [30.0, 60.0],
            [10.0, 60.0],
            [10.0, 50.0]
          ]
        ]
      }
    }
  ]
};

const mapContainer = ref(null);
onMounted(() => {
  if (mapContainer.value) {
    map = L.map(mapContainer.value).setView([51.505, -0.09], 3); // Initial view: center at lat/lon with zoom level

    L.tileLayer("https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png", {
      attribution: '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors',
    }).addTo(map);

    L.geoJSON(poland, {
      style: (feature) => {
        return {
          color: "black",
          weight: 0,
          opacity: 10,
          fillColor: "red",
          fillOpacity: 0.5,

        };
      },
    }).addTo(map);
  }
});
</script>

<template>
  <h1>Map Compare</h1>
  <button>Change map</button>
  <div class="map" ref="mapContainer" ></div>
</template>
