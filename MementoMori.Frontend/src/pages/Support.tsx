import React, {useEffect, useState} from 'react';
import {
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Paper,
  Button,
  Typography,
  Chip,
  Box,
} from '@mui/material';
import {CreateTicketModal} from '../modals/CreateTicket';
import {ViewTicketModal} from '../modals/ViewTicket';
import {Ticket} from '../types';

export const SupportPage: React.FC = () => {
  const [role, setRole] = useState<'user' | 'admin'>('user');
  const [tickets, setTickets] = useState<Ticket[]>([]);
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [isViewTicketModalOpen, setIsViewTicketModalOpen] = useState(false);
  const [currentTicket, setCurrentTicket] = useState<Ticket | null>(null);

  // test data
  useEffect(() => {
    setRole('user');
    setTickets([
      {
        id: 1,
        title: 'Login Issue',
        description: 'something more about the issue',
        submissionDate: '2024-03-15',
        status: 'Open',
        user: 'Alice',
        comments: [{text: 'Initial comment from Alice', isAdmin: false}],
      },
      {
        id: 2,
        title: 'Payment Failure',
        description: 'something more about the issue',
        submissionDate: '2024-03-14',
        status: 'In Progress',
        user: 'Bob',
        comments: [{text: 'Payment issue reported by Bob', isAdmin: false}],
      },
      {
        id: 3,
        title: 'Bug in Dashboard',
        description: 'something more about the issue',
        submissionDate: '2024-03-13',
        status: 'Resolved',
        user: 'Charlie',
        comments: [{text: 'Bug fixed by developer', isAdmin: true}],
      },
    ]);
  }, []);

  const handleNewTicket = () => {
    setIsModalOpen(true);
  };

  const handleModalClose = () => {
    setIsModalOpen(false);
  };

  const handleSubmitNewTicket = (title: string, description: string) => {
    const newTicket: Ticket = {
      id: tickets.length + 1,
      title,
      description,
      submissionDate: new Date().toISOString().split('T')[0],
      status: 'Open',
      user: 'Current User',
      comments: [],
    };
    setTickets([...tickets, newTicket]);
    setIsModalOpen(false);
  };

  const handleOpenTicket = (ticket: Ticket) => {
    setCurrentTicket(ticket);
    setIsViewTicketModalOpen(true);
  };

  const handleViewTicketModalClose = () => {
    setIsViewTicketModalOpen(false);
    setCurrentTicket(null);
  };

  const handleSubmitComment = (comment: string, isAdmin: boolean) => {
    if (currentTicket) {
      const updatedTicket = {
        ...currentTicket,
        comments: [...currentTicket.comments, {text: comment, isAdmin}],
      };
      const updatedTickets = tickets.map((ticket) =>
        ticket.id === currentTicket.id ? updatedTicket : ticket,
      );

      setTickets(updatedTickets);
      setCurrentTicket(updatedTicket);
    }
  };

  const handleStatusChange = (ticketId: number, status: 'Open' | 'In Progress' | 'Resolved') => {
    const updatedTickets = tickets.map((ticket) =>
      ticket.id === ticketId ? {...ticket, status} : ticket,
    );
    setTickets(updatedTickets);
  };

  return (
    <>
      {tickets.length === 0 ? (
        <Typography variant='body1'>No tickets found.</Typography>
      ) : (
        <TableContainer component={Paper}>
          <Table>
            <TableHead>
              <TableRow>
                <TableCell sx={{width: '30%'}}>Title</TableCell>
                {role === 'admin' && <TableCell sx={{width: '20%'}}>User</TableCell>}
                <TableCell sx={{width: '30%'}}>Status</TableCell>
                <TableCell>
                  <Box display='flex' alignItems='center' justifyContent='space-between'>
                    <Typography>Submission Date</Typography>
                    {role === 'user' && (
                      <Button
                        size='small'
                        variant='contained'
                        color='primary'
                        onClick={handleNewTicket}
                      >
                        New Ticket
                      </Button>
                    )}
                  </Box>
                </TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {tickets.map((ticket) => (
                <TableRow
                  key={ticket.id}
                  hover
                  sx={{cursor: 'pointer'}}
                  onClick={() => handleOpenTicket(ticket)}
                >
                  <TableCell>{ticket.title}</TableCell>
                  {role === 'admin' && <TableCell>{ticket.user}</TableCell>}
                  <TableCell>
                    <Chip
                      label={ticket.status}
                      color={
                        ticket.status === 'Open'
                          ? 'error'
                          : ticket.status === 'In Progress'
                          ? 'warning'
                          : 'success'
                      }
                    />
                  </TableCell>
                  <TableCell>{ticket.submissionDate}</TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
        </TableContainer>
      )}

      <CreateTicketModal
        open={isModalOpen}
        onClose={handleModalClose}
        onSubmit={handleSubmitNewTicket}
      />

      {currentTicket && (
        <ViewTicketModal
          open={isViewTicketModalOpen}
          onClose={handleViewTicketModalClose}
          ticket={currentTicket}
          onSubmitComment={handleSubmitComment}
          onStatusChange={handleStatusChange}
          role={role}
        />
      )}
    </>
  );
};
