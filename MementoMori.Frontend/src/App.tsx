import { BrowserRouter, Routes, Route } from 'react-router-dom';
import Home from './Home';
import Browser from './deckBrowser/Browser';
import './App.css';
import DeckPage from './mainFunctionality/DeckPage';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { Deck } from './decks/Deck.tsx';
import { Login } from './Login.tsx';
import { Register } from './Register.tsx';
import MainHeader from './homePage/MainHeader';
import EditDeck from './Editor/DeckEditor.tsx';
import Shop from './shop/Shop.tsx';

const client = new QueryClient();

function App() {
  return (
    <QueryClientProvider client={client}>
      <BrowserRouter>
        <MainHeader />
        <Routes>
          <Route path='/' element={<Home />} />
          <Route path='/decks/:deckId/edit' element={<EditDeck />} />
          <Route path='/decks/:deckId' element={<Deck />} />
          <Route path='/decks/:deckId/practice' element={<DeckPage />} />
          <Route path='/browser' element={<Browser />} />
          <Route path='/login' element={<Login />} />
          <Route path='/register' element={<Register />} />
          <Route path='/shop' element={<Shop />} />
        </Routes>
      </BrowserRouter>
    </QueryClientProvider>
  );
}

export default App;
