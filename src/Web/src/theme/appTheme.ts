export const appBrand = {
  primary: '#2563eb',
  secondary: '#64748b',
  ink: '#0f172a',
  surface: '#ffffff',
  background: '#f8fafc',
} as const

export const appLightTheme = {
  dark: false,
  colors: {
    primary: appBrand.primary,
    secondary: appBrand.secondary,
    accent: appBrand.primary,
    background: appBrand.background,
    surface: appBrand.surface,
    'on-primary': '#ffffff',
    'on-secondary': '#ffffff',
    'on-background': appBrand.ink,
    'on-surface': appBrand.ink,
  },
} as const

export const appDarkTheme = {
  dark: true,
  colors: {
    primary: '#60a5fa',
    secondary: '#94a3b8',
    accent: '#60a5fa',
    background: '#0f172a',
    surface: '#1e293b',
    'on-primary': '#0f172a',
    'on-secondary': '#0f172a',
    'on-background': '#f1f5f9',
    'on-surface': '#f1f5f9',
  },
} as const
