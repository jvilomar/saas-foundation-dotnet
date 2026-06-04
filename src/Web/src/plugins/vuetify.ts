import { createVuetify } from 'vuetify'
import '@mdi/font/css/materialdesignicons.css'
import 'vuetify/styles'

import { appDarkTheme, appLightTheme } from '@/theme/appTheme'

export default createVuetify({
  theme: {
    defaultTheme: 'light',
    themes: {
      light: appLightTheme,
      dark: appDarkTheme,
    },
  },
})
