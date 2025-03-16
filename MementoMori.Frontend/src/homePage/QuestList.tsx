import * as React from 'react';
import List from '@mui/material/List';
import ListItem from '@mui/material/ListItem';
import ListItemText from '@mui/material/ListItemText';
import ListItemAvatar from '@mui/material/ListItemAvatar';
import Avatar from '@mui/material/Avatar';
import Typography from '@mui/material/Typography';
import AssignmentIcon from '@mui/icons-material/Assignment';
//import CheckCircleIcon from '@mui/icons-material/CheckCircle';
import { useEffect, useState } from 'react';
import axios from 'axios';

interface Quest {
  id: string;
  title: string;
  description: string;
  progress: number;
  required: number;
  reward: string;
  color?: string;
  status?: string;
}

export default function QuestList() {
  const [isLoading, setIsLoading] = useState(true);
  const [questData, setQuestData] = useState<Quest[]>([]);
  //const [isComplete, setIsComplete] = useState(false);
  const [quests, setQuests] = useState<JSX.Element[]>([]);

  useEffect(() => {
    const fetchQuests = async () => {
      try {
        const response = await axios.get('http://localhost:5173/quests.json');
        setQuestData(response.data);
        setIsLoading(false);

        //const isCompleteResponse = await axios.get('http://localhost:5173/Quest/isComplete');
        //setIsComplete(isCompleteResponse.data);
      } catch (error) {
        console.error('Error fetching quests:', error);
      } finally {
        setIsLoading(false); // Ensure loading state is reset even on error
      }
    };

    fetchQuests();
  }, []);

  useEffect(() => {
    if (isLoading) {
      setQuests([
        <Typography
          key="loading-message"
          variant="body2"
          color="text.secondary"
          align="center"
        >
          Loading quests...
        </Typography>,
      ]);
    } else {
      setQuests(
        questData.map((quest) => (
          <ListItem key={quest.id} alignItems="center">
            <ListItemAvatar>
              <Avatar>
                <AssignmentIcon />
              </Avatar>
            </ListItemAvatar>
            <ListItemText
              primary={quest.title}
              sx={{ color: 'black' }}
              secondary={
                <React.Fragment>
                  <Typography
                    component="span"
                    variant="body2"
                    sx={{ display: 'inline' }}
                  >
                    {quest.description}
                  </Typography>
                  {' — ' + quest.progress + '/' + quest.required}
                </React.Fragment>
              }
            />
          </ListItem>
        ))
      );
    }
  }, [isLoading, questData]);

  return (
    <List
      sx={{
        bgcolor: 'white',
        width: '100%',
        maxWidth: 460,
        position: 'absolute',
        border: 1,
        borderRadius: '6px',
        borderColor: '#D4A017',
        borderWidth: 2,
        right: 114,
        top: 55,
        overflowY: 'auto',
      }}
    >
      {quests}
    </List>
  );
}
