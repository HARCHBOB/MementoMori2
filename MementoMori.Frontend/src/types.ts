export interface Ticket {
  id: number;
  title: string;
  description: string;
  submissionDate: string;
  status: 'Open' | 'In Progress' | 'Resolved';
  user?: string;
  comments: {text: string; isAdmin: boolean}[];
}
