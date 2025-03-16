import { useEffect, useState } from 'react';
import '@fontsource/roboto/300.css';
import '@fontsource/roboto/400.css';
import '@fontsource/roboto/500.css';
import '@fontsource/roboto/700.css';

import * as React from 'react';
import Box from '@mui/material/Box';
import Card from '@mui/material/Card';
import CardActions from '@mui/material/CardActions';
import CardContent from '@mui/material/CardContent';
import Button from '@mui/material/Button';
import Typography from '@mui/material/Typography';

const bull = (
    <Box
        component="span"
        sx={{ display: 'inline-block', mx: '2px', transform: 'scale(0.8)' }}
    >
        •
    </Box>
);

export default function BasicCard() {
    const [show, setShow] = useState(false)
    return (
        <Card sx={{ minWidth: 400, minHeight: 500, textAlign: 'center' }}>
            <CardContent>
                <Typography variant="h5" component="div">
                    be{bull}nev{bull}o{bull}lent
                </Typography>
                <Typography sx={{ color: 'text.secondary', mb: 1.5 }}>adjective</Typography>
                {show ?  
                    
                <Typography variant="body2">
                    well meaning and kindly.
                    <br />
                    {'"a benevolent smile"'}
                </Typography> : null
                }
            </CardContent>
            <CardActions style={{ justifyContent: 'center' }}>
                <Button variant="outlined" size='large' onClick={() => { setShow(true)} }> Reveal</Button> 
            </CardActions>
            <CardActions>
                
                <Button variant="outlined"> New </Button>
                <Button variant="outlined" color="success"> Remembered </Button>
                <Button variant="outlined" color="error"> Forgot </Button>
            </CardActions>
        </Card> 
    );
}
