import React, {useState} from 'react';
import {
  Modal,
  Box,
  Typography,
  TextField,
  Button,
  Chip,
  List,
  ListItem,
  Select,
  MenuItem,
  FormControl,
  SelectChangeEvent,
} from '@mui/material';
import {Ticket} from '../types';

interface ViewTicketModalProps {
  open: boolean;
  onClose: () => void;
  ticket: Ticket;
  onSubmitComment: (comment: string, isAdmin: boolean) => void;
  onStatusChange: (ticketId: number, status: 'Open' | 'In Progress' | 'Resolved') => void;
  role: 'user' | 'admin';
}

export const ViewTicketModal: React.FC<ViewTicketModalProps> = ({
  open,
  onClose,
  ticket,
  onSubmitComment,
  onStatusChange,
  role,
}) => {
  const [comment, setComment] = useState('');
  const [status, setStatus] = useState(ticket.status);

  const handleCommentSubmit = () => {
    if (comment) {
      onSubmitComment(comment, role === 'admin');
      setComment('');
    }
  };

  const handleStatusChange = (event: SelectChangeEvent<string>) => {
    const newStatus = event.target.value as 'Open' | 'In Progress' | 'Resolved';
    setStatus(newStatus);
    onStatusChange(ticket.id, newStatus);
  };

  return (
    <Modal open={open} onClose={onClose}>
      <Box
        sx={{
          position: 'absolute',
          top: '50%',
          left: '50%',
          transform: 'translate(-50%, -50%)',
          width: 500,
          bgcolor: 'background.paper',
          boxShadow: 24,
          p: 3,
          display: 'flex',
          flexDirection: 'column',
          gap: 2,
          maxHeight: 600,
        }}
      >
        <Box display='flex' alignItems='center' justifyContent='space-between'>
          <Typography variant='h6'>Ticket Details</Typography>

          {role === 'user' ? (
            <Chip
              label={ticket.status}
              color={
                ticket.status === 'Open'
                  ? 'error'
                  : ticket.status === 'In Progress'
                  ? 'warning'
                  : 'success'
              }
              sx={{mb: 2}}
            />
          ) : (
            <FormControl>
              <Select
                value={status}
                onChange={handleStatusChange}
                size='small'
                sx={{width: '150px'}}
              >
                <MenuItem value='Open'>Open</MenuItem>
                <MenuItem value='In Progress'>In Progress</MenuItem>
                <MenuItem value='Resolved'>Resolved</MenuItem>
              </Select>
            </FormControl>
          )}
        </Box>

        <Typography variant='body2'>Title: {ticket.title}</Typography>
        <Typography variant='body2' sx={{mb: 1}}>
          Description: {ticket.description}
        </Typography>

        <Typography variant='body2'>Comments</Typography>
        <List sx={{maxHeight: 200, overflowY: 'auto'}}>
          {ticket.comments.map((comment, index) => (
            <ListItem key={index}>
              <Typography variant='body2'>
                {comment.isAdmin ? (
                  <>Customer Support: {comment.text}</>
                ) : (
                  <>
                    {ticket.user}: {comment.text}
                  </>
                )}
              </Typography>
            </ListItem>
          ))}
        </List>

        <Typography variant='body2'>Add new comment</Typography>
        <TextField
          fullWidth
          multiline
          rows={3}
          value={comment}
          onChange={(e) => setComment(e.target.value)}
        />
        <Box display='flex' justifyContent='end' gap={2} sx={{mt: 2}}>
          <Button variant='outlined' onClick={onClose}>
            Close
          </Button>
          <Button variant='contained' onClick={handleCommentSubmit} disabled={!comment}>
            Add Comment
          </Button>
        </Box>
      </Box>
    </Modal>
  );
};
