import { fileURLToPath, URL } from 'node:url';

import { defineConfig } from 'vite';
import plugin from '@vitejs/plugin-react';

const target = 'http://localhost:8080'

// https://vitejs.dev/config/
export default defineConfig({
  plugins: [plugin()],
  resolve: {
    alias: {
      '@': fileURLToPath(new URL('./src', import.meta.url)),
    },
  },
  server: {
    proxy: {
      '^/DeckBrowser/getDecks': {
        target,
        secure: false,
      },
      '^/decks/.*?/cards': {
        target,
        secure: false,
      },
      '^/Decks/.*?/editDeck': {
        target,
        secure: false,
      },
      '^/Decks/.*?/EditorView': {
        target,
        secure: false,
      },
      '^/Decks/.*?/createDeck': {
        target,
        secure: false,
      },
      '^/Decks/.*?/deck': {
        target,
        secure: false,
      },
      '^/Decks/.*?/addToCollection': {
        target,
        secure: false,
      },
      '^/Decks/.*?/deleteDeck': {
        target,
        secure: false,
      },
      '^/Decks/.*?/cards/update/.*?': {
        target,
        secure: false,
      },
      '^/Decks/.*?/DeckTitle': {
        target,
        secure: false,
},
      '^/QuestController/isComplete': {
        target,
        secure: false,
      },
      '^/auth/login': {
        target,
        secure: false,
      },
      '^/auth/register': {
        target,
        secure: false,
      },
      '^/auth/logout': {
        target,
        secure: false,
      },
      '^/auth/loginResponse': {
        target,
        secure: false,
      },
      '^/UserDecks/userInformation': {
        target,
        secure: false,
      },
      '^/color/color': {
        target,
        secure: false,
      },
      '^/Shop/newColor': {
        target,
        secure: false,
      },
      '^/UserDecks/userCollectionDecksController': {
        target,
        secure: false,
      },
      '^/UserDecks/userCollectionRemoveDeckController': {
        target,
        secure: false,
      },
      '^/QuestController/quests': {
        target,
        secure: false,
      },
    },
    port: 5173
  },
});
