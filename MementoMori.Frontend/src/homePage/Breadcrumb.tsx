import { Breadcrumbs, Link } from '@mui/material';
import { useNavigate, useLocation } from 'react-router-dom';
import { useQuery } from '@tanstack/react-query';
import axios from 'axios';

const DynamicBreadcrumb: React.FC = () => {
  const navigate = useNavigate();
  const location = useLocation();

  // Split pathname by "/" and remove the first empty element
  const pathnameSegments = location.pathname
    .split('/')
    .filter((segment) => segment);

  // Check if we're on the /browser page
  const isBrowserPage = location.pathname === '/browser';

  // Extract deckId from the URL
  const deckIdSegmentIndex = pathnameSegments.indexOf('decks') + 1;
  const deckId =
    deckIdSegmentIndex > 0 && pathnameSegments[deckIdSegmentIndex] !== ''
      ? pathnameSegments[deckIdSegmentIndex]
      : null;

  // Check if we are on the "new deck" route
  const isNewDeck = deckId && deckId === '00000000-0000-0000-0000-000000000000';

  const {
    data: deckTitle,
    isFetched,
    isError,
  } = useQuery<string, Error>({
    queryKey: ['deckTitle', deckId],
    queryFn: async () => {
      if (isNewDeck) return 'New deck'; // Display "New deck" if it's the specific UUID route
      if (!deckId) throw new Error('deckId not found');
      const response = await axios.get(`http://localhost:5173/Decks/${deckId}/DeckTitle`);
      return response.data;
    },
    enabled: !isNewDeck && !!deckId,
  });

  const handleNavigate = (path: string[]) => {
    navigate(`/${path.join('/')}`);
  };

  const scrollToTop = () => {
    window.scrollTo({ top: 0, behavior: 'smooth' });
  };

  const styles = {
    breadcrumb: {
      color: 'indigo',
      fontSize: '1.5rem',
    },
    breadcrumbLink: {
      cursor: 'pointer', // Change cursor to pointer on hover
      '&:hover': {
        textDecoration: 'underline', // Optional: underline on hover
      },
    },
  };

  const capitalize = (str: string) =>
    str.charAt(0).toUpperCase() + str.slice(1);

  return (
    <Breadcrumbs separator="›" aria-label="breadcrumb" sx={styles.breadcrumb}>
      {pathnameSegments.map((pathSegment, index) => {
        if (isBrowserPage) {
          return (
            <Link
              key={index}
              underline="hover"
              color="inherit"
              sx={styles.breadcrumbLink}
              onClick={() => {
                navigate('/browser');
                scrollToTop(); // Scroll to top when clicking the breadcrumb
              }}
            >
              Deck browser
            </Link>
          );
        }

        if (pathSegment === 'decks' && index === 0) {
          return (
            <Link
              key={index}
              underline="hover"
              color="inherit"
              sx={styles.breadcrumbLink}
              onClick={() => {
                navigate('/browser');
                scrollToTop(); // Scroll to top when clicking the breadcrumb
              }}
            >
              Deck browser
            </Link>
          );
        }

        if (
          deckId &&
          deckIdSegmentIndex > 0 &&
          deckId === '00000000-0000-0000-0000-000000000000'
        ) {
          return (
            <Link
              key={index}
              underline="hover"
              color="inherit"
              sx={styles.breadcrumbLink}
              onClick={() => {
                window.location.reload();
                scrollToTop(); // Scroll to top when clicking the breadcrumb
              }}
            >
              New deck
            </Link>
          );
        }

        if (pathSegment === deckId) {
          if (isError) {
            return <div key={index}>Error loading deck title.</div>;
          }
          if (isFetched && deckTitle) {
            return (
              <Link
                key={index}
                underline="hover"
                color="inherit"
                sx={styles.breadcrumbLink}
                onClick={() => {
                  handleNavigate(pathnameSegments.slice(0, index + 1));
                  scrollToTop(); // Scroll to top when clicking the breadcrumb
                }}
              >
                {deckTitle}
              </Link>
            );
          }
          return <div key={index}>Loading...</div>;
        }

        return (
          <Link
            key={index}
            underline="hover"
            color="inherit"
            sx={styles.breadcrumbLink}
            onClick={() => {
              handleNavigate(pathnameSegments.slice(0, index + 1));
              scrollToTop(); // Scroll to top when clicking the breadcrumb
            }}
          >
            {capitalize(pathSegment)}
          </Link>
        );
      })}
    </Breadcrumbs>
  );
};

export default DynamicBreadcrumb;
