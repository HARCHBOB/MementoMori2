import {Box, Button, Modal, TextField, Typography} from '@mui/material';
import {useState} from 'react';

export const CreateTicketModal: React.FC<{
  open: boolean;
  onClose: () => void;
  onSubmit: (title: string, description: string) => void;
}> = ({open, onClose, onSubmit}) => {
  const [title, setTitle] = useState('');
  const [description, setDescription] = useState('');

  const handleSubmit = () => {
    if (title && description) {
      onSubmit(title, description);
      setTitle('');
      setDescription('');
    }
  };

  return (
    <Modal open={open} onClose={onClose}>
      <Box
        sx={{
          position: 'absolute',
          top: '50%',
          left: '50%',
          transform: 'translate(-50%, -50%)',
          width: 400,
          bgcolor: 'background.paper',
          boxShadow: 24,
          p: 3,
          display: 'flex',
          flexDirection: 'column',
          gap: 2,
        }}
      >
        <Typography variant='h6'>Create New Ticket</Typography>
        <Box>
          <Typography variant='caption' sx={{mb: 1}}>
            Title
          </Typography>
          <TextField fullWidth value={title} onChange={(e) => setTitle(e.target.value)} />
        </Box>
        <Box>
          <Typography variant='caption' sx={{mb: 1}}>
            Description
          </Typography>
          <TextField
            fullWidth
            multiline
            rows={4}
            value={description}
            onChange={(e) => setDescription(e.target.value)}
          />
        </Box>
        <Box display='flex' justifyContent='end' gap={2}>
          <Button variant='outlined' onClick={onClose}>
            Cancel
          </Button>
          <Button variant='contained' onClick={handleSubmit} disabled={!title || !description}>
            Submit
          </Button>
        </Box>
      </Box>
    </Modal>
  );
};
