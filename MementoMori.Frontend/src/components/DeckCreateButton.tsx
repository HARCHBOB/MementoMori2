import {Button} from '@mui/material';
import {useNavigate} from 'react-router-dom';

const RedirectButton = () => {
  const navigate = useNavigate();
  const NEW_DECK_URL = '/decks/00000000-0000-0000-0000-000000000000/edit';

  const handleRedirect = () => navigate(NEW_DECK_URL);

  return (
    <Button
      variant='contained'
      color='primary'
      fullWidth
      sx={{
        flex: 1,
        mx: 1,
        borderRadius: 2,
        maxWidth: '100%',
      }}
      onClick={handleRedirect}
    >
      Create a New Deck
    </Button>
  );
};

export default RedirectButton;
