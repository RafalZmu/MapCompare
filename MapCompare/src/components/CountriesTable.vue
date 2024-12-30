<!-- filepath: /f:/Projects/MapCompere/MapCompare/src/components/CountryTable.vue -->
<template>
  <div>
    <h2>Country Data</h2>
    <table>
      <thead>
        <tr>
          <th>Name</th>
          <th>Year</th>
          <th>Value</th>
          <th>Description</th>
          <th>Color</th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="country in countries" :key="country.name">
          <td>{{ country.name }}</td>
          <td>{{ country.year }}</td>
          <td>
            <input type="number" v-model.number="country.value" @change="updateValue(country)" />
          </td>
          <td>{{ country.description }}</td>
          <td>
            <span :style="{ backgroundColor: country.color, display: 'inline-block', width: '20px', height: '20px' }"></span>
            {{ country.color }}
          </td>
        </tr>
      </tbody>
    </table>
  </div>
</template>

<script setup lang="ts">
import { defineProps, defineEmits } from 'vue';
import Country from '../Models/Country';

const props = defineProps<{
  countries: Country[];
}>();

const emits = defineEmits(['update-country-value']);

function updateValue(country: Country) {
  emits('update-country-value', country);
}
</script>

<style scoped>
table {
  width: 100%;
  border-collapse: collapse;
}

th, td {
  border: 1px solid #ddd;
  padding: 8px;
}

th {
  background-color: #f2f2f2;
}
</style>