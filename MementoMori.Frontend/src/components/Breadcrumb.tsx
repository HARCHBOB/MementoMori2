import {Breadcrumbs, Link} from '@mui/material';
import {useNavigate, useLocation} from 'react-router-dom';
import {useQuery} from '@tanstack/react-query';
import axios from 'axios';

const fetchDeckTitle = async (deckId: string) => {
  if (deckId === '00000000-0000-0000-0000-000000000000') return 'New deck';
  if (!deckId) throw new Error('deckId not found');
  const response = await axios.get(`http://localhost:5173/Decks/${deckId}/DeckTitle`);
  return response.data;
};

const BreadcrumbLink = ({label, onClick}: {label: string; onClick: () => void}) => (
  <Link
    underline='hover'
    color='inherit'
    sx={{cursor: 'pointer', '&:hover': {textDecoration: 'underline'}}}
    onClick={onClick}
  >
    {label}
  </Link>
);

const DynamicBreadcrumb: React.FC = () => {
  const navigate = useNavigate();
  const location = useLocation();
  const pathnameSegments = location.pathname.split('/').filter(Boolean);
  const isBrowserPage = location.pathname === '/browser';

  const deckId = pathnameSegments.includes('decks')
    ? pathnameSegments[pathnameSegments.indexOf('decks') + 1]
    : null;
  const isNewDeck = deckId === '00000000-0000-0000-0000-000000000000';

  const {
    data: deckTitle,
    isFetched,
    isError,
  } = useQuery({
    queryKey: ['deckTitle', deckId],
    queryFn: () => fetchDeckTitle(deckId!),
    enabled: !!deckId && !isNewDeck,
  });

  const handleNavigate = (path: string[]) => navigate(`/${path.join('/')}`);
  const capitalize = (str: string) => str.charAt(0).toUpperCase() + str.slice(1);

  return (
    <Breadcrumbs separator='›' aria-label='breadcrumb' sx={{color: 'indigo', fontSize: '1.5rem'}}>
      {isBrowserPage ? (
        <BreadcrumbLink label='Deck browser' onClick={() => navigate('/browser')} />
      ) : (
        pathnameSegments.map((segment, index) => {
          if (segment === 'decks' && index === 0) {
            return (
              <BreadcrumbLink
                key={index}
                label='Deck browser'
                onClick={() => navigate('/browser')}
              />
            );
          }

          if (segment === deckId) {
            if (isError) return <div key={index}>Error loading deck title.</div>;
            if (!isFetched) return <div key={index}>Loading...</div>;
            return (
              <BreadcrumbLink
                key={index}
                label={deckTitle!}
                onClick={() => handleNavigate(pathnameSegments.slice(0, index + 1))}
              />
            );
          }

          return (
            <BreadcrumbLink
              key={index}
              label={capitalize(segment)}
              onClick={() => handleNavigate(pathnameSegments.slice(0, index + 1))}
            />
          );
        })
      )}
    </Breadcrumbs>
  );
};

export default DynamicBreadcrumb;
