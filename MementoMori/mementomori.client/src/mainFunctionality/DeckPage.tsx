import React, { useState, useEffect } from 'react';
import { useParams } from 'react-router-dom';

interface Card {
  id: string;
  question: string;
  description?: string | null;
  answer: string;
  lastReviewed?: string;
}

export default function DeckPage() {
  const { deckId } = useParams<{ deckId: string }>();
  const [cards, setCards] = useState<Card[]>([]);
  const [currentIndex, setCurrentIndex] = useState(0);
  const [currentCard, setCurrentCard] = useState<Card | null>(null);
  const [showAnswer, setShowAnswer] = useState(false);
  const [color, setColor] = useState('white');

  useEffect(() => {
    const fetchCards = async () => {
      try {
        const response = await fetch(
          `http://localhost:5173/decks/${deckId}/cards`
        );
        const text = await response.text();
        console.log('Raw Response:', text);

        try {
          const data = JSON.parse(text);
          setColor(data.color);
          setCards(data.cards);
          if (data.cards.length > 0) setCurrentCard(data.cards[0]);
        } catch (jsonError) {
          console.error('Failed to parse JSON:', jsonError);
        }
      } catch (error) {
        console.error('Failed to fetch cards', error);
      }
    };

    fetchCards();
  }, [deckId]);

  const handleQualitySubmit = async (quality: number) => {
    if (!currentCard) return;

    try {
      const response = await fetch(
        `http://localhost:5173/Decks/${deckId}/cards/update/${currentCard.id}`,
        {
          method: 'POST',
          headers: { 'Content-Type': 'application/json' },
          body: JSON.stringify(quality),
        }
      );

      if (response.ok) {
        console.log(`Successfully updated card with quality: ${quality}`);

        setCards((prevCards) => {
          const updatedCards = prevCards.filter(
            (card) => card.id !== currentCard.id
          );
          if (updatedCards.length > 0) {
            setCurrentIndex(0);
            setCurrentCard(updatedCards[0]);
          } else {
            setCurrentCard(null);
          }
          return updatedCards;
        });

        setShowAnswer(false);
      } else {
        console.error('Failed to update card');
      }
    } catch (error) {
      console.error('Error updating card', error);
    }
  };

  return (
    <div>
      {currentCard ? (
        <div
          className='card-container'
          style={{ display: 'flex', justifyContent: 'center' }}
        >
          <div
            className='card'
            style={{
              backgroundColor: color,
              boxShadow: '0px 4px 12px rgba(0, 0, 0, 0.2)', // Elevation effect
              borderRadius: '12px',
              padding: '20px',
              maxWidth: '500px',
              width: '100%',
              textAlign: 'center',
              margin: '20px',
            }}
          >
            <h2 style={{ margin: '0 0 10px', color: '#333' }}>
              {currentCard.question}
            </h2>
            {currentCard.description && (
              <p style={{ color: '#555', marginBottom: '15px' }}>
                {currentCard.description}
              </p>
            )}

            {showAnswer ? (
              <p>
                <strong>Answer: </strong>
                {currentCard.answer ?? 'No answer provided'}
              </p>
            ) : (
              <button
                onClick={() => setShowAnswer(true)}
                style={{
                  margin: '10px',
                  padding: '8px 16px',
                  cursor: 'pointer',
                  border: 'none',
                  borderRadius: '8px',
                }}
              >
                Reveal Answer
              </button>
            )}

            <div
              className='spaced-repetition-buttons'
              style={{ marginTop: '20px' }}
            >
              <button
                onClick={() => {
                  setShowAnswer(true);
                  handleQualitySubmit(1);
                }}
              >
                Forgot
              </button>
              <button
                onClick={() => {
                  setShowAnswer(true);
                  handleQualitySubmit(3);
                }}
              >
                Good
              </button>
              <button
                onClick={() => {
                  setShowAnswer(true);
                  handleQualitySubmit(5);
                }}
              >
                Easy
              </button>
            </div>
          </div>
        </div>
      ) : (
        <div>No cards due for review are left</div>
      )}
    </div>
  );
}
