import { useEffect, useState } from 'react';
import axios from 'axios';
import { useMutation } from '@tanstack/react-query';

import {
  Chip,
  Paper,
  TableRow,
  TableHead,
  TableContainer,
  TableCell,
  TableBody,
  CircularProgress,
  Box,
  Table,
  Link,
} from '@mui/material';
import SearchBar from './SearchBar';
import { Tag } from './TagSelector';
import TagSelector from './TagSelector';

export type browserRowData = {
  id: string;
  title: string;
  rating: number;
  modified: string;
  cards: number;
  tags: Tag[];
};

export default function Browser() {
  const [searchString, setSearchString] = useState<string>('');
  const [selectedTags, setSelectedTags] = useState<string[]>([]);
  const [tableRows, setTableRows] = useState<browserRowData[]>([]);

  const { mutate, isPending } = useMutation({
    mutationFn: (params: { searchString: string; selectedTags: string[] }) => {
      return axios.get<browserRowData[]>('http://localhost:5173/DeckBrowser/getDecks', {
        params: params,
        paramsSerializer: { indexes: null },
      });
    },
    onSuccess: (response) => {
      setTableRows(response.data);
    },
  });

  useEffect(() => {
    mutate({ searchString, selectedTags });
  }, [searchString, selectedTags, mutate]);

  return (
    <Box
      sx={{
        flexDirection: 'column',
        display: 'flex',
        alignItems: 'flex-start',
        justifyContent: 'flex-start',
        minWidth: '100%',
      }}
    >
      <Box
        sx={{
          width: '100%',
          flexDirection: 'row',
          display: 'flex',
          justifyContent: 'space-between',
          alignItems: 'center',
        }}
      >
        <h2>Shared decks</h2>

        <Box
          sx={{
            flexDirection: 'row',
            display: 'flex',
            alignItems: 'center',
            gap: 1,
          }}
        >
          <TagSelector setSelectedTags={setSelectedTags} />
          <SearchBar setSearchString={setSearchString} />
        </Box>
      </Box>
      <TableContainer component={Paper}>
        <Table sx={{ minWidth: 650 }} aria-label="simple table">
          <TableHead>
            <TableRow>
              <TableCell>
                <b>Title</b>
              </TableCell>
              <TableCell align="right">
                <b>Rating</b>
              </TableCell>
              <TableCell align="right">
                <b>Modified</b>
              </TableCell>
              <TableCell align="right">
                <b>Cards</b>
              </TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {isPending ? (
              <TableRow>
                <TableCell colSpan={4} align="center">
                  <CircularProgress />
                </TableCell>
              </TableRow>
            ) : (
              tableRows.map((deck) => (
                <TableRow
                  key={deck.id}
                  sx={{ '&:last-child td, &:last-child th': { border: 0 } }}
                >
                  <TableCell component="th" scope="row">
                    <Box
                      sx={{
                        display: 'flex',
                        alignItems: 'center',
                        flexDirection: 'row',
                        gap: 1,
                      }}
                    >
                      <Link href={`/decks/${deck.id}`} underline="hover">
                        <b>{deck.title}</b>
                      </Link>
                      {deck.tags?.map((tag) => (
                        <Chip label={tag} variant="outlined" />
                      ))}
                    </Box>
                  </TableCell>
                  <TableCell align="right">{deck.rating}</TableCell>
                  <TableCell align="right">{deck.modified}</TableCell>
                  <TableCell align="right">{deck.cards}</TableCell>
                </TableRow>
              ))
            )}
          </TableBody>
        </Table>
      </TableContainer>
    </Box>
  );
}
