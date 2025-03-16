import {BrowserRouter, Routes, Route} from 'react-router-dom';
import Home from './pages/Home.tsx';
import Browser from './pages/Browser.tsx';
import './App.css';
import DeckPage from './pages/DeckPage.tsx';
import {QueryClient, QueryClientProvider} from '@tanstack/react-query';
import {Deck} from './components/Deck.tsx';
import MainHeader from './components/MainHeader.tsx';
import EditDeck from './pages/DeckEditor.tsx';
import Shop from './pages/Shop.tsx';

const client = new QueryClient();

const App = () => {
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
          <Route path='/shop' element={<Shop />} />
        </Routes>
      </BrowserRouter>
    </QueryClientProvider>
  );
};

export default App;
