import React, { useState } from 'react';
import FormControl from '@mui/joy/FormControl';
import Input from '@mui/joy/Input';
import Button from '@mui/joy/Button';

export default function SearchBar(props: {
  setSearchString: React.Dispatch<React.SetStateAction<string>>;
}) {
  const [data, setData] = useState<string>('');

  return (
    <form
      onSubmit={(e) => {
        e.preventDefault();
        props.setSearchString(data);
      }}
      id="browserSearchBar"
    >
      <FormControl>
        <Input
          sx={{ '--Input-decoratorChildHeight': '45px' }}
          placeholder="Search..."
          value={data}
          onChange={(event) => setData(event.target.value)}
          endDecorator={
            <Button
              variant="solid"
              color="primary"
              type="submit"
              sx={{ borderTopLeftRadius: 0, borderBottomLeftRadius: 0 }}
            >
              Search
            </Button>
          }
        />
      </FormControl>
    </form>
  );
}
