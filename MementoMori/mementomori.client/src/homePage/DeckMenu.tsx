import React, { useEffect, useState } from 'react';
import List from '@mui/material/List';
import ListItem from '@mui/material/ListItem';
import ListItemText from '@mui/material/ListItemText';
import IconButton from '@mui/material/IconButton';
import Button from '@mui/material/Button';
import Typography from '@mui/material/Typography';
import Divider from '@mui/material/Divider';
import DeckCreateButton from './DeckCreateButton.tsx';
import axios from 'axios';
import { Box, Dialog } from '@mui/material';
import DeleteIcon from '@mui/icons-material/Delete';
import DialogTitle from '@mui/material/DialogTitle';
import DialogContent from '@mui/material/DialogContent';
import DialogActions from '@mui/material/DialogActions';

interface Deck {
  id: string;
  title: string;
}
interface UserInformationResponse {
  isLoggedIn: boolean;
  decks: Deck[] | null;
}

export default function DeckMenu() {
  const [isLoggedOn, setIsLoggedOn] = useState<boolean>(false);
  const [decks, setDecks] = useState<Deck[] | null>(null);
  const [collectionDecks, setCollectionDecks] = useState<Deck[] | null>(null);
  const [openDialog, setOpenDialog] = useState<boolean>(false);
  const [deckToDelete, setDeckToDelete] = useState<string | null>(null);

  // Fetch collection decks
  async function fetchCollectionDecks() {
    try {
      const response = await axios.get<UserInformationResponse>(
        `http://localhost:5173/UserDecks/userCollectionDecksController`
      );
      setCollectionDecks(response.data.decks);
    } catch (error) {
      console.error('Error fetching decks in collection:', error);
    }
  }

  useEffect(() => {
    async function fetchDeck() {
      try {
        const response = await axios.get<UserInformationResponse>(
          `http://localhost:5173/UserDecks/userInformation`
        );
        setIsLoggedOn(response.data.isLoggedIn);
        setDecks(response.data.decks);
      } catch (error) {
        console.error('Error fetching deck:', error);
      }
    }
    fetchDeck();
    fetchCollectionDecks();
  }, []);

  const handleDelete = async () => {
    if (deckToDelete) {
      try {
        await axios.post(`http://localhost:5173/UserDecks/userCollectionRemoveDeckController`, {
          Id: deckToDelete,
        });
        await fetchCollectionDecks();
      } catch (error) {
        console.error('Error removing deck:', error);
      } finally {
        setDeckToDelete(null);
        setOpenDialog(false);
      }
    }
  };

  console.log(collectionDecks);

  const handleOpenDialog = (deckId: string) => {
    setDeckToDelete(deckId);
    setOpenDialog(true);
  };

  const handleCloseDialog = () => {
    setDeckToDelete(null);
    setOpenDialog(false);
  };

  return (
    <Box
      sx={{
        boxSizing: 'border-box',
        width: '100%',
        display: 'flex',
        flexDirection: 'column',
        alignItems: 'center',
      }}
    >
      {/* Confirmation Dialog */}
      <Dialog open={openDialog} onClose={handleCloseDialog}>
        <DialogTitle>Confirm Deletion</DialogTitle>
        <DialogContent>
          Are you sure you want to remove this deck from your collection?
        </DialogContent>
        <DialogActions>
          <Button onClick={handleCloseDialog} color='primary'>
            No
          </Button>
          <Button onClick={handleDelete} color='error'>
            Yes
          </Button>
        </DialogActions>
      </Dialog>

      <Typography
        variant='h6'
        sx={{
          color: 'black',
          background: 'white',
          padding: '15px',
          width: '50%',
          textAlign: 'center',
          border: 2,
          borderColor: '#8A2BE2',
          borderRadius: '8px 8px 0 0',
          boxSizing: 'border-box',
        }}
      >
        Decks
      </Typography>

      <Box
        sx={{
          display: 'flex',
          width: '50%',
          border: 2,
          borderColor: '#8A2BE2',
          backgroundColor: 'white',
          boxSizing: 'border-box',
          borderRadius: '0 0 8px 8px',
          overflow: 'hidden',
        }}
      >
        {/* First Column */}
        <Box
          sx={{
            flex: 1,
            borderRight: '2px solid #8A2BE2',
            padding: '10px',
            maxHeight: '400px',
            overflowY: 'auto',
          }}
        >
          <Typography
            variant='subtitle1'
            align='center'
            sx={{ marginBottom: '8px' }}
          >
            Your Collection Decks
          </Typography>
          <List>
            {isLoggedOn ? (
              collectionDecks && collectionDecks.length > 0 ? (
                collectionDecks.map((deck) => (
                  <React.Fragment key={deck.id}>
                    <ListItem
                      disableGutters
                      sx={{
                        cursor: 'pointer',
                        '&:hover': { backgroundColor: '#f0f0f0' },
                      }}
                    >
                      <ListItemText
                        primary={deck.title}
                        onClick={() =>
                          (window.location.href = `http://localhost:5173/decks/${deck.id}`)
                        }
                      />
                      <IconButton onClick={() => handleOpenDialog(deck.id)}>
                        <DeleteIcon />
                      </IconButton>
                    </ListItem>
                    <Divider />
                  </React.Fragment>
                ))
              ) : (
                <Typography align='center'>No decks in collection</Typography>
              )
            ) : (
              <Typography align='center'>Log in to see your decks</Typography>
            )}
          </List>
          <Box
            sx={{
              display: 'flex',
              justifyContent: 'center',
              marginTop: '10px',
            }}
          >
            <Button
              variant='contained'
              color='primary'
              sx={{ borderRadius: '8px', paddingX: '20px' }}
              onClick={() => (window.location.href = '/browser')}
            >
              Go to Deck Browser
            </Button>
          </Box>
        </Box>

        {/* Second Column */}
        <Box
          sx={{
            flex: 1,
            padding: '10px',
            maxHeight: '400px',
            overflowY: 'auto',
          }}
        >
          <Typography
            variant='subtitle1'
            align='center'
            sx={{ marginBottom: '8px' }}
          >
            Your Decks
          </Typography>
          <List>
            {decks && decks.length > 0 ? (
              decks.map((deck) => (
                <React.Fragment key={deck.id}>
                  <ListItem
                    disableGutters
                    onClick={() =>
                      (window.location.href = `http://localhost:5173/decks/${deck.id}`)
                    }
                    sx={{
                      cursor: 'pointer',
                      '&:hover': { backgroundColor: '#f0f0f0' },
                    }}
                  >
                    <ListItemText primary={deck.title} />
                  </ListItem>
                  <Divider />
                </React.Fragment>
              ))
            ) : (
              <Typography align='center'>No decks available</Typography>
            )}
          </List>
          <Box
            sx={{
              display: 'flex',
              justifyContent: 'center',
              marginTop: '10px',
            }}
          >
            <DeckCreateButton />
          </Box>
        </Box>
      </Box>
    </Box>
  );
}
