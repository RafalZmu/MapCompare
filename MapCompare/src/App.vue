<script setup lang="ts">
import HelloWorld from './components/HelloWorld.vue'
import TheWelcome from './components/TheWelcome.vue'
import { onMounted, type Ref } from 'vue';
import { ref, reactive } from 'vue';
import PinchScrollZoom, {
  type PinchScrollZoomEmitData,
  type PinchScrollZoomExposed
} from '@coddicat/vue-pinch-scroll-zoom';

const zoomer = ref<PinchScrollZoomExposed>();
let CountryList: Ref<SVGPathElement[]> = ref([]);
let map = ref('');
let countrySVG: Ref<string[]> = ref([]);
let isDragging: boolean = false;

const state = reactive({
  eventName: '',
  eventData: null as PinchScrollZoomEmitData | null,
  scale: 1,
  originX: 0,
  originY: 0,
  translateX: 0,
  translateY: 0
});

function onEvent(name: string, e: PinchScrollZoomEmitData): void {
  state.eventName = name;
  state.eventData = e;
  state.scale = e.scale;
  state.originX = e.originX;
  state.originY = e.originY;
  state.translateX = e.translateX;
  state.translateY = e.translateY;
}

onMounted(async () => {
  map.value = await GetMap();
  CountryList.value = await GetCountryList(map);

  for(let i = 0; i < CountryList.value.length; i++){
    const path = CountryList.value[i].outerHTML;
    countrySVG.value.push(`<svg baseprofile="tiny" fill="#ececec" stroke="black" stroke-linecap="round" stroke-linejoin="round" stroke-width=".2" version="1.2" viewbox="0 0 2000 857" xmlns="http://www.w3.org/2000/svg">${path}</svg>`);
  }
  //One country to svg example
})

async function GetMap(): Promise<string>{
  try {
    const response = await fetch('https://localhost:7210/Map');
    if (!response.ok) {
      throw new Error('Network response was not ok');
    }
    const data = await response.text();
    return data;
  } catch (error) {
    console.error('There was a problem with the fetch operation:', error);
  }
  return '';
}

async function GetCountryList(map: Ref<string>){
  const parser = new DOMParser();
  const doc = parser.parseFromString(map.value, 'image/svg+xml');
  const countries = doc.getElementsByTagName('path');
  return Array.from(countries);
}

function changePathColor(event: MouseEvent){
  if(isDragging === true){
    return;
  }
  const target = event.target as SVGPathElement;

  if(target.tagName === 'path'){
    console.log(target.getAttribute("d"))
    if(target.getAttribute("fill") === 'red'){
      target.setAttribute('fill', 'green')
    }else{
      target.setAttribute('fill', 'red');
    }
  }
}

function draggingStart(){
  setTimeout(() =>{
    isDragging = true;
  }, 120);
}
function draggingStop(){
  console.log("Not dragging");
  setTimeout(() =>{
    isDragging = false;
  }, 100);
}

</script>

<template>
  <h1>Map Compare</h1>
  <button>Change map</button>
  <PinchScrollZoom ref="zoomer" :min-scale="1" :max-scale="10"  :width="2200" :height="857" @start-drag="draggingStart" @stop-drag="draggingStop"
    style="border: 1px solid black; position: relative; width:90vw; height: 90vh;">
    <div @click="changePathColor($event)" v-html="map"></div>
    <h5 style="position: absolute; top: 89px; left: 535px; color: white;">1</h5>

  </PinchScrollZoom>
</template>
