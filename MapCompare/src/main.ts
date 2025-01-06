import './assets/main.css'
import '@coddicat/vue-pinch-scroll-zoom/style.css';

import { createVuetify } from 'vuetify';
import 'vuetify/styles'; // Import Vuetify styles
// (Optional) Import Material Design Icons
import { aliases, mdi } from 'vuetify/iconsets/mdi';

import { createApp } from 'vue'
import App from './App.vue'

// Create Vuetify Instance
const vuetify = createVuetify({
  icons: {
    defaultSet: 'mdi',
    aliases,
    sets: { mdi },
  },
  theme: {
    themes: {
      light: {
        colors: {
          primary: '#1976D2',
          secondary: '#424242',
        },
      },
    },
  },
});

const app = createApp(App);
app.use(vuetify);
app.mount('#app');

