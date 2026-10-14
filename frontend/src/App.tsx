import { useEffect, useState, type FormEvent } from 'react';
import { createNote, deleteNote, fetchNotes } from './api';
import { useAppDispatch, useAppSelector } from './store/hooks';
import { noteAdded, noteRemoved, notesLoaded } from './store/notesSlice';

function App() {
  const dispatch = useAppDispatch();
  const notes = useAppSelector((state) => state.notes.items);
  const [title, setTitle] = useState('');
  const [content, setContent] = useState('');
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    fetchNotes()
      .then((loaded) => dispatch(notesLoaded(loaded)))
      .catch((e: Error) => setError(e.message));
  }, [dispatch]);

  async function handleSubmit(event: FormEvent) {
    event.preventDefault();
    try {
      const note = await createNote(title, content);
      dispatch(noteAdded(note));
      setTitle('');
      setContent('');
      setError(null);
    } catch (e) {
      setError((e as Error).message);
    }
  }

  async function handleDelete(id: string) {
    try {
      await deleteNote(id);
      dispatch(noteRemoved(id));
    } catch (e) {
      setError((e as Error).message);
    }
  }

  return (
    <main>
      <h1>Notizen</h1>

      <form onSubmit={handleSubmit}>
        <label htmlFor="title">Titel</label>
        <input id="title" value={title} onChange={(e) => setTitle(e.target.value)} />

        <label htmlFor="content">Inhalt</label>
        <textarea id="content" value={content} onChange={(e) => setContent(e.target.value)} />

        <button type="submit">Speichern</button>
      </form>

      {error && <p role="alert">{error}</p>}

      <ul>
        {notes.map((note) => (
          <li key={note.id}>
            <strong>{note.title}</strong>
            <p>{note.content}</p>
            <button onClick={() => handleDelete(note.id)}>Löschen</button>
          </li>
        ))}
      </ul>
    </main>
  );
}

export default App;
