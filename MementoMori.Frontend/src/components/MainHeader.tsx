import Box from '@mui/material/Box';
import Avatar from '@mui/material/Avatar';
import Menu from '@mui/material/Menu';
import MenuItem from '@mui/material/MenuItem';
import ListItemIcon from '@mui/material/ListItemIcon';
import Divider from '@mui/material/Divider';
import IconButton from '@mui/material/IconButton';
import Tooltip from '@mui/material/Tooltip';
import Settings from '@mui/icons-material/Settings';
import Logout from '@mui/icons-material/Logout';
import Button from '@mui/material/Button';
import HomeIcon from '@mui/icons-material/Home';
import Breadcrumb from './Breadcrumb.tsx';
import React, {useState, useEffect} from 'react';
import axios from 'axios';
import {AuthDialog} from '../modals/AuthDialog.tsx';
import {useNavigate} from 'react-router-dom';

export default function MainHeader() {
  const navigate = useNavigate();
  const [isLoggedIn, setIsLoggedIn] = useState(false);
  const [isAuthDialogVisible, setIsAuthDialogVisible] = useState(false);
  const [anchorEl, setAnchorEl] = useState<null | HTMLElement>(null);
  const open = Boolean(anchorEl);

  useEffect(() => {
    const fetchLoginStatus = async () => {
      try {
        const response = await axios.get('http://localhost:5173/auth/loginResponse');
        setIsLoggedIn(response.data);
      } catch (error) {
        console.error('Error fetching login status:', error);
      }
    };
    fetchLoginStatus();
  }, []);

  const handleAuthDialogClose = async () => {
    setIsAuthDialogVisible(false);
    try {
      const response = await axios.get('http://localhost:5173/auth/loginResponse');
      setIsLoggedIn(response.data);
    } catch (error) {
      console.error('Error updating login status:', error);
    }
  };

  const handleClick = (event: React.MouseEvent<HTMLElement>) => setAnchorEl(event.currentTarget);
  const handleClose = () => setAnchorEl(null);

  const handleLogout = async () => {
    try {
      const response = await axios.post('http://localhost:5173/auth/logout');
      if (response.status === 200) {
        setIsLoggedIn(false);
        window.location.reload();
      } else {
        console.error('Error logging out:', response.data);
      }
    } catch (error) {
      console.error('Error logging out:', error);
    }
  };

  return (
    <>
      {isAuthDialogVisible ? <AuthDialog closeCallback={handleAuthDialogClose} /> : null}
      <Box
        sx={{
          position: 'fixed',
          top: 0,
          left: 106,
          display: 'flex',
          alignItems: 'center',
          minWidth: '85%',
          border: 1,
          borderRadius: '6px',
          borderColor: 'purple',
          borderWidth: 2,
          textAlign: 'center',
          justifyContent: 'space-between',
          bgcolor: 'white',
          gap: 2,
          zIndex: 99,
        }}
      >
        <Box sx={{display: 'flex', alignItems: 'center'}}>
          <Tooltip title='Return home'>
            <IconButton onClick={() => navigate('/')} sx={{cursor: 'pointer'}}>
              <Avatar sx={{width: 32, height: 32, color: 'indigo'}}>
                <HomeIcon />
              </Avatar>
            </IconButton>
          </Tooltip>
          <Breadcrumb />
        </Box>

        <Box sx={{display: 'flex', alignItems: 'center', gap: 2}}>
          <Button
            sx={{minWidth: 80, color: 'indigo', fontSize: 18, textTransform: 'capitalize'}}
            variant='text'
            onClick={() => navigate('/support')}
          >
            Support
          </Button>
          <Button
            sx={{minWidth: 80, color: 'indigo', fontSize: 18, textTransform: 'capitalize'}}
            variant='text'
            onClick={() => navigate('/shop')}
          >
            Shop
          </Button>
          <Button
            sx={{minWidth: 80, color: 'indigo', fontSize: 18, textTransform: 'capitalize'}}
            variant='text'
            onClick={() => navigate('/browser')}
          >
            Deck browser
          </Button>

          {isLoggedIn ? (
            <Tooltip title='Account settings'>
              <IconButton onClick={handleClick} size='small' sx={{ml: 2}}>
                <Avatar sx={{width: 32, height: 32}}>D</Avatar>
              </IconButton>
            </Tooltip>
          ) : (
            <Button
              sx={{minWidth: 80, color: 'indigo', fontSize: 18, textTransform: 'capitalize'}}
              variant='text'
              onClick={() => setIsAuthDialogVisible(true)}
            >
              Log In
            </Button>
          )}
        </Box>
      </Box>

      <Menu
        anchorEl={anchorEl}
        id='account-menu'
        open={open}
        onClose={handleClose}
        onClick={handleClose}
        slotProps={{
          paper: {
            elevation: 0,
            sx: {
              border: 2,
              borderColor: '#D4A017',
              borderRadius: 2,
              bgcolor: 'white',
              overflow: 'visible',
              mt: 1.5,
              filter: 'drop-shadow(0px 2px 8px rgba(0,0,0,0.32))',
              '& .MuiAvatar-root': {
                width: 32,
                height: 32,
                ml: -0.5,
                mr: 1,
              },
              '&::before': {
                content: '""',
                display: 'block',
                position: 'absolute',
                top: 0,
                right: 14,
                width: 10,
                height: 10,
                transform: 'translateY(-50%) rotate(45deg)',
                zIndex: 0,
              },
            },
          },
        }}
        transformOrigin={{horizontal: 'right', vertical: 'top'}}
        anchorOrigin={{horizontal: 'right', vertical: 'bottom'}}
      >
        <MenuItem onClick={handleClose}>
          <Avatar /> My account
        </MenuItem>
        <Divider />
        <MenuItem onClick={handleClose}>
          <ListItemIcon>
            <Settings sx={{color: 'black'}} fontSize='small' />
          </ListItemIcon>
          Settings
        </MenuItem>
        <MenuItem onClick={handleLogout}>
          <ListItemIcon>
            <Logout sx={{color: 'black'}} fontSize='small' />
          </ListItemIcon>
          Logout
        </MenuItem>
      </Menu>
    </>
  );
}
