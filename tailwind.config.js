/** @type {import('tailwindcss').Config} */
module.exports = {
  content: [
    './Pages/**/*.cshtml',
    './Views/**/*.cshtml'
  ],
  theme: {
    extend: {
      transitionProperty: {
        'spacing': 'margin, padding',
      },
      padding: {
        'full': '100%',
      },
      colors: {
        'text': '#1a1a1a',
        'black': '#1a1a1a',
      },
      fontFamily: {
        'outfit': 'Outfit',
        'manrope': 'Manrope'
      }
    },
  },
  plugins: [],
}

