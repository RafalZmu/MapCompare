<script setup lang="ts">
import { onMounted, type Ref } from 'vue';
import { ref, reactive } from 'vue';
import { GetBaseMap } from './services/MapService';
import PinchScrollZoom, {
  type PinchScrollZoomEmitData,
  type PinchScrollZoomExposed
} from '@coddicat/vue-pinch-scroll-zoom';

const zoomer = ref<PinchScrollZoomExposed>();
let map = ref('');
let isDragging: boolean = false;


onMounted(async () => {
  map.value = await GetBaseMap();
})

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
  <PinchScrollZoom ref="zoomer" :min-scale="1" :max-scale="10"  :width="2200" :height="857"
    @start-drag="draggingStart" @stop-drag="draggingStop"

    style="border: 1px solid black; position: relative; width:90vw; height: 90vh;">
    <div @click="changePathColor($event)" v-html="map"></div>
    <h5 style="position: absolute; top: 89px; left: 535px; color: white;">1</h5>

  </PinchScrollZoom>
</template>
