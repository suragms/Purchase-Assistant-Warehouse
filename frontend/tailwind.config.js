/** @type {import('tailwindcss').Config} */
export default {
  content: [
    "./index.html",
    "./src/**/*.{js,ts,jsx,tsx}",
  ],
  theme: {
    extend: {
      colors: {
        brand: {
          primary: '#0E4F46',
          secondary: '#065F4F',
          accent: '#159A8A',
          gold: '#D4AF37',
          bg: '#F7F9F6',
          card: '#FFFFFF',
          border: '#E2E8E6',
          text: '#0F172A',
          muted: '#64748B'
        }
      },
      fontFamily: {
        sans: ['"Plus Jakarta Sans"', 'sans-serif'],
      }
    },
  },
  plugins: [],
}