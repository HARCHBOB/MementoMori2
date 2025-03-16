import React, {useState} from 'react';
import {Dialog, DialogContent, IconButton, Tabs, Tab} from '@mui/material';
import CloseIcon from '@mui/icons-material/Close';
import {Login} from './Login';
import {Register} from './Register';

export type AuthDialogProps = {
  closeCallback: () => void;
  isAuthenticatedCallback?: () => void;
};

export function AuthDialog(props: AuthDialogProps) {
  const [tabIndex, setTabIndex] = useState(0);

  const handleClose = () => {
    props.closeCallback();
  };

  const handleTabChange = (_e: React.SyntheticEvent, newValue: number) => {
    setTabIndex(newValue);
  };

  return (
    <>
      <Dialog open onClose={handleClose} maxWidth='xs' fullWidth>
        <IconButton
          aria-label='close'
          onClick={handleClose}
          sx={{
            position: 'absolute',
            right: 8,
            top: 8,
          }}
        >
          <CloseIcon />
        </IconButton>
        <DialogContent dividers>
          <Tabs value={tabIndex} onChange={handleTabChange} centered>
            <Tab label='Login' />
            <Tab label='Register' />
          </Tabs>
          {tabIndex === 0 ? <Login {...props} /> : <Register {...props} />}
        </DialogContent>
      </Dialog>
    </>
  );
}
