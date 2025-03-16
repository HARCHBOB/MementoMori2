import React, {useState, useCallback} from 'react';
import FormControl from '@mui/joy/FormControl';
import Input from '@mui/joy/Input';
import Button from '@mui/joy/Button';

export default function SearchBar({
  setSearchString,
}: {
  setSearchString: React.Dispatch<React.SetStateAction<string>>;
}) {
  const [query, setQuery] = useState('');

  const handleChange = useCallback((event: React.ChangeEvent<HTMLInputElement>) => {
    setQuery(event.target.value);
  }, []);

  const handleSubmit = (event: React.FormEvent) => {
    event.preventDefault();
    if (query.trim()) {
      setSearchString(query);
    }
  };

  return (
    <form onSubmit={handleSubmit} id='browserSearchBar'>
      <FormControl>
        <Input
          sx={{'--Input-decoratorChildHeight': '45px'}}
          placeholder='Search...'
          value={query}
          onChange={handleChange}
          aria-label='Search input'
          endDecorator={
            <Button
              variant='solid'
              color='primary'
              type='submit'
              sx={{borderTopLeftRadius: 0, borderBottomLeftRadius: 0}}
              disabled={!query.trim()}
            >
              Search
            </Button>
          }
        />
      </FormControl>
    </form>
  );
}
