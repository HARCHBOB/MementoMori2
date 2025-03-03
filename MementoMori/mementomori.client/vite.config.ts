import { fileURLToPath, URL } from 'node:url';

import { defineConfig } from 'vite';
import plugin from '@vitejs/plugin-react';
import fs from 'fs';
import path from 'path';
import child_process from 'child_process';
import { env } from 'process';

const baseFolder =
  env.APPDATA !== undefined && env.APPDATA !== ''
    ? `${env.APPDATA}/ASP.NET/https`
    : `${env.HOME}/.aspnet/https`;

const certificateName = 'mementomori.client';
const certFilePath = path.join(baseFolder, `${certificateName}.pem`);
const keyFilePath = path.join(baseFolder, `${certificateName}.key`);

if (!fs.existsSync(certFilePath) || !fs.existsSync(keyFilePath)) {
  if (
    0 !==
    child_process.spawnSync(
      'dotnet',
      [
        'dev-certs',
        'https',
        '--export-path',
        certFilePath,
        '--format',
        'Pem',
        '--no-password',
      ],
      { stdio: 'inherit' }
    ).status
  ) {
    throw new Error('Could not create certificate.');
  }
}

const target = env.ASPNETCORE_HTTPS_PORT
  ? `https://localhost:${env.ASPNETCORE_HTTPS_PORT}`
  : env.ASPNETCORE_URLS
  ? env.ASPNETCORE_URLS.split(';')[0]
  : 'http://localhost:5176';

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
    port: 5173,
    https: {
      key: fs.readFileSync(keyFilePath),
      cert: fs.readFileSync(certFilePath),
    },
  },
});
