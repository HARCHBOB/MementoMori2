import React, {useState} from 'react';
import axios from 'axios';
import Snackbar from '@mui/material/Snackbar';
import Alert from '@mui/material/Alert';

const Shop: React.FC = () => {
  const [snackbarOpen, setSnackbarOpen] = useState(false);
  const [snackbarMessage, setSnackbarMessage] = useState('');

  const updateHeaderColor = async (newColor: string) => {
    try {
      const response = await axios.post('http://localhost:5173/Shop/newColor', {
        newColor,
      });
      if (response.status === 200) {
        setSnackbarMessage(`Card color updated to ${newColor}`);
        setSnackbarOpen(true);
      } else {
        console.error('Failed to update header color:', response.data);
      }
    } catch (error) {
      console.error('Error updating card color:', error);
    }
  };

  const handleCloseSnackbar = () => {
    setSnackbarOpen(false);
  };

  return (
    <div style={{padding: '20px'}}>
      <h1>Shop Page</h1>
      <p>Select a card background color:</p>
      <div>
        <button
          onClick={() => updateHeaderColor('lightblue')}
          style={{
            backgroundColor: 'lightblue',
            margin: '5px',
            padding: '10px',
            border: 'none',
            cursor: 'pointer',
          }}
        >
          Light Blue
        </button>
        <button
          onClick={() => updateHeaderColor('lightgreen')}
          style={{
            backgroundColor: 'lightgreen',
            margin: '5px',
            padding: '10px',
            border: 'none',
            cursor: 'pointer',
          }}
        >
          Light Green
        </button>
        <button
          onClick={() => updateHeaderColor('lightpink')}
          style={{
            backgroundColor: 'lightpink',
            margin: '5px',
            padding: '10px',
            border: 'none',
            cursor: 'pointer',
          }}
        >
          Light Pink
        </button>
        <button
          onClick={() => updateHeaderColor('lavender')}
          style={{
            backgroundColor: 'lavender',
            margin: '5px',
            padding: '10px',
            border: 'none',
            cursor: 'pointer',
          }}
        >
          Lavender
        </button>
        <button
          onClick={() => updateHeaderColor('white')}
          style={{
            backgroundColor: 'white',
            margin: '5px',
            padding: '10px',
            border: '1px solid black',
            cursor: 'pointer',
          }}
        >
          Reset to White
        </button>
      </div>
      <Snackbar
        open={snackbarOpen}
        autoHideDuration={3000}
        onClose={handleCloseSnackbar}
        anchorOrigin={{vertical: 'top', horizontal: 'center'}}
      >
        <Alert onClose={handleCloseSnackbar} severity='success' sx={{width: '100%'}}>
          {snackbarMessage}
        </Alert>
      </Snackbar>
    </div>
  );
};

export default Shop;
