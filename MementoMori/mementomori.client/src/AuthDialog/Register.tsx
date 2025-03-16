import React, { useState } from 'react';
import {
  TextField,
  Button,
  Container,
  Alert,
  FormControl,
  CircularProgress,
  Checkbox,
  FormControlLabel,
} from '@mui/material';
import { useMutation } from '@tanstack/react-query';
import axios from 'axios';
import { AuthDialogProps } from './AuthDialog';

export function Register(props: AuthDialogProps) {
  const [username, setUsername] = useState<string>('');
  const [password, setPassword] = useState<string>('');
  const [error, setError] = useState<string>('');
  const [rememberMe, setRememberMe] = useState<boolean>(false);

  const { mutate: register, isPending } = useMutation({
    mutationFn: () => {
      return axios.post<string>('http://localhost:5173/auth/register', {
        username,
        password,
        rememberMe,
      });
    },
    onSuccess: () => {
      if (props.isAuthenticatedCallback) {
        props.isAuthenticatedCallback();
      }
      location.reload();
      props.closeCallback();
    },
    onError: () => {
      setError('Register failed. This username already exists.');
    },
  });

  const handleSubmit = (event: React.FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    setError('');
    register();
  };

  return (
    <Container maxWidth='xs'>
      <form onSubmit={handleSubmit}>
        <FormControl fullWidth>
          <TextField
            label='Username'
            variant='outlined'
            margin='normal'
            value={username}
            onChange={(e) => setUsername(e.target.value)}
            required
          />
          <TextField
            label='Password'
            variant='outlined'
            type='password'
            margin='normal'
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            required
          />
          <FormControlLabel
            control={
              <Checkbox
                checked={rememberMe}
                onChange={(e) => setRememberMe(e.target.checked)}
                color='primary'
              />
            }
            label='Remember Me'
          />
          {error && <Alert severity='error'>{error}</Alert>}
          {isPending ? (
            <Button
              type='submit'
              variant='contained'
              color='primary'
              disabled
              style={{ marginTop: '16px' }}
            >
              <CircularProgress size={24} />
            </Button>
          ) : (
            <Button
              type='submit'
              variant='contained'
              color='primary'
              style={{ marginTop: '16px' }}
            >
              Register
            </Button>
          )}
        </FormControl>
      </form>
    </Container>
  );
}
